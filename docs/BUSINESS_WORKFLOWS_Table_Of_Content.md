# Business Workflow Reference

This document captures the business rules and state machines embedded in the code. These rules are not documented anywhere else — they exist only in controller logic and are critical institutional knowledge.

---

## Summary & Index

This reference describes **10 business domains** inside the API HUNT application. Broadly, the system handles two generations of integration workflow (a legacy HomeController pipeline and a newer Partner-based system), plus supporting functions for approvals, exceptions, ticketing, offboarding, auditing, and admin.

| # | Section | What it covers |
|---|---------|----------------|
| 1 | [Login & Session Management](#1-login--session-management) | Concurrent session prevention (5-min rule), 3-strike AD lockout, account status messages (Locked/Dormant/Disabled), ISAC escalation. |
| 2 | [New Integration Workflow (HomeController)](#2-new-integration-workflow-homecontroller) | Legacy 3-stage review pipeline: BTGUSER → ITUSER → ITARCHITECH. Role-based edit rights, status filter codes, draft (`12`) vs submitted, document folder naming. |
| 3 | [Partner Onboarding (New System)](#3-partner-onboarding-new-system) | Creating partners with Client Profile + Risk Assessment uploads, PartnerID generation, role-based routing (USER vs approver). |
| 4 | [Partner Integration (New System)](#4-partner-integration-new-system) | Case creation against approved partners, risk-driven approval matrix (5 depts × 3 levels), status lifecycle from Drafted → Approved/Reject. |
| 5 | [Multi-Department Approval](#5-multi-department-approval-partnerapprovalcontroller) | How approvers see and action cases, Approve/Reject/Feedback paths, `Awaiting For Reply` loop, current-approver detection via session. |
| 6 | [Exception Management](#6-exception-management) | 3-level approver escalation driven by `ImpactOnBank`, link back to original integration via `OriginalOnboardingGASID` (must contain "APIGW"). |
| 7 | [JIRA Ticket Creation](#7-jira-ticket-creation) | Decision tree routing by Platform (Internal/External), Middleware (SOA/OBP), and Existing/New. Deduplication flags, attachment upload, OBP Excel generation. |
| 8 | [Partner Offboarding](#8-partner-offboarding) | Search partner → select APIs → deactivate. Restricted file types (`.pdf`, `.jpg`, `.jpeg`), GUID-based storage. |
| 9 | [Activity Logging](#9-activity-logging) | Standard audit record shape (`Emp_Code`, `Form_Name`, `Activity`, etc.) written for every significant action. Module is always `"API HUNT"`. |
| 10 | [Admin Operations](#10-admin-operations) | CRUD for **User Master** (drives role-based visibility) and **App Master** (feeds Producer/Consumer dropdowns). |

### Key cross-cutting concepts

- **Two integration systems coexist.** Sections 2 (HomeController) and 4 (Partner Integration) are parallel workflows — legacy and new — not a single pipeline. Be careful not to conflate them.
- **Role gates everything.** `Session["Role"]` and `AccessRight` decide what the user sees and whether edit controls are enabled. A user viewing an item outside their queue gets `"No AccessRight"`.
- **Status strings are load-bearing.** Values like `"Drafted"`, `"Created"`, `"In Progress"`, `"Awaiting For Reply"`, `"Feedback by BTG User"`, and the numeric code `12` (Draft) are compared as literals throughout the codebase.
- **ISAC is the escape hatch.** Any account-state failure in Section 1 funnels the user to ISAC — there is no in-app recovery.
- **Approver identity is session-derived.** Section 5's "current approver" is resolved by matching `Session["EmpId"]` against approval trail rows at request time, not stored on the case.

### Suggested reading order

- **First-time readers:** 1 → 2 → 3 → 4 → 5 (login, then each workflow end-to-end).
- **Migration / refactor work:** 2 and 4 side-by-side to understand the legacy-vs-new split, then 7 (JIRA) and 9 (logging) for integration points.
- **Auditors / compliance:** 5, 6, 9 — approvals, exceptions, and the audit trail.

---

## 1. Login & Session Management

### Concurrent Session Prevention
```
IF LastLogoutDate < LastLoginDate (user didn't logout)
  AND LastLoginDate was within the last 5 minutes
    → Block login: "User is already Logged-in"
  ELSE (more than 5 minutes ago)
    → Allow login (assume stale session)
```

### Account Lockout
```
IF Active Directory authentication fails
  → Increment unsuccessful attempt counter
  IF counter reaches 3
    → Lock the user ID (set Locked = 1 in UserMaster)
```

### Account Status Check
```
IF UserMaster.Active = 0
  → Check Status:
    "Locked"   → "Your LoginID Is Locked. Kindly raised the request in ISAC."
    "Dormant"  → "Your LoginID Is Dormant. Kindly raised the request in ISAC."
    "Disabled" → "Your LoginID Is Disabled. Kindly raised the request in ISAC."
IF user not found in UserMaster
  → "Your ID is not mapped, Kindly raised the request in ISAC."
```

ISAC is the internal IT service request system where users must raise tickets for access issues.

### Logout
On logout, the user is redirected to the LogOut.

---

## 2. New Integration Workflow (HomeController)

This is the original, most complex workflow. It routes integration requests through a review pipeline based on user roles.

### Role-Based Routing Matrix

```
USER creates integration
  → Assigned to BTGUSER queue

BTGUSER reviews
  → Can approve (moves to ITUSER queue)
  → Can reject (Status = "Rejected")
  → Can send feedback (Status = "Feedback by BTG User")

ITUSER reviews
  → Can approve (moves to ITARCHITECH queue)
  → Can reject
  → Can send feedback (Status = "Feedback by ITUser")

ITARCHITECH reviews
  → Can approve (Status = "Closed")
  → Can reject
  → Can send feedback (Status = "Feedback by IT ARCHITECH")
```

### Access Rights Logic
Each role can only **edit** items assigned to their queue. When viewing items in other queues, `AccessRight` is set to `"No AccessRight"` in the session, disabling edit controls in the view.

### Status Filter Codes (URL parameter)
| Code | Filters to |
|------|-----------|
| `1` | Review To BTGUser |
| `2` | Rejected |
| `3` | Review To ITUSER |
| `4` | Review To ITARCHITECH |
| `5` | Closed |
| `6` | Feedback to current user role |

### Draft vs Submit
- Workflow status `12` = Draft (can be edited freely)
- All other statuses are submitted and follow the review pipeline
- Drafts can be deleted; submitted items cannot

### Document Folder Convention
```
FolderName = "API" + CreatedDate.ToString("ddMMMyyyy") + IntegrationId
Example: "API20Apr2026123"

If ParentIntegrationId exists (child integration):
  FolderName uses ParentIntegrationId instead
```

---

## 3. Partner Onboarding (New System)

### Creation Flow
```
1. USER navigates to AddPartnerOnBoarding
2. Fills partner details + uploads Client Profile Sheet and Risk Assessment Sheet
3. System generates a new PartnerID
4. Files stored as: {PartnerID}_{DocType}{extension}
5. Partner saved with status based on Action field
```

### Role-Based View Routing
```
IF Role == "USER" → Show partner list (PartnerOnboardingController.ListofPartner)
ELSE → Redirect to PartnerApproval/Index (approval view)
```

---

## 4. Partner Integration (New System)

### Case Creation Flow
```
1. USER selects a partner from dropdown (populated from approved partners)
2. Partner details auto-populated via AJAX (GetPartnerDetails)
3. API details added (name, risk score, risk classification)
4. Approval matrix auto-populated based on risk classifications
5. System generates CaseID
6. Case saved as "Draft" or "Created"
7. If Created → approval trail records inserted for all assigned approvers
```

### Approval Matrix Population
```
PartnerCaseApprovalMetrix(PartnerRiskClassification, APIRiskClassification, ApproverType, ApproverLevel)
  → Returns list of approver UserIDs for each department/level combination
  → Populated into 5 departments × 3 levels = up to 15 approver slots
  → Null/empty slots are filtered out before saving
```

### Status Flow with Codes
```
"Drafted"              → Can edit, can delete, can submit
"Created" / "Submit"   → Submitted, in approval queue
"In Progress"          → At least one approver has reviewed
"Approved"             → All required approvals received
"Reject"               → Any approver rejected
"Awaiting For Reply"   → Approver sent feedback, waiting for creator response
```

---

## 5. Multi-Department Approval (PartnerApprovalController)

### Approver's View
```
1. Approver sees only cases where they are an assigned approver
2. Filtered to show only "In Progress" and "Created" status cases
3. Approver can:
   a. Approve → Status = "Approved" (if final approver) or advances to next
   b. Reject  → Status = "Reject"
   c. Feedback → Status changes to "Awaiting For Reply"
      - Creator sees feedback in their edit view
      - Creator can reply via FeedbackReply field
      - Once replied, approver sees the reply and can proceed
```

### Current Approver Detection
The system identifies the current approver by matching `Session["EmpId"]` against the approval trail records. It extracts:
- `CurrentApproverLevel` (FH/VH/GH)
- `CurrentDepartment` (HOPP/HOB/etc.)
- `CurrentSequence` (1/2/3)
- `CurrentApprovalTrialID` (database record ID)

---

## 6. Exception Management

### Level Determination
The exception level is determined by `ImpactOnBank` — a classification of how the exception affects the bank. The mapping is stored in the database and returned via `GetExceptionLevel()`.

### Level 1 Approvers (Low Impact)
- Business Vertical Head
- Business Group Head
- CIO Group

### Level 2 Approvers (Medium Impact)
Everything from Level 1, plus:
- ITDRM Vertical Head
- ITDRM Group Head
- CISO Group Head

### Level 3 Approvers (High Impact)
A different, more senior set:
- IT Vertical Head
- BSG Vertical Head
- Business Vertical Head
- Compliance Vertical Head
- ITDRM Vertical Head
- ISG Vertical Head
- APEX Steering Committee

### Exception Case Link
Exceptions reference an `OriginalOnboardingGASID` that links back to the original integration case. The Case ID must contain "APIGW" to load existing details.

---

## 7. JIRA Ticket Creation

### When Tickets Are Created
JIRA tickets are created for approved integrations. The trigger is in `JIRACreatorController.CreateJIRA()`.

### Ticket Type Decision Tree
```
IF IN_Platform == "External"
  └─ IF Existing_New_Id == 1 (Existing)
  │    → PayloadForAPIINT (one ticket for all existing external services)
  └─ IF Existing_New_Id == 2 (New)
       → PayloadForAPIGATE (one ticket for all new external services)

IF IN_Platform == "Internal"
  └─ IF MiddlewareNameId == 82 (SOA)
  │    → PayloadForSOAENHN (one ticket for all SOA services)
  └─ IF MiddlewareNameId == 83 (OBP)
       └─ IF Existing_New_Id == 1 → PayloadForOBPINT
       └─ IF Existing_New_Id == 2 → PayloadForOBPENH
```

### Deduplication
Boolean flags (`gwExistingIdProcessed`, `gwNewIdProcessed`, `obpExistingIdProcessed`, `obpNewIdProcessed`, `soaProcessed`) ensure only one JIRA ticket is created per category, even when multiple services exist in that category. The service names are concatenated into a single description.

### Post-Creation
After ticket creation:
1. JIRA key is stored in the database via `InsertJIRAId(ServiceID, key)`
2. Integration documents are uploaded as attachments to the JIRA ticket
3. An OBP document (Excel) is generated on-the-fly via `HomeController.Table_Export_IntegratedXL()`

---

## 8. Partner Offboarding

### Simple Deactivation Flow
```
1. Search for partner by name (autocomplete)
2. Load partner's API list
3. Select APIs to deactivate
4. Submit → UpdateAPIStatus(apiName, false) for each selected API
5. Redirect to PartnerApproval index
```

### File Upload
Offboarding supports document uploads (`.pdf`, `.jpg`, `.jpeg` only) for both primary and "other" documents. Files are stored with GUID filenames in `wwwroot/uploads/`.

---

## 9. Activity Logging

Every significant user action is logged with this structure:
```
Emp_Code        → Who did it
Form_Name       → Which module (e.g., "Partner Onboarding", "Admin", "Login")
Module_Name     → Always "API HUNT"
Total_Count     → Always 1
Activity        → What happened (e.g., "Admin Insert AppMaster")
Activity_Details→ Details (e.g., "Admin Insert AppMaster - 1234567")
Activity_Date   → NOW() at insert time
```

---

## 10. Admin Operations

### User Master
- CRUD for application users
- Fields: UserID (EmpCode), UserName, Role, ApproverDept, Email, Status
- Roles assigned here determine what the user sees in the app

### App Master
- CRUD for registered applications
- Fields: APPShortName, FullName, Purpose, HostingDC, ITGRCCode, ITGRCName, Department, SpocLevel, Status
- These applications appear in dropdowns when creating integrations (Producer/Consumer app selection)
