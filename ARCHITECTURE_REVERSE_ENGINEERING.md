# HUNT - API Management Partner Lifecycle Management System
## Complete Reverse Engineering & Architecture Documentation

**Document Purpose:** Complete architectural blueprint for migrating Hunt application to Java/Python or other technologies

**System Context:** API management at organization level - Team onboarding for API consumption with multi-level approval workflows

**Technology Stack (Current):**
- Framework: ASP.NET Core 8.0 MVC
- Database: SQL Server (database name: Hunt)
- Authentication: Active Directory (LDAP) - ldap.hunt.com
- External Integration: JIRA API
- Data Export: EPPlus (Excel generation)
- Session Management: HttpContext.Session (cookie-based)

---

## SECTION 1: HIGH-LEVEL ARCHITECTURE OVERVIEW

### 1.1 System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         USER INTERFACE LAYER (MVC Views)                    │
│  ┌──────────────────┐ ┌──────────────────┐ ┌──────────────────────────────┐│
│  │ Partner Portal   │ │ Approver Portal  │ │ Admin Dashboard              ││
│  │ - Onboarding    │ │ - View Approvals │ │ - User Management            ││
│  │ - Offboarding   │ │ - Approve/Reject │ │ - App Configuration          ││
│  │ - Integration   │ │ - Feedback       │ │ - System Settings            ││
│  │ - Dashboard     │ │ - Approval Trail │ │                              ││
│  └──────────────────┘ └──────────────────┘ └──────────────────────────────┘│
└────────────────────────────────────┬────────────────────────────────────────┘
                                     │
┌────────────────────────────────────▼────────────────────────────────────────┐
│                            APPLICATION LAYER (Controllers)                   │
│  ┌─────────────────────────────────────────────────────────────────────────┐│
│  │ Partner Lifecycle Management (8 Controllers)                            ││
│  │ ├─ PartnerOnboardingController (Legacy & New)                           ││
│  │ ├─ PartnerApprovalController (Legacy & New)                             ││
│  │ ├─ PartnerIntegrationController (Legacy & New)                          ││
│  │ ├─ PartnerOffboardingController                                         ││
│  │ ├─ PartnerDashboardController                                           ││
│  │ └─ Partner_IntegrationController                                        ││
│  └─────────────────────────────────────────────────────────────────────────┘│
│  ┌─────────────────────────────────────────────────────────────────────────┐│
│  │ Exception Management (3 Controllers)                                    ││
│  │ ├─ ExceptionManagementController (Create/Edit)                          ││
│  │ ├─ ExceptionApprovalController (Multi-level Approval)                   ││
│  └─────────────────────────────────────────────────────────────────────────┘│
│  ┌─────────────────────────────────────────────────────────────────────────┐│
│  │ System & Integration (3 Controllers)                                    ││
│  │ ├─ LoginController (LDAP Authentication)                                ││
│  │ ├─ AdminController (User & App Management)                              ││
│  │ ├─ JIRACreatorController (Issue Tracking)                               ││
│  └─────────────────────────────────────────────────────────────────────────┘│
│  ┌─────────────────────────────────────────────────────────────────────────┐│
│  │ Utilities (2 Controllers)                                               ││
│  │ ├─ HomeController (Navigation)                                          ││
│  │ ├─ CommonController (Session Management)                                ││
│  └─────────────────────────────────────────────────────────────────────────┘│
└────────────────────────────────────┬────────────────────────────────────────┘
                                     │
┌────────────────────────────────────▼────────────────────────────────────────┐
│                        BUSINESS LOGIC LAYER (Models & Repositories)         │
│  ┌─────────────────────────────────────────────────────────────────────────┐│
│  │ Repository Classes (Data Access Pattern)                                ││
│  │ ├─ PartnerRepository.cs        ├─ ExceptionRepository.cs                ││
│  │ ├─ PartnerApprovalRepository.cs├─ ExceptionApprovalRepository.cs        ││
│  │ ├─ JIRARepository.cs           ├─ AdminRepository.cs                    ││
│  │ ├─ SubmitRepository.cs         ├─ LoginRepository.cs                    ││
│  └─────────────────────────────────────────────────────────────────────────┘│
│  ┌─────────────────────────────────────────────────────────────────────────┐│
│  │ Utility Classes                                                         ││
│  │ ├─ SendEmail.cs               ├─ AESEncryptDecrypt.cs                  ││
│  │ ├─ CustomFilter.cs            ├─ ResponseContent.cs                    ││
│  │ └─ GetConnectionString.cs                                              ││
│  └─────────────────────────────────────────────────────────────────────────┘│
└────────────────────────────────────┬────────────────────────────────────────┘
                                     │
┌────────────────────────────────────▼────────────────────────────────────────┐
│                           DATA ACCESS LAYER                                 │
│  ┌──────────────────────────────┬─────────────────────────────────────────┐│
│  │ Direct SQL (SqlConnection)   │ Database: Hunt                          ││
│  │ ├─ SqlCommand                │ ├─ Partner Tables                       ││
│  │ ├─ SqlDataAdapter            │ ├─ Exception Tables                     ││
│  │ ├─ Parameterized Queries     │ ├─ Approval Trail Tables                ││
│  │ │  (inconsistent usage)      │ ├─ API Master Tables                    ││
│  │ └─ Stored Procedures         │ ├─ User & Profile Tables                ││
│  │                              │ ├─ Integration Tables                   ││
│  │                              │ └─ Audit & Log Tables                   ││
│  └──────────────────────────────┴─────────────────────────────────────────┘│
└────────────────────────────────────┬────────────────────────────────────────┘
                                     │
┌────────────────────────────────────▼────────────────────────────────────────┐
│                          EXTERNAL INTEGRATIONS                              │
│  ┌─────────────────────────┬──────────────────────────────────────────────┐│
│  │ LDAP Authentication     │ JIRA Integration                              ││
│  │ ├─ Server: ldap.        │ ├─ URL: jira.hunt.com               ││
│  │ │   hunt.com        │ ├─ Endpoint: /rest/api/2/issue               ││
│  │ ├─ User Roles           │ ├─ Authentication: Basic Auth                ││
│  │ ├─ Directory Services   │ └─ Issue Creation & Tracking                 ││
│  │ └─ Active Directory     │                                              ││
│  └─────────────────────────┴──────────────────────────────────────────────┘│
│  ┌──────────────────────────────────────────────────────────────────────────┐│
│  │ Email System                  │ File System                              ││
│  │ ├─ SendEmail.cs               │ ├─ Path: wwwroot/UploadPO/              ││
│  │ ├─ SMTP Configuration         │ ├─ File Types: PDF, DOC, XLSX           ││
│  │ └─ Workflow Notifications     │ ├─ Document Types:                      ││
│  │                               │ │  - Client Profile Sheet              ││
│  │                               │ │  - API Risk Assessment               ││
│  │                               │ │  - Other Documents                   ││
│  │                               │ └─ File Naming: CaseID_DocumentType     ││
│  └──────────────────────────────────────────────────────────────────────────┘│
└────────────────────────────────────────────────────────────────────────────────┘
```

### 1.2 Module Dependency Matrix

```
                          │ P.Boarding │ P.Approval │ P.Integration │ Exception │ Admin │ JIRA │ Login │
─────────────────────────┼────────────┼────────────┼───────────────┼───────────┼───────┼──────┼───────┤
Database (Hunt)          │      ✓     │      ✓     │       ✓       │     ✓     │   ✓   │  ✓   │   ✓   │
LDAP/AD Authentication   │      ✓     │      ✓     │       ✓       │     ✓     │   ✓   │      │   ✓   │
Session Management       │      ✓     │      ✓     │       ✓       │     ✓     │   ✓   │  ✓   │   ✓   │
SendEmail                │      ✓     │      ✓     │       ✓       │     ✓     │       │      │       │
File Upload/Download     │      ✓     │      ✓     │       ✓       │           │       │      │       │
Activity Logging         │      ✓     │      ✓     │       ✓       │     ✓     │   ✓   │  ✓   │   ✓   │
JIRA Integration         │            │            │               │     ✓     │       │  ✓   │       │
Excel Export             │      ✓     │      ✓     │       ✓       │           │   ✓   │      │       │
Approval Matrix          │      ✓     │      ✓     │       ✓       │     ✓     │       │      │       │
Role-Based Access        │      ✓     │      ✓     │       ✓       │     ✓     │   ✓   │      │       │
```

---

## SECTION 2: MODULE SPECIFICATIONS & FEATURES

### 2.1 Partner Onboarding Module

**Controllers Involved:**
- `PartnerOnboardingController` (Legacy - API_HUNT.Controllers namespace)
- `PartnerOnboardingNewController` (Modern - Hunt.Controllers namespace)

**Key Features:**

| Feature | Description | User Type | Status |
|---------|-------------|-----------|--------|
| **Create Partner Case** | Initialize new partner onboarding | USER | ✓ Complete |
| **Draft Management** | Save partner details progressively | USER | ✓ Complete |
| **Partner Details Form** | Name, Type, Entity Type, Risk Scores | USER | ✓ Complete |
| **API Selection** | Associate APIs with partner | USER | ✓ Complete |
| **Risk Assessment** | Partner Risk Score, API Risk Score | USER | ✓ Complete |
| **Document Upload** | Client profile, Risk assessment, Other docs | USER | ✓ Complete |
| **Submit for Approval** | Route to approvers (auto-assign) | USER | ✓ Complete |
| **Status Tracking** | Created → Drafted → In Progress → Approved/Rejected | USER | ✓ Complete |
| **Feedback Handling** | Respond to approver feedback | USER | ✓ Complete |
| **Auto-Routing** | Route based on Risk Classification Matrix | SYSTEM | ✓ Complete |
| **Department-wise Approval** | Route to HOPP, HOB, HODB, HOISG, HOITDRM | SYSTEM | ✓ Complete |
| **Tentative Go-Live Date** | Track planned launch date | USER | ✓ Complete |
| **TPRM Assessment Applicability** | Flag whether TPRM assessment needed | USER | ✓ Complete |

**Primary Data Flow:**
```
User Creates Draft
  ↓
Add Partner Details (name, type, entity, APIs)
  ↓
Add Risk Information (partner risk score, API risk scores)
  ↓
Upload Supporting Documents
  ↓
Define Approval Chain (select approver per department)
  ↓
Submit for Approval
  ↓
System Routes Based on Risk Classification Matrix
  ↓
Multi-Department Approval Process
  ↓
User Responds to Feedback (if needed)
  ↓
Final Approval/Rejection
  ↓
Partner Activated
```

**Database Tables:**
- `tbl_API_HUNT_Partner_Onboarding` - Main partner onboarding cases
- `tbl_API_HUNT_PartnerOnboarding` - Alternative structure (redundant)
- `tbl_API_HUNT_PartnerCaseServiceList` - APIs associated with partner case
- `tbl_API_HUNT_PO_ApiDetail` - API details for partner case
- `tbl_API_HUNT_MSTPartnerType` - Master: Partner types and entity types
- `tbl_API_HUNT_MstPartnerCaseApprovalMetrix` - Approval routing rules

**Input Parameters:**
```
Partner Details:
  - Partner_Name: varchar(50)
  - Project_Description: varchar(100)
  - Tentative_GoLive_Date: datetime
  - Partner_Type: varchar(100) [Lookup from tbl_API_HUNT_MSTPartnerType]
  - Partner_Entity_Type: varchar(100) [Lookup]
  - Partner_TPRM_Assessment_Applicable: varchar(100) [YES/NO]
  - Partner_Risk: varchar(100) [Lookup]
  - Partner_Risk_Score: varchar(100) [Scale 1-5]

API Details (repeating):
  - API_Name: varchar(100)
  - API_Risk: varchar(100) [Lookup]
  - API_Risk_Score: varchar(100) [Scale 1-5]

Document Uploads:
  - AttachedJourneyDocuments: File path
  - APIRiskAssessment: File path
  - OtherDocument: File path

Approval Configuration:
  - Department: HOPP, HOB, HODB, HOISG, HOITDRM
  - Approver_Level: FH (Functional Head), VH (Vertical Head), GH (Global Head)
```

**Output/Results:**
- Case ID generated (integer identity)
- Status codes: 1 (Created), 2 (Drafted), 3 (In Progress), 10 (Awaiting Reply), 15 (Approved), 20 (Rejected)
- Approval trail created
- Email notifications sent to approvers
- Audit log entries

---

### 2.2 Partner Approval Module

**Controllers Involved:**
- `PartnerApprovalController` (Legacy)
- `PartnerApprovalNewController` (Modern)

**Key Features:**

| Feature | Description | User Type | Status |
|---------|-------------|-----------|--------|
| **View Pending Approvals** | List all cases pending approval | APPROVER | ✓ Complete |
| **Filter by Department** | Filter approvals by department code | APPROVER | ✓ Complete |
| **Filter by Status** | Filter by approval status | APPROVER | ✓ Complete |
| **View Partner Details** | Read-only view of partner case | APPROVER | ✓ Complete |
| **View Approval Matrix** | Display approval chain for case | APPROVER | ✓ Complete |
| **Download Documents** | Download uploaded partner documents | APPROVER | ✓ Complete |
| **Approve Action** | Approve with optional comments | APPROVER | ✓ Complete |
| **Reject Action** | Reject with mandatory reason | APPROVER | ✓ Complete |
| **Provide Feedback** | Request changes from initiator | APPROVER | ✓ Complete |
| **View Approval Trail** | Track all previous approvals | APPROVER | ✓ Complete |
| **Department-level Approval** | Track by HOPP, HOB, HODB, HOISG, HOITDRM | APPROVER | ✓ Complete |
| **Hierarchy-level Approval** | FH → VH → GH sequential | APPROVER | ✓ Complete |
| **Move to Next Approver** | Route to next level automatically | SYSTEM | ✓ Complete |

**Primary Data Flow:**
```
Approval Assigned to Approver (by CaseID, Department, Level)
  ↓
Approver Reviews Partner Details & Documents
  ↓
Decision: Approve/Reject/Request Feedback
  ↓
If Approve:
  └─→ Mark current level approved
      └─→ Route to next level (VH or GH)
      └─→ Send notification to next approver

If Reject:
  └─→ Mark case rejected
      └─→ Notify initiator
      └─→ Case returns to draft

If Feedback:
  └─→ Create feedback record
      └─→ Notify initiator to respond
      └─→ Wait for response
      └─→ Continue approval
```

**Database Tables:**
- `tbl_API_HUNT_PO_ApprovalTrailTable` - Approval trail (legacy)
- `tbl_API_HUNT_PO_ApprovalTrailTable_New` - Approval trail (current)
- `tbl_API_HUNT_POCaseApproverMetrix` - Case-approver mapping
- `tbl_API_HUNT_POFeedbackReply_history` - Feedback and replies
- `tbl_API_HUNT_Feedback` - Alternative feedback structure
- `tbl_API_HUNT_MstPartnerCaseApprovalMetrix` - Approval routing matrix

**Database Relationships:**
```
Partner Case
  ├─ N:1 ─ Partner Details (tbl_API_HUNT_Partner_Onboarding)
  ├─ 1:N ─ Approval Trail (tbl_API_HUNT_PO_ApprovalTrailTable)
  │         ├─ CaseID → tbl_API_HUNT_Partner_Onboarding.CaseID
  │         ├─ Department → HOPP, HOB, HODB, HOISG, HOITDRM
  │         ├─ ApproverLevel → FH, VH, GH
  │         ├─ Sequence → 1, 2, 3 (order of approval)
  │         └─ Status → Pending, Approved, Rejected
  ├─ 1:N ─ Feedback History (tbl_API_HUNT_POFeedbackReply_history)
  │         ├─ CaseID → Partner Case
  │         ├─ ApprovalId → Approval Trail Entry
  │         ├─ Feedback → Text from approver
  │         ├─ Feedback_Reply → Response from initiator
  │         └─ Status → Pending, Resolved
  └─ 1:N ─ API List (tbl_API_HUNT_PartnerCaseServiceList)
```

---

### 2.3 Partner Integration Module

**Controllers Involved:**
- `PartnerIntegrationController` (Legacy)
- `PartnerIntegrationNewController` (Modern)
- `Partner_IntegrationController` (Alternative)

**Key Features:**

| Feature | Description | User Type | Status |
|---------|-------------|-----------|--------|
| **Integration Details Input** | Project info, managers, sponsors | USER | ✓ Complete |
| **Service Selection** | Select APIs/services for integration | USER | ✓ Complete |
| **Service Questions Questionnaire** | Weighted questionnaire for services | USER | ✓ Complete |
| **Document Management** | Upload integration documents | USER | ✓ Complete |
| **Approval Workflow** | Similar to onboarding approval | APPROVER | ✓ Complete |
| **Integration Status Tracking** | Submission → In Progress → Approved | USER | ✓ Complete |
| **Feedback Loop** | Request changes on integration specs | APPROVER | ✓ Complete |
| **Platform Assignment** | Assign to specific platform | USER | ✓ Complete |
| **Consumer/Producer Apps** | Select consumer & producer applications | USER | ✓ Complete |

**Primary Data Structure:**
```
Integration Case
  ├─ Project Information
  │   ├─ ProjectName: varchar(100)
  │   ├─ ProjectId: varchar(100)
  │   ├─ ProjectManagerBTG: varchar(100)
  │   ├─ ProjectManagerIT: varchar(100)
  │   ├─ PlannedGoLiveDate: datetime
  │   ├─ BusinessJustification: varchar(max)
  │   ├─ CostCenterCode: varchar(100)
  │   ├─ BusinessSponsor: varchar(100)
  │   └─ ExecutiveSponsor: varchar(100)
  │
  ├─ Services (1:N relationship)
  │   ├─ ServiceName: varchar(5000)
  │   ├─ Purpose: varchar(100)
  │   ├─ Existing_New: [Existing/New]
  │   ├─ ConsumerApplication: varchar(100)
  │   ├─ ProducerApplication: varchar(100)
  │   ├─ Rest_Soap: [REST/SOAP]
  │   ├─ Transformation: [YES/NO]
  │   ├─ Volume: varchar(100)
  │   ├─ APICategory: [Lookup]
  │   ├─ APIType: [Lookup]
  │   ├─ APIRiskScore: [1-5 scale]
  │   ├─ PartnerRiskScore: [1-5 scale]
  │   └─ Platform: varchar(50)
  │
  ├─ Service Questions (1:N per Service)
  │   ├─ Question: varchar(1000)
  │   ├─ Options: [Multiple choice]
  │   ├─ SelectedOption: varchar(100)
  │   └─ Weightage: float (contributes to decision)
  │
  └─ Documents
      ├─ UserJourneyDocumentPath
      ├─ IntegrationDocumentPath
      ├─ RDConceptNote
      ├─ SequenceDiagram
      └─ ExpectedServiceSpecificationDocument
```

**Database Tables:**
- `tbl_API_HUNT_Integration` - Main integration case
- `tbl_API_HUNT_ServiceDetails` - Services in integration
- `tbl_API_HUNT_QusServiceDetails` - Service-question mappings
- `tbl_API_HUNT_QuestionData` - Question templates
- `tbl_API_ApplicationsSPOC` - Application & SPOC information

---

### 2.4 Partner Offboarding Module

**Controllers Involved:**
- `PartnerOffboardingController`

**Key Features:**

| Feature | Description | User Type | Status |
|---------|-------------|-----------|--------|
| **Select Partner** | Choose partner to offboard | USER | ✓ Complete |
| **Select APIs** | Choose which APIs to decommission | USER | ✓ Complete |
| **Exit Scenario** | Define exit scenario/reason | USER | ✓ Complete |
| **Partner Checklist** | Compliance checklist for exit | USER | ✓ Complete |
| **Document Upload** | Upload decommissioning documentation | USER | ✓ Complete |
| **Mark APIs Inactive** | Change API status to Inactive | SYSTEM | ✓ Complete |
| **Approval Workflow** | Route for offboarding approval | APPROVER | ✓ Complete |
| **Offboarding Status** | Track offboarding workflow | USER | ✓ Complete |

**Database Tables:**
- `tbl_API_HUNT_PartnerOffboardning_getdetails` - Offboarding request details
- `tbl_API_HUNT_PartnerCaseServiceList` - APIs linked to partner

---

### 2.5 Partner Dashboard Module

**Controllers Involved:**
- `PartnerDashboardController`

**Key Features:**

| Feature | Description | User Type | Status |
|---------|-------------|-----------|--------|
| **Partner Summary** | Count of onboarded partners | USER/APPROVER | ✓ Complete |
| **Approval Status Charts** | Visual breakdown of approval statuses | All | ✓ Complete |
| **Case Status Summary** | Pending, Approved, Rejected counts | All | ✓ Complete |
| **Partner List Export** | Export to Excel | USER | ✓ Complete |
| **Approval Trail View** | Historical approval data | APPROVER | ✓ Complete |
| **Performance Metrics** | Approval turnaround time | ADMIN | ☐ Partial/Missing |
| **Active Partners Grid** | Filterable partner table | USER | ✓ Complete |

---

### 2.6 Exception Management Module

**Controllers Involved:**
- `ExceptionManagementController`
- `ExceptionApprovalController`

**Purpose:** Manage exceptions to normal API approval processes (time-bound exceptions, policy waivers, etc.)

**Key Features:**

| Feature | Description | User Type | Status |
|---------|-------------|-----------|--------|
| **Create Exception** | Initiate new exception request | USER | ✓ Complete |
| **Exception Details** | Project info, API info, reason | USER | ✓ Complete |
| **Impact Assessment** | Impact on bank, partners | USER | ✓ Complete |
| **Exception Level Selection** | Level 1, 2, or 3 | USER | ✓ Complete |
| **Start/End Dates** | Validity period for exception | USER | ✓ Complete |
| **Multi-Level Approval** | Based on exception level | APPROVER | ✓ Complete |
| **Exception Status Tracking** | Created → Submitted → Approved/Rejected | USER | ✓ Complete |
| **Level-wise Approval** | Different approvers per level | SYSTEM | ✓ Complete |
| **Exception Audit Trail** | Complete approval history | ADMIN | ✓ Complete |
| **Automatic Expiry** | Exception expires after EndDate | SYSTEM | ☐ Missing |

**Exception Level Matrix:**

| Level | Usage | Approvers | Impact |
|-------|-------|-----------|--------|
| **Level 1** | Minor, departmental | Department Head, Vertical Head | Limited scope |
| **Level 2** | Moderate, cross-department | Department Head, VP, SVP | Medium scope |
| **Level 3** | Major, organization-wide | Executive, VP, Director | High scope |

**Database Tables:**
- `tbl_API_HUNT_ExceptionManagement` - Main exception cases
- `tbl_API_HUNT_NewExceptionManagement` - New exception structure
- `tbl_API_HUNT_ExceptionLevel` - Exception level definitions
- `tbl_API_HUNT_MstExepMetrix` - Exception impact to level mapping
- `tbl_API_HUNT_MstExceptionApprovalMetrix` - Approver matrix for exceptions
- `tbl_API_HUNT_Audit_log` - Exception audit trail

**Exception Case Structure:**
```
Exception Case
  ├─ CaseId: [auto-generated]
  ├─ OriginalOnboardingGASID: [reference to onboarding case]
  ├─ ExceptionRequestor: [EmpCode]
  ├─ APIProjectName: varchar(100)
  ├─ APIProjectDescription: varchar(100)
  ├─ PartnersToBeIntegrated: varchar(100)
  ├─ ProductAPIToBeConsumed: varchar(100)
  ├─ RequestedException: [description of exception]
  ├─ ReasonForException: [business reason]
  ├─ StartDate: date
  ├─ EndDate: date
  ├─ HowExceptionToBeImplemented: varchar(100)
  ├─ ImpactOnBank: [scope of impact]
  ├─ ExceptionLevel: [1, 2, or 3]
  ├─ Status: [Pending/Approved/Rejected/Expired]
  ├─ CreatedBy: [EmpCode]
  ├─ CreatedDate: datetime
  ├─ UpdatedBy: [EmpCode]
  └─ UpdatedDate: datetime
```

---

### 2.7 Authentication & Authorization Module

**Controllers Involved:**
- `LoginController`

**Key Features:**

| Feature | Description | Method | Status |
|---------|-------------|--------|--------|
| **LDAP Authentication** | Validate against Active Directory | ValidateActiveDirectoryLogin() | ✓ Complete |
| **User Master Lookup** | Check user in application DB | GetUserRole() | ✓ Complete |
| **Session Initialization** | Set session variables | HttpContext.Session | ✓ Complete |
| **Concurrent Login Control** | Prevent duplicate logins | LastLogoutDate vs LastLoginDate | ✓ Complete |
| **Login Attempt Tracking** | Log login attempts | CaptureProductivityDetails() | ✓ Complete |
| **Role Assignment** | Assign USER/APPROVER/ADMIN | GetUserRole() from DB | ✓ Complete |
| **Active/Inactive User Check** | Verify user is active | Active flag in tbl_API_HUNT_USER | ✓ Complete |
| **Session Timeout** | Auto logout on timeout | Need config in appsettings.json | ☐ Missing |
| **Multi-factor Authentication** | Additional security layer | | ☐ Missing |

**Authentication Flow:**
```
User Enters Credentials
  ↓
Validate Against LDAP (ldap.hunt.com)
  │
  ├─ Invalid → Show error message
  │
  └─ Valid → Proceed
         ↓
    Check in User Master (tbl_API_HUNT_USER)
         ↓
    Verify User is Active
         ├─ NO → Show status message
         │
         └─ YES → Check for concurrent login
              ├─ Already logged in → Error
              │
              └─ Not logged in → Success
                   ↓
              Create Session Variables:
                - EmpId
                - LoginTime
                - Role (USER/APPROVER/ADMIN)
                - Logid
                   ↓
              Update LastSuccessLoginDate
                   ↓
              Log Activity
                   ↓
              Redirect to Home/Dashboard
```

**Database Tables:**
- `tbl_API_HUNT_USER` - User credentials and roles
- `LoginAttempts` - Login audit trail
- `ProfileMaster` - User profiles/roles
- `tbl_API_HUNT_Activity_Log_Tracker` - Activity logging

**User Roles:**

| Role | Permissions | Access Areas |
|------|-------------|--------------|
| **USER** | Create/Edit own cases, Upload docs | Onboarding, Offboarding, Integration, Exception, Dashboard (view own) |
| **APPROVER** | View assigned approvals, Approve/Reject, Provide feedback | Approval, Dashboard (view assigned), Audit Trail |
| **ADMIN** | System configuration, User management | Admin Panel, User Master, App Configuration, System Settings |

---

### 2.8 Admin Module

**Controller:**
- `AdminController`

**Key Features:**

| Feature | Description | User Type | Status |
|---------|-------------|-----------|--------|
| **User Management** | Create/Edit/Deactivate users | ADMIN | ✓ Complete |
| **Role Assignment** | Assign USER/APPROVER/ADMIN roles | ADMIN | ✓ Complete |
| **Application Master** | Manage applications in system | ADMIN | ✓ Complete |
| **SPOC Management** | Manage Single Points of Contact | ADMIN | ✓ Complete |
| **Approval Matrix Config** | Configure approver routing rules | ADMIN | ☐ Partial |
| **Audit Log View** | Review activity logs | ADMIN | ✓ Complete |
| **System Settings** | Configure parameters | ADMIN | ☐ Partial |

**Database Tables:**
- `tbl_API_HUNT_USER` - User records
- `tbl_API_ApplicationsSPOC` - Application SPOC mappings
- `tbl_API_HUNT_Activity_Log_Tracker` - Audit logs

---

### 2.9 JIRA Integration Module

**Controller:**
- `JIRACreatorController`

**Purpose:** Create and track issues in JIRA system

**Key Features:**

| Feature | Description | Status |
|---------|-------------|--------|
| **JIRA Issue Creation** | Auto-create JIRA ticket for partner case | ✓ Complete |
| **Issue Type Selection** | Story, Bug, Task, Epic | ✓ Complete |
| **Issue Linking** | Associate JIRA ID with case | ✓ Complete |
| **Status Sync** | Keep JIRA status in sync with approvals | ☐ Partial |
| **Comment Integration** | Log approvals/feedback in JIRA | ☐ Partial |

**JIRA Configuration:**
```
Server: jira.hunt.com
Endpoint: /rest/api/2/issue
Authentication: Basic Auth (username/password)
Default Project: Likely IT or API_HUNT
Issue Types: Story, Task, Sub-task, Epic, Bug
Fields: Summary, Description, Assignee, Labels, Attachment
```

**Database Tables:**
- `TBL_API_EXTERNALSERVICES` - External service details
- Uses JIRA_ID field to link cases

---

## SECTION 3: COMPLETE DATABASE SCHEMA

### 3.1 Core Partner Tables

#### Table: `tbl_API_HUNT_Partner_Onboarding` (Main Partner Case)
```sql
CREATE TABLE [dbo].[tbl_API_HUNT_Partner_Onboarding](
    [PartnetOnboading_ID] [int] NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Partner_Name] [varchar](50) NULL,                    -- Partner name (Lookup from master)
    [Project_Description] [varchar](100) NULL,           -- Project justification
    [TentativeGoLive_Date] [date] NULL,                  -- Expected launch date
    [PartnerType] [varchar](100) NULL,                    -- Lookup from tbl_API_HUNT_MSTPartnerType
    [PartnerEntityType] [varchar](100) NULL,             -- Lookup from tbl_API_HUNT_MSTPartnerType
    [PartnerTPRM_Application] [varchar](100) NULL,        -- TPRM system ID if applicable
    [Partnerrisk_score] [varchar](100) NULL,             -- 1-5 scale
    [Partnerrisk] [varchar](100) NULL,                    -- LOW, MEDIUM, HIGH, CRITICAL
    [API_name] [varchar](100) NULL,                      -- APIs associated
    [API_risk] [varchar](100) NULL,                       -- LOW, MEDIUM, HIGH, CRITICAL
    [APIrisk_score] [varchar](100) NULL,                 -- 1-5 scale
    [statusCode] [int] NULL,                             -- 1:Created, 2:Drafted, 3:InProgress, 10:AwaitingReply, 15:Approved, 20:Rejected
    [created_By] [varchar](50) NULL,                     -- EmpCode of initiator
    [created_date] [datetime] NULL,                      -- Case creation timestamp
    [Updated_By] [varchar](20) NULL,                     -- Last updater EmpCode
    [Updated_date] [datetime] NULL,                      -- Last update timestamp
    [AttachedJourneyDocuments] [varchar](150) NULL,      -- File path
    [APIRiskAssessment] [varchar](150) NULL,             -- File path
    [OtherDocument] [varchar](150) NULL                  -- File path
) ON [PRIMARY]
```

#### Table: `tbl_API_HUNT_PartnerCaseServiceList` (APIs Associated with Partner)
```sql
CREATE TABLE [dbo].[tbl_API_HUNT_PartnerCaseServiceList](
    [CaseID] [int] NOT NULL,                             -- FK to Partner_Onboarding
    [APIName] [varchar](100) NULL,                       -- API Name
    [APIRiskClassification] [int] NULL,                  -- 1 (LOW), 2 (MEDIUM), 3 (HIGH), 4 (CRITICAL)
    [APIRiskScore] [varchar](100) NULL,                 -- Score 1-5
    [APIStatus] [varchar](100) NULL,                     -- ACTIVE, INACTIVE, DEPRECATED
    [CreatedBy] [varchar](100) NULL,                     -- EmpCode
    [DateCreated] [datetime] NULL,                       -- Creation timestamp
    [DateUpdated] [date] NULL,                           -- Last update date
    CONSTRAINT PK_PartnerCaseServiceList PRIMARY KEY (CaseID)
)
```

#### Table: `tbl_API_HUNT_PO_ApiDetail` (Alternative API Details)
```sql
CREATE TABLE [dbo].[tbl_API_HUNT_PO_ApiDetail](
    [Id] [int] NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CaseId] [varchar](50) NULL,                         -- FK to Partner_Onboarding
    [APIName] [varchar](100) NULL,
    [APIRisk] [varchar](100) NULL,
    [APIRiskScore] [varchar](10) NULL
) ON [PRIMARY]
```

### 3.2 Approval & Routing Tables

#### Table: `tbl_API_HUNT_PO_ApprovalTrailTable` (Main Approval Trail)
```sql
CREATE TABLE [dbo].[tbl_API_HUNT_PO_ApprovalTrailTable](
    [Id] [int] NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CaseId] [varchar](50) NULL,                         -- FK to Partner Case
    [Department] [varchar](100) NULL,                    -- HOPP, HOB, HODB, HOISG, HOITDRM
    [ApproverLevel] [varchar](100) NULL,                 -- FH (Functional Head), VH (Vertical Head), GH (Global Head)
    [ApproverUserID] [varchar](100) NULL,                -- EmpCode of approver
    [Sequence] [varchar](10) NULL,                       -- 1, 2, 3... (approval order)
    [Status] [varchar](50) NULL                          -- PENDING, APPROVED, REJECTED, AWAITING_FEEDBACK
) ON [PRIMARY]
```

#### Table: `tbl_API_HUNT_MstPartnerCaseApprovalMetrix` (Routing Rules Matrix)
```sql
CREATE TABLE [dbo].[tbl_API_HUNT_MstPartnerCaseApprovalMetrix](
    [PartnerRiskClassification] [varchar](100) NULL,     -- LOW, MEDIUM, HIGH, CRITICAL
    [APIRiskClassification] [varchar](100) NULL,         -- LOW, MEDIUM, HIGH, CRITICAL
    [ApproverType] [varchar](100) NULL,                  -- Department type
    [ApproverLevel] [varchar](100) NULL,                 -- FH, VH, GH
    [ApproverUserID] [varchar](100) NULL,                -- EmpCode
    [ApproverName] [varchar](100) NULL,                  -- Full name
    [ApproverEmail] [varchar](100) NULL,                 -- Email for notifications
    [ApproverSequence] [varchar](100) NULL               -- Order in approval chain
) ON [PRIMARY]
```

#### Table: `tbl_API_HUNT_POFeedbackReply_history` (Feedback & Responses)
```sql
CREATE TABLE [dbo].[tbl_API_HUNT_POFeedbackReply_history](
    [Id] [int] NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CaseID] [int] NULL,                                 -- FK to Partner Case
    [ApprovalId] [int] NULL,                             -- FK to Approval Trail entry
    [Department] [varchar](20) NULL,                     -- Department code
    [approvalLevel] [varchar](50) NULL,                  -- FH, VH, GH
    [feedbackBy] [varchar](50) NULL,                     -- EmpCode of approver giving feedback
    [Role] [varchar](50) NULL,                           -- Role of feedback provider
    [Status] [varchar](20) NULL,                         -- PENDING, RESOLVED
    [CreatedBy] [varchar](50) NULL,                      -- EmpCode of creator
    [CreatedDate] [datetime] NULL,                       -- Feedback creation time
    [Feedback] [varchar](250) NULL,                      -- Feedback text from approver
    [Feedback_Reply] [varchar](100) NULL,                -- Reply from initiator
    [feedbackReplyBy] [varchar](50) NULL,                -- EmpCode of reply author
    [FeedbackID] [varchar](20) NULL                      -- Reference ID
) ON [PRIMARY]
```

### 3.3 Exception Management Tables

#### Table: `tbl_API_HUNT_ExceptionManagement` (Main Exception Cases)
```sql
CREATE TABLE [dbo].[tbl_API_HUNT_ExceptionManagement](
    [ID] [int] NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CaseId] [varchar](100) NULL,                        -- Unique case identifier
    [OriginalOnboardingGASID] [varchar](100) NULL,       -- Reference to parent partner case
    [ExceptionRequestor] [varchar](100) NULL,            -- EmpCode of requester
    [APIProjectName] [varchar](100) NULL,                -- Project name
    [APIProjectDescription] [varchar](100) NULL,         -- Description
    [PartnersToBeIntegrated] [varchar](100) NULL,        -- Partner names
    [ProductAPIToBeConsumed] [varchar](100) NULL,        -- API names
    [RequestedException] [varchar](100) NULL,            -- Exception type/code
    [ReasonForException] [varchar](100) NULL,            -- Business reason
    [StartDate] [date] NULL,                             -- Exception validity start
    [EndDate] [date] NULL,                               -- Exception validity end
    [HowExceptionToBeImplemented] [varchar](100) NULL,   -- Implementation approach
    [ImpactOnBank] [varchar](100) NULL,                  -- Impact scope
    [ExceptionLevel] [varchar](100) NULL,                -- 1, 2, or 3
    [createdBy] [varchar](100) NULL,                     -- Creator EmpCode
    [createdDate] [datetime] NULL,                       -- Creation timestamp
    [updateBy] [varchar](100) NULL,                      -- Last updater EmpCode
    [updatedDate] [datetime] NULL                        -- Last update timestamp
) ON [PRIMARY]
```

#### Table: `tbl_API_HUNT_MstExceptionApprovalMetrix` (Exception Approvers)
```sql
CREATE TABLE [dbo].[tbl_API_HUNT_MstExceptionApprovalMetrix](
    [ExceptionLevel] [varchar](100) NULL,                -- 1, 2, or 3
    [ApproverType] [varchar](100) NULL,                  -- Department/type
    [ApproverLevel] [varchar](100) NULL,                 -- FH, VH, GH
    [ApproverUserID] [varchar](100) NULL,                -- EmpCode
    [ApproverName] [varchar](100) NULL,                  -- Full name
    [ApproverEmail] [varchar](100) NULL                  -- Email
) ON [PRIMARY]
```

#### Table: `tbl_API_HUNT_MstExepMetrix` (Exception Level Mapping)
```sql
CREATE TABLE [dbo].[tbl_API_HUNT_MstExepMetrix](
    [ExceptionImpact] [varchar](50) NULL,                -- Impact scope
    [ExceptionLevel] [varchar](50) NULL                  -- Level 1, 2, or 3
) ON [PRIMARY]
```

### 3.4 Integration Tables

#### Table: `tbl_API_HUNT_Integration` (Integration Cases)
```sql
CREATE TABLE [dbo].[tbl_API_HUNT_Integration](
    [IntegrationId] [int] NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ProjectManagerBTG] [varchar](100) NULL,             -- BTG (Business) PM
    [ProjectManagerIT] [varchar](100) NULL,              -- IT/Technical PM
    [ProjectName] [varchar](100) NULL,                   -- Integration project name
    [ProjectId] [varchar](100) NULL,                     -- Project code/ID
    [PlannedGoLiveDate] [datetime] NULL,                 -- Target launch date
    [BusinessJustification] [varchar](max) NULL,         -- Business case
    [BusinessSponsor] [varchar](100) NULL,               -- Business sponsor name
    [ExecutiveSponsor] [varchar](100) NULL,              -- Executive sponsor name
    [CostCenterCode] [varchar](100) NULL,                -- Cost center for billing
    [UserJourneyDocumentPath] [varchar](100) NULL,       -- File path
    [Feedback] [varchar](500) NULL,                      -- Approver feedback
    [Status] [int] NULL,                                 -- Status code
    [CreatedBy] [varchar](100) NULL,                     -- Creator EmpCode
    [CreatedAt] [datetime] NULL,                         -- Creation timestamp
    [UpdatedBy] [varchar](100) NULL,                     -- Last updater EmpCode
    [UpdatedAt] [datetime] NULL,                         -- Last update timestamp
    [Assign] [varchar](50) NULL,                         -- Assigned to user
    [AssignFrom] [varchar](50) NULL,                     -- Previous assignee
    [IntegrationDocumentPath] [varchar](100) NULL,       -- File path
    [ConsumerApplication] [varchar](100) NULL,           -- Consuming app
    [BTG_USER] [varchar](50) NULL,                       -- Business user
    [IT_USER] [varchar](50) NULL,                        -- IT technical user
    [IT_ARCHITECTURE] [varchar](50) NULL,                -- Architecture user
    [ChannelID] [varchar](50) NULL,                      -- Channel identifier
    [ContainerName] [nchar](10) NULL,                    -- Container/environment
    [RDConceptNote] [varchar](100) NULL,                 -- File reference
    [SequenceDiagram] [varchar](100) NULL,               -- File reference
    [ExpectedServiceSpecificationDocument] [varchar](100) NULL, -- File reference
    [In_Platform] [varchar](50) NULL,                    -- Platform code
    [Parent_IntegrationId] [int] NULL,                   -- Parent integration case
    [ConsumerApplicationId] [int] NULL                   -- Consumer app ID
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
```

#### Table: `tbl_API_HUNT_ServiceDetails` (Services in Integration)
```sql
CREATE TABLE [dbo].[tbl_API_HUNT_ServiceDetails](
    [ServiceID] [int] NOT NULL PRIMARY KEY IDENTITY(1,1),
    [IntegrationId] [int] NULL,                          -- FK to Integration
    [ServiceName] [varchar](5000) NULL,                  -- API/Service name
    [Purpose] [varchar](100) NULL,                       -- Purpose/use case
    [Existing_New] [varchar](100) NULL,                  -- EXISTING or NEW
    [ConsumerApplication] [varchar](100) NULL,           -- Consuming application
    [ProducerApplication] [varchar](100) NULL,           -- Producing application
    [Is_APIGW_Request] [bit] NULL,                       -- API Gateway required?
    [Rest_Soap] [varchar](100) NULL,                     -- REST or SOAP
    [Transformation] [varchar](100) NULL,                -- Data transformation needed?
    [Volume] [varchar](100) NULL,                        -- Expected volume/throughput
    [CreatedBy] [varchar](100) NULL,                     -- Creator EmpCode
    [CreatedAt] [datetime] NULL,                         -- Creation timestamp
    [UpdatedBy] [varchar](100) NULL,                     -- Last updater EmpCode
    [UpdatedAt] [datetime] NULL,                         -- Last update timestamp
    [Existing_New_Id] [int] NULL,                        -- Lookup ID
    [Rest_SOAP_Id] [int] NULL,                           -- Lookup ID
    [ServiceType_Id] [int] NULL,                         -- Lookup ID
    [APIType_Id] [int] NULL,                             -- Lookup ID
    [APICategory_Id] [int] NULL,                         -- Lookup ID
    [APIRiskScore_Id] [int] NULL,                        -- Lookup ID
    [PartnerRiskScore_Id] [int] NULL,                    -- Lookup ID
    [DomainName_Id] [int] NULL,                          -- Lookup ID
    [ConsumerDC_Id] [int] NULL,                          -- Data center ID
    [ProducerDC_Id] [int] NULL,                          -- Data center ID
    [Platform] [varchar](50) NULL,                       -- Platform code
    [QValue1] [int] NULL,                                -- Question answer 1
    [QWeightage1] [real] NULL,                           -- Question 1 weightage
    [QValue2] [int] NULL,                                -- Question answer 2
    ...
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
```

#### Table: `tbl_API_HUNT_QusServiceDetails` (Service-Question Mappings)
```sql
CREATE TABLE [dbo].[tbl_API_HUNT_QusServiceDetails](
    [QusSerID] [int] NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ServiceID] [int] NULL,                              -- FK to Service
    [QID] [int] NULL,                                    -- FK to Question
    [OptionsID] [int] NULL,                              -- Selected option ID
    [IsActive] [bit] NULL,                               -- Active flag
    [CreatedBy] [varchar](50) NULL,                      -- Creator EmpCode
    [CreatedAt] [datetime] NULL,                         -- Timestamp
    [UpdatedBy] [varchar](50) NULL,                      -- Updater EmpCode
    [UpdatedAt] [datetime] NULL                          -- Timestamp
) ON [PRIMARY]
```

### 3.5 User & Authorization Tables

#### Table: `tbl_API_HUNT_USER` (User Accounts)
```sql
CREATE TABLE [dbo].[tbl_API_HUNT_USER](
    [Id] [int] NOT NULL PRIMARY KEY IDENTITY(1,1),
    [EmpCode] [varchar](25) NULL,                        -- Employee code (LDAP username)
    [Role] [varchar](20) NULL,                           -- USER, APPROVER, ADMIN
    [EmailId] [varchar](150) NULL,                       -- Email for notifications
    [IsActive] [int] NULL,                               -- 1: Active, 0: Inactive
    [LastSuccessLoginDate] [datetime] NULL,              -- Last login timestamp
    [CreatedBy] [varchar](25) NULL,                      -- Creator EmpCode
    [CreatedDate] [datetime] NULL,                       -- Account creation date
    [UpdateBy] [varchar](25) NULL,                       -- Last updater EmpCode
    [UpdatedDate] [datetime] NULL                        -- Last update date
) ON [PRIMARY]
```

#### Table: `LoginAttempts` (Login Audit Trail)
```sql
CREATE TABLE [dbo].[LoginAttempts](
    [Id] [numeric](18, 0) NOT NULL PRIMARY KEY,
    [Empcode] [varchar](50) NULL,                        -- Employee code
    [Empname] [varchar](150) NULL,                       -- Full name
    [LoginTime] [datetime] NULL,                         -- Login timestamp
    [LogoutTime] [datetime] NULL,                        -- Logout timestamp
    [Attempts] [int] NULL,                               -- Number of login attempts
    [ProfileId] [int] NULL,                              -- Profile/Role ID
    [ProfileName] [varchar](150) NULL,                   -- Profile/Role name
    [Brcode] [int] NULL,                                 -- Branch code
    [Brname] [varchar](150) NULL,                        -- Branch name
    [IpAddress] [varchar](150) NULL,                     -- User IP address
    [AssetCode] [varchar](50) NULL,                      -- Asset/Computer code
    [Flag] [varchar](50) NULL                            -- Status flag
) ON [PRIMARY]
```

#### Table: `ProfileMaster` (User Profiles/Roles)
```sql
CREATE TABLE [dbo].[ProfileMaster](
    [ProfileId] [numeric](18, 0) NOT NULL PRIMARY KEY,
    [ProfileShortCode] [varchar](50) NULL,               -- Code: USR, APR, ADM
    [ProfileName] [varchar](150) NULL,                   -- USER, APPROVER, ADMIN
    [ProfileDescription] [varchar](500) NULL,           -- Description
    [Maker] [varchar](50) NULL,                          -- Created by
    [MakerDate] [datetime] NULL,                         -- Creation date
    [Authorised] [bit] NULL,                             -- Approved?
    [Authoriser] [varchar](50) NULL,                     -- Approved by
    [AuthoriseDate] [datetime] NULL,                     -- Approval date
    [Flag] [varchar](50) NULL                            -- Status
) ON [PRIMARY]
```

### 3.6 API & Application Master Tables

#### Table: `TBL_API_EXTERNALSERVICES` (External API Definitions)
```sql
CREATE TABLE [dbo].[TBL_API_EXTERNALSERVICES](
    [EXTERNALSERVICE_ID] [int] NOT NULL PRIMARY KEY,
    [COD_SERVICE_ID] [varchar](max) NULL,                -- Service code
    [SERVICE_SIGNATURE] [varchar](max) NULL,             -- Service fingerprint
    [NAM_SERVICE_MIDDLEWARE] [varchar](50) NULL,         -- Middleware name
    [SERVICE_DESC] [varchar](max) NULL,                  -- Description
    [SERVICE_PROVIDER] [varchar](max) NULL,              -- Provider name
    [SERVICE_INTERFACE_TYPE] [varchar](50) NULL,         -- REST, SOAP, gRPC, etc.
    [SERVICE_CATEGORY] [varchar](max) NULL,              -- Category
    [SERVICE_TYPE] [varchar](100) NULL,                  -- Type classification
    [OBP_SERVICE_URL_UAT] [varchar](max) NULL,           -- UAT endpoint
    [OBP_SERVICE_URL_PSUPP] [varchar](max) NULL,         -- Support endpoint
    [OBP_SERVICE_URL_PROD] [varchar](max) NULL,          -- Production endpoint
    [OBP_WSDL_URL] [varchar](max) NULL,                  -- WSDL location
    [SERVICE_VERSION] [varchar](max) NULL,               -- API version
    [TXT_REMARKS] [varchar](max) NULL,                   -- Remarks
    ...
    [API_CAT] [varchar](50) NULL,                        -- API category
    [VIRTUALIZED] [varchar](max) NULL,                   -- Virtualized?
    [AUTOMATED] [varchar](max) NULL,                     -- Automated?
    [DEPRICATED_API] [varchar](max) NULL,                -- Deprecated?
    [DOMAIN_NAME] [varchar](50) NULL,                    -- Domain
    [API_TYPE_] [varchar](100) NULL                      -- API type
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
```

#### Table: `tbl_API_ApplicationsSPOC` (Application SPOC Mapping)
```sql
CREATE TABLE [dbo].[tbl_API_ApplicationsSPOC](
    [Id] [int] NOT NULL PRIMARY KEY IDENTITY(1,1),
    [APPID] [int] NULL,                                  -- Application ID
    [APPShortName] [varchar](100) NULL,                  -- App code
    [SPOCDepartment] [varchar](30) NULL,                 -- Department code
    [SpocEMPCode] [varchar](20) NULL,                    -- SPOC employee code
    [SPOCName] [varchar](150) NULL,                      -- SPOC full name
    [EmailAddress] [varchar](250) NULL,                  -- SPOC email
    [SPOCLevel] [varchar](150) NULL,                     -- SPOC level/role
    [status] [int] NULL,                                 -- Active status
    [CreatedBy] [varchar](25) NULL,                      -- Creator
    [CreatedDate] [datetime] NULL,                       -- Creation date
    [UpdateBy] [varchar](25) NULL,                       -- Last updater
    [UpdatedDate] [datetime] NULL                        -- Last update date
) ON [PRIMARY]
```

#### Table: `TBL_API_Main` (API Catalog)
```sql
CREATE TABLE [dbo].[TBL_API_Main](
    [TBL_API_Main_ID] [int] NOT NULL PRIMARY KEY,
    [COD_SERVICE_ID] [varchar](max) NULL,                -- API code
    [SERVICE_SIGNATURE] [varchar](max) NULL,             -- API signature
    [NAM_SERVICE_MIDDLEWARE] [varchar](100) NULL,        -- Middleware
    [SERVICE_DESC] [varchar](max) NULL,                  -- Description
    [SERVICE_PROVIDER] [varchar](100) NULL,              -- Provider
    [SERVICE_INTERFACE_TYPE] [varchar](100) NULL,        -- REST/SOAP
    [SERVICE_CATEGORY] [varchar](100) NULL,              -- Category
    [SERVICE_TYPE] [varchar](100) NULL,                  -- Type
    [OBP_SERVICE_URL_UAT] [varchar](max) NULL,           -- UAT URL
    [OBP_SERVICE_URL_PSUPP] [varchar](max) NULL,         -- Support URL
    [OBP_SERVICE_URL_PROD] [varchar](max) NULL,          -- Prod URL
    [SERVICE_VERSION] [int] NULL,                        -- Version
    [DAT_GO_LIVE] [datetime] NULL,                       -- Launch date
    [JIRA_ID] [varchar](max) NULL,                       -- JIRA ticket
    ...
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
```

### 3.7 Audit & Activity Tables

#### Table: `tbl_API_HUNT_Activity_Log_Tracker` (Activity Log)
```sql
CREATE TABLE [dbo].[tbl_API_HUNT_Activity_Log_Tracker](
    [Emp_Code] [varchar](50) NULL,                       -- EmpCode
    [Form_Name] [varchar](50) NULL,                      -- UI form name
    [Module_Name] [varchar](50) NULL,                    -- Module name
    [Total_Count] [int] NULL,                            -- Transaction count
    [Activity] [varchar](50) NULL,                       -- Activity type: CREATE, UPDATE, APPROVE
    [Activity_Details] [varchar](100) NULL,              -- Details
    [Activity_Date] [datetime] NULL                      -- Timestamp
) ON [PRIMARY]
```

#### Table: `tbl_API_HUNT_Audit_log` (Audit Log)
```sql
CREATE TABLE [dbo].[tbl_API_HUNT_Audit_log](
    [id] [int] NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CaseID] [int] NULL,                                 -- FK to case
    [ApprovalID] [int] NULL,                             -- FK to approval
    [status] [varchar](20) NULL,                         -- Action status
    [createdBy] [varchar](20) NULL,                      -- EmpCode
    [createdDate] [datetime] NULL,                       -- Timestamp
    [Feedback] [varchar](250) NULL                       -- Feedback/notes
) ON [PRIMARY]
```

### 3.8 Feedback Tables

#### Table: `tbl_API_HUNT_Feedback` (Feedback & Comments)
```sql
CREATE TABLE [dbo].[tbl_API_HUNT_Feedback](
    [Feedback_Id] [int] NOT NULL PRIMARY KEY,
    [Integration_Id] [int] NULL,                         -- FK to Integration
    [Feedback_Details] [varchar](max) NULL,              -- Feedback text
    [Status] [int] NULL,                                 -- Status code
    [Created_By] [varchar](30) NULL,                     -- Creator EmpCode
    [Created_Date] [datetime] NULL,                      -- Creation date
    [IsActive] [bit] NULL,                               -- Active?
    [Updated_By] [varchar](30) NULL,                     -- Last updater
    [Updated_Date] [datetime] NULL,                      -- Last update
    [AssignTo] [nvarchar](20) NULL,                      -- Assigned to user
    [AssignFrom] [nvarchar](20) NULL                     -- Previous assignee
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
```

### 3.9 Master/Configuration Tables

#### Table: `tbl_API_HUNT_MSTPartnerType` (Partner Type Master)
```sql
CREATE TABLE [dbo].[tbl_API_HUNT_MSTPartnerType](
    [PartnerType] [varchar](100) NULL,                   -- E.g., Vendor, Technology Partner, Business Partner
    [PartnerEntityType] [varchar](100) NULL,             -- E.g., Individual, Company, Division
    [TPRMAapplicable] [varchar](100) NULL                -- YES/NO
) ON [PRIMARY]
```

#### Table: `tbl_API_HUNT_Misccd` (Miscellaneous Codes/Master Data)
```sql
CREATE TABLE [dbo].[tbl_API_HUNT_Misccd](
    [MisccdId] [int] NULL,                               -- Code ID
    [CDTP] [varchar](250) NULL,                          -- Code type
    [CDValDesc] [varchar](500) NULL,                     -- Code value description
    [Seq] [int] NULL,                                    -- Sequence
    [Status] [int] NULL,                                 -- Active status
    [LUSR] [varchar](20) NULL,                           -- Last user
    [LUDT] [varchar](20) NULL,                           -- Last update date
    [LUTM] [varchar](20) NULL                            -- Last update time
) ON [PRIMARY]
```

#### Table: `TBL_API_Master` (API Master Data)
```sql
CREATE TABLE [dbo].[TBL_API_Master](
    [ID] [int] NOT NULL PRIMARY KEY,
    ...
) ON [PRIMARY]
```

#### Table: `TBL_API_Master_Values` (Master Data Values)
```sql
CREATE TABLE [dbo].[TBL_API_Master_Values](
    [ID] [int] NOT NULL PRIMARY KEY,
    ...
) ON [PRIMARY]
```

---

## SECTION 4: DATA FLOWS & PROCESS WORKFLOWS

### 4.1 Partner Onboarding End-to-End Flow

```
┌─────────────────────────────────────────────────────────────────────┐
│  INITIATOR (USER) CREATES PARTNER CASE                              │
│  ├─ Input: Partner name, type, entity type, APIs, risk scores      │
│  ├─ DB: Insert into tbl_API_HUNT_Partner_Onboarding (status: Created)   │
│  ├─ Form: Multi-step wizard (5-6 steps)                             │
│  └─ Output: CaseID generated                                        │
└────────────────────────────────┬────────────────────────────────────┘
                                 │
┌────────────────────────────────▼────────────────────────────────────┐
│  INITIATOR UPLOADS DOCUMENTS & DEFINES APPROVAL CHAIN                │
│  ├─ Upload: Client Profile Sheet, API Risk Assessment               │
│  ├─ DB: tbl_API_HUNT_Partner_Onboarding.AttachedJourneyDocuments etc.    │
│  ├─ File storage: wwwroot/UploadPO/CaseID_DocumentType.pdf          │
│  ├─ Select: Approvers per department (HOPP, HOB, HODB, etc.)        │
│  └─ DB: tbl_API_HUNT_PO_ApprovalTrailTable.ApproverUserID               │
└────────────────────────────────┬────────────────────────────────────┘
                                 │
┌────────────────────────────────▼────────────────────────────────────┐
│  INITIATOR SUBMITS FOR APPROVAL                                      │
│  ├─ Action: Click "Submit for Approval"                              │
│  ├─ DB: Update tbl_API_HUNT_Partner_Onboarding.statusCode = 3 (In Prog) │
│  ├─ DB: Create approval trail with Sequence=1, Status=PENDING        │
│  ├─ Lookup: tbl_API_HUNT_MstPartnerCaseApprovalMetrix                    │
│  │  └─ Find approver based on:                                       │
│  │     ├─ Partner Risk Classification                                │
│  │     ├─ API Risk Classification                                    │
│  │     └─ ApproverType (Department)                                  │
│  └─ Email: Send notification to first-level approver (FH)            │
└────────────────────────────────┬────────────────────────────────────┘
                                 │
                    ┌────────────┴────────────┐
                    │                         │
        ┌───────────▼──────────┐   ┌─────────▼──────────┐
        │  APPROVER PROVIDES   │   │  APPROVER REQUESTS │
        │  FEEDBACK            │   │  INFORMATION       │
        └───────────┬──────────┘   └─────────┬──────────┘
                    │                         │
        ┌───────────▼────────────────────────▼────────────┐
        │ Update tbl_API_HUNT_POFeedbackReply_history         │
        │  ├─ Feedback: Comment from approver             │
        │  ├─ feedbackBy: ApproverUserID                  │
        │  └─ Status: PENDING (awaiting reply)             │
        │ Email: Send feedback to initiator               │
        │ Update Case Status: Awaiting For Reply           │
        └───────────┬────────────────────────────────────┘
                    │
        ┌───────────▼────────────────────────┐
        │ INITIATOR RESPONDS TO FEEDBACK      │
        │ ├─ Update documents if needed      │
        │ ├─ Add reply in Feedback_Reply     │
        │ └─ Resubmit for approval           │
        └───────────┬────────────────────────┘
                    │
                    └────────────┐
                                 │
                ┌────────────────▼──────────────────┐
                │  APPROVER REVIEWS & DECIDES       │
                │  ├─ Review partner details        │
                │  ├─ Review documents              │
                │  ├─ Review feedback history       │
                │  └─ Decision: Approve/Reject      │
                └─────────┬──────────────────────────┘
                          │
            ┌─────────────┴─────────────┐
            │                           │
    ┌───────▼────────┐         ┌───────▼────────┐
    │   APPROVE      │         │    REJECT      │
    └───────┬────────┘         └───────┬────────┘
            │                           │
    ┌───────▼────────────┐     ┌───────▼────────────┐
    │ Update approval    │     │ Mark case REJECTED │
    │ trail: Status=Appr │     │ Email initiator    │
    │                    │     │ Case returns to    │
    │ Check if more      │     │ DRAFT state        │
    │ approvals needed   │     └────────────────────┘
    │ (Sequence check)   │
    │                    │
    │ If more approvals: │
    │ └─ Create approval │
    │    trail entry     │
    │    with Seq=2      │
    │ └─ Email VH/GH     │
    │                    │
    │ If no more approvals:
    │ └─ Mark case APPROVED
    │ └─ Email initiator
    │ └─ Activate partner
    │ └─ Create JIRA ticket
    └────────────────┘
```

### 4.2 Exception Management Flow

```
┌──────────────────────────────────────────────┐
│ INITIATOR CREATES EXCEPTION REQUEST           │
│ ├─ Input: Exception reason, duration, scope  │
│ ├─ Select: Exception Level (1, 2, or 3)      │
│ ├─ Dates: StartDate, EndDate                 │
│ └─ DB: Insert tbl_API_HUNT_ExceptionManagement   │
└────────────────┬─────────────────────────────┘
                 │
┌────────────────▼─────────────────────────────┐
│ SYSTEM ROUTES TO LEVEL-SPECIFIC APPROVERS    │
│ ├─ Lookup: tbl_API_HUNT_MstExceptionApprovalMetrix
│ ├─ Match: ExceptionLevel → Approvers         │
│ ├─ Sequence: FH → VH → GH per level          │
│ └─ Create approval trail entries             │
└────────────────┬─────────────────────────────┘
                 │
┌────────────────▼─────────────────────────────┐
│ LEVEL-1 APPROVER REVIEWS & APPROVES/REJECTS  │
│ ├─ If Approve: Move to Level-2 approver      │
│ ├─ If Reject: Exception denied, notify user  │
│ └─ DB: Update tbl_API_HUNT_ExceptionManagement   │
└────────────────┬─────────────────────────────┘
                 │
┌────────────────▼─────────────────────────────┐
│ SEQUENTIAL LEVEL APPROVAL (FH → VH → GH)     │
│ ├─ Level 2: VH reviews                       │
│ ├─ Level 3: GH reviews                       │
│ └─ Final: Exception APPROVED or REJECTED     │
└────────────────┬─────────────────────────────┘
                 │
┌────────────────▼─────────────────────────────┐
│ ON APPROVAL COMPLETION                       │
│ ├─ Update: ExceptionManagement.Status=Appr   │
│ ├─ Email: Notify initiator of approval       │
│ ├─ JIRA: Create ticket if applicable         │
│ └─ Activity: Log in tbl_API_HUNT_Audit_log       │
└──────────────────────────────────────────────┘
```

### 4.3 Document Upload & Management Flow

```
┌───────────────────────────────────────┐
│ USER UPLOADS DOCUMENT VIA FORM         │
│ ├─ File types: PDF, DOCX, XLSX        │
│ ├─ Max size: (Check form validation)  │
│ └─ Field: AttachedJourneyDocuments    │
└────────────┬────────────────────────────┘
             │
┌────────────▼────────────────────────┐
│ SERVER-SIDE FILE HANDLING            │
│ ├─ Path: wwwroot/UploadPO/          │
│ ├─ Naming: CaseID_DocumentType.ext   │
│ ├─ Check: Directory exists (create)  │
│ ├─ Store: Physical file on disk      │
│ └─ DB: Save file path in DB field    │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│ FILE VISIBILITY IN APPROVAL          │
│ ├─ Approver: Can view document       │
│ ├─ Download: Click "Download"        │
│ ├─ Path: Retrieve from DB            │
│ └─ Serve: Download to user's machine │
└────────────────────────────────────┘
```

### 4.4 Email Notification Flow

```
Key Email Events:
┌─────────────────────────────────────────────────┐
│ Event                          │ Recipient      │
├─────────────────────────────────────────────────┤
│ Case Submitted for Approval    │ First Approver │
│ Approval Completed             │ Initiator      │
│ Case Rejected                  │ Initiator      │
│ Feedback Requested             │ Initiator      │
│ Exception Created              │ Approver       │
│ Exception Approved             │ Initiator      │
└─────────────────────────────────────────────────┘

Email Content Template:
├─ CaseID
├─ Partner/Exception details
├─ Action URL (for approval)
├─ Deadline (if applicable)
└─ Contact info
```

### 4.5 Session & Authentication Flow

```
┌────────────────────────────────┐
│ USER VISITS LOGIN PAGE          │
│ ├─ Enters: EmpCode, Password   │
│ └─ Method: POST /Login/Index    │
└────────────┬───────────────────┘
             │
┌────────────▼───────────────────────────┐
│ LDAP AUTHENTICATION                    │
│ ├─ Server: ldap.hunt.com           │
│ ├─ Validates: EmpCode + Password       │
│ ├─ Success: DirectorySearcher returns  │
│ └─ Failure: Invalid credentials error  │
└────────────┬───────────────────────────┘
             │
   ┌─────────┴─────────┐
   │                   │
   NO                 YES
   │                   │
   │        ┌──────────▼──────────┐
   │        │ CHECK USER MASTER   │
   │        │ (tbl_API_HUNT_USER) │
   │        └──────┬───────────────┘
   │               │
   │        ┌──────▼──────────────────┐
   │        │ Verify: User Active=1   │
   │        └──────┬───────────────────┘
   │               │
   │        ┌──────▼─────────────┐
   │        │ Check: Concurrent  │
   │        │ Logins Allowed?    │
   │        └──────┬─────────────┘
   │               │
   │        ┌──────▼──────────────────┐
   │        │ CREATE SESSION           │
   │        │ ├─ EmpId: Set value      │
   │        │ ├─ Role: GetUserRole()   │
   │        │ ├─ LoginTime: Now        │
   │        │ └─ Logid: Generate       │
   │        └──────┬──────────────────┘
   │               │
   │        ┌──────▼──────────────────┐
   │        │ UPDATE LOGIN ATTEMPTS    │
   │        │ ├─ LastSuccessLoginDate │
   │        │ └─ LoginAttempts++      │
   │        └──────┬──────────────────┘
   │               │
   │        ┌──────▼──────────────────┐
   │        │ LOG ACTIVITY             │
   │        │ └─ tbl_API_HUNT_         │
   │        │   Activity_Log_Tracker   │
   │        └──────┬──────────────────┘
   │               │
   │        ┌──────▼──────────────────┐
   │        │ REDIRECT TO HOME/DASH    │
   │        └──────────────────────────┘
   │
   └───► ERROR PAGE
```

---

## SECTION 5: MISSING FEATURES & ENHANCEMENTS

### 5.1 Identified Missing Features

| Feature | Priority | Impact | Effort |
|---------|----------|--------|--------|
| **Session Timeout** | HIGH | Security risk - sessions never expire | MEDIUM |
| **Multi-Factor Authentication** | HIGH | Enhanced security needed | HIGH |
| **Automatic Exception Expiry** | MEDIUM | Manual cleanup required | LOW |
| **Performance Metrics/Dashboard** | MEDIUM | No visibility into approval times | MEDIUM |
| **Bulk Import/Export** | MEDIUM | Manual data entry tedious | MEDIUM |
| **API Version Management** | MEDIUM | Track API changes over time | HIGH |
| **Approval SLA Tracking** | MEDIUM | No deadline enforcement | MEDIUM |
| **Notification Preferences** | LOW | Users can't customize alerts | LOW |
| **Document Versioning** | LOW | Can't track document changes | MEDIUM |
| **Searchable Audit Logs** | MEDIUM | Logs not indexed/searchable | MEDIUM |
| **JIRA Sync (Bidirectional)** | MEDIUM | Manual JIRA status updates | HIGH |
| **API Rate Limiting Config** | MEDIUM | No rate limiting per API | HIGH |
| **Test/Sandbox Environment** | HIGH | Only UAT/PROD support visible | HIGH |
| **Rollback/Revert Approval** | LOW | Can't undo approval | MEDIUM |
| **Delegation of Authority** | MEDIUM | No approval delegation | MEDIUM |
| **Approver Escalation** | MEDIUM | No escalation on timeout | MEDIUM |

### 5.2 Data Integrity Issues Observed

| Issue | Location | Severity | Fix Effort |
|-------|----------|----------|-----------|
| **Duplicate Tables** | Partner tables have >3 versions | MEDIUM | HIGH - Consolidate |
| **Inconsistent Naming** | PartnetOnboading (typo) vs correct spelling | LOW | LOW - Rename |
| **Unused FILLER columns** | TBL_API_Main has FILLER_01-10 | LOW | LOW - Clean up |
| **Missing Foreign Keys** | No FK constraints defined | MEDIUM | MEDIUM - Add constraints |
| **Parameterized Query Inconsistency** | Some queries may use string concatenation | HIGH | MEDIUM - Audit all queries |
| **No Surrogate Key Pattern** | Mix of identity and user-assigned IDs | MEDIUM | HIGH - Standardize |

### 5.3 Code Quality Issues Observed

| Issue | Location | Impact | Fix Effort |
|-------|----------|--------|-----------|
| **Mixed Namespaces** | Hunt.* & API_HUNT.* in same project | MEDIUM | HIGH - Consolidate |
| **Empty Model Stubs** | Hunt.Models have no implementation | HIGH | MEDIUM - Complete migration |
| **Legacy Code Remnants** | Old HomeController still present | LOW | LOW - Remove |
| **Hardcoded Credentials** | LDAP domain hardcoded | CRITICAL | HIGH - Move to config |
| **Direct SQL in Controllers** | Data access not abstracted | MEDIUM | HIGH - Use proper DAL |
| **No Error Handling Standardization** | Try-catch varies widely | MEDIUM | HIGH - Create utilities |

### 5.4 Recommended Enhancements for Migration

1. **Switch to Entity Framework Core** - Eliminate raw SQL queries
2. **Implement Repository Pattern Properly** - Centralize data access
3. **Add Dependency Injection** - Remove tight coupling
4. **Create Service Layer** - Business logic centralization
5. **Implement Unit Testing** - Currently no test projects
6. **Add Logging Framework** - Serilog or similar
7. **Move Secrets to Configuration** - Use secure config management
8. **Implement Distributed Caching** - For approval matrices
9. **Add API Documentation** - Swagger/OpenAPI
10. **Implement Health Checks** - For monitoring

---

## SECTION 6: INTEGRATION POINTS & EXTERNAL DEPENDENCIES

### 6.1 External System Integrations

#### LDAP/Active Directory Integration

**Purpose:** User authentication and role validation

**Configuration:**
```
Server: ldap.hunt.com
Protocol: LDAP (port 389)
Authentication Method: Bind-based authentication
User Query: (uid={username}) OR (samAccountName={username})
Location: LoginController.cs (line 106)

Implementation:
- ValidateActiveDirectoryLogin() method
- DirectorySearcher for LDAP queries
- DirectoryEntry for authentication
```

**Usage:**
- Every login attempt validates against LDAP
- EmpCode must match LDAP username
- Password is encrypted before sending

#### JIRA API Integration

**Purpose:** Issue tracking and management

**Configuration:**
```
Server: jira.hunt.com
API Version: REST API v2
Endpoint: /rest/api/2/issue
Authentication: Basic Auth (username:password in header)
HTTP Method: POST for issue creation

Implementation:
- JIRACreatorController.cs
- HttpClient for REST calls
- JSON request/response format

Sample Request:
POST /rest/api/2/issue
{
  "fields": {
    "project": {"key": "..."},
    "summary": "... case: [CaseID]",
    "description": "Partner details...",
    "issuetype": {"name": "Story|Task|Bug"},
    "assignee": {"name": "..."},
    "labels": ["partner-onboarding", "api-management"]
  }
}

Response:
{
  "id": "10000",
  "key": "PROJECT-1",
  "self": "https://jira.hunt.com/rest/api/2/issue/10000"
}
```

**Database Linkage:**
- JIRA ticket ID stored in TBL_API_Main.JIRA_ID
- Exception Management stores JIRA_ID
- No sync mechanism for status updates (one-way)

#### Email System Integration

**Purpose:** Workflow notifications

**Configuration:**
```
SMTP Server: (Configured in appsettings.json - not visible)
SendEmail.cs class provides:
- Send email to approvers
- Send email to initiators
- Send bulk notifications

Trigger Points:
1. On case submission
2. On approval/rejection
3. On feedback request
4. On exception approval
5. On case completion

Email Template Variables:
- CaseID
- PartnerName
- ApprovalStatus
- DueDate (if applicable)
- ActionURL
- Requester contact info
```

#### File Storage Integration

**Purpose:** Document management

**Configuration:**
```
Storage Type: Local File System
Base Path: wwwroot/UploadPO/
Directory Structure:
├─ wwwroot/
│  └─ UploadPO/
│     ├─ [CaseID]/
│     │  ├─ ClientProfileSheet.pdf
│     │  ├─ APIRiskAssessment.pdf
│     │  └─ OtherDocuments.docx
│     └─ [CaseID]/
│        └─ ...

File Handling:
- Directory.CreateDirectory() in constructor
- Path.Combine() for path construction
- File storage with CaseID prefix
- Download via file stream

Supported Types: PDF, DOC, DOCX, XLSX (assumed)
Max Size: (Need to check HTML form validation)
```

### 6.2 Database Dependencies

**Connection String Source:**
```
Startup.connectionstring (static property)
Location: Not visible in current codebase
Likely in configuration middleware or Startup.cs
Server: SQL Server (2016 or later based on script)
Database: Hunt
Authentication: SQL or Windows
Pooling: Default (likely enabled)
```

**Connection Management:**
```
Pattern:
- New SqlConnection created in each controller constructor
- No connection pooling management visible
- SqlDataAdapter for dataset operations
- SqlCommand for query execution

Risk: Resource leaks if connections not properly closed
```

---

## SECTION 7: ARCHITECTURE PATTERNS & DESIGN DECISIONS

### 7.1 Identified Patterns

| Pattern | Usage | Location | Assessment |
|---------|-------|----------|------------|
| **Repository Pattern** | Data access abstraction | *Repository.cs classes | Partially implemented (empty stubs) |
| **Session-Based Auth** | User context management | LoginController | Legacy, consider token-based |
| **Activity Logging** | Audit trail | Activity_Log_Tracker table | Manual, not centralized |
| **Multi-Level Approval** | Workflow routing | Approval Trail tables | Custom implementation |
| **Master Data Lookup** | Configuration data | Misccd, Master tables | Good approach |
| **Status Lifecycle** | State management | Status codes in case tables | Works but not explicit states |
| **Email Notifications** | Event-driven actions | SendEmail utility | Event-based, good pattern |

### 7.2 Design Anti-Patterns Observed

| Anti-Pattern | Location | Impact | Recommendation |
|--------------|----------|--------|-----------------|
| **God Controllers** | HomeController, PartnerController | 500+ lines each | Split into smaller classes |
| **Tight Coupling** | Direct SQL in controllers | Hard to test, maintain | Abstract to DAL layer |
| **Magic Numbers** | Status codes (1, 2, 3, 10, 15, 20) | Unclear meaning | Use enums |
| **Hardcoded Values** | LDAP domain, file paths | Hard to configure | Externalize to config |
| **Null Reference Issues** | Inconsistent null checks | Runtime exceptions | Use Nullable pattern |
| **Missing Transactions** | Multi-table updates | Data consistency issues | Add transaction support |

### 7.3 Architectural Decisions for Migration

When migrating to Java/Python, consider:

1. **Authentication:**
   - Keep LDAP for compatibility
   - Add JWT tokens for API layer
   - Implement OAuth2 for third-party integration

2. **Data Access:**
   - Use Hibernate/JPA (Java) or SQLAlchemy (Python)
   - Implement proper transaction management
   - Add connection pooling (HikariCP for Java)

3. **Workflow Engine:**
   - Consider Camunda or jBPM for complex approvals
   - Or implement simple state machine pattern

4. **API Layer:**
   - Build REST API alongside MVC
   - Add API versioning
   - Implement GraphQL for complex queries

5. **Caching:**
   - Redis for session and master data
   - Consider Memcached alternative

6. **Messaging:**
   - RabbitMQ/Kafka for async notifications
   - Replace direct email calls

7. **Storage:**
   - Consider S3/Azure Blob for documents
   - Or keep local filesystem with proper backups

---

## SECTION 8: DEPLOYMENT & CONFIGURATION

### 8.1 Current Configuration

**Launch Profiles (launchSettings.json):**
```json
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "applicationUrl": "http://localhost:5089",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "applicationUrl": "https://localhost:7044;http://localhost:5089",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "IIS Express": {...}
  }
}
```

**Application Settings (appsettings.json):**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**Missing Configurations:**
- Database connection string
- LDAP server details
- JIRA credentials
- Email/SMTP settings
- Session timeout
- File upload settings
- Security headers

### 8.2 Build & Deployment

**Build Command:**
```bash
dotnet build
```

**Run Commands:**
```bash
dotnet run                              # Default (HTTP)
dotnet run --launch-profile https       # HTTPS
dotnet run --launch-profile "IIS Express"
```

**Restore Dependencies:**
```bash
dotnet restore
```

**Deployment Targets:**
- IIS (Windows)
- Docker (Linux)
- Azure App Service
- AWS (EC2, ECS, AppRunner)

### 8.3 Recommended Configuration for Migration

```
Environment Variables:
├─ ASPNETCORE_ENVIRONMENT: Production/Development/Staging
├─ DATABASE_CONNECTION_STRING: Server=...;Database=Hunt;...
├─ LDAP_SERVER: ldap.hunt.com
├─ LDAP_PORT: 389
├─ JIRA_SERVER: jira.hunt.com
├─ JIRA_USER: api_user@company.com
├─ JIRA_TOKEN: encrypted_token
├─ SMTP_SERVER: mail.company.com
├─ SMTP_PORT: 587
├─ SMTP_USER: noreply@company.com
├─ SMTP_PASSWORD: encrypted_password
├─ FILE_UPLOAD_PATH: /var/uploads/
├─ MAX_FILE_SIZE_MB: 50
├─ SESSION_TIMEOUT_MINUTES: 30
└─ LOG_LEVEL: Debug/Info/Warning

appsettings.json:
├─ ConnectionStrings
├─ Authentication (LDAP/OAuth)
├─ Authorization (Role mappings)
├─ ExternalServices (JIRA, Email)
├─ FileStorage (Upload paths)
├─ Logging (Serilog config)
└─ FeatureFlags (New features)
```

---

## SECTION 9: METRICS & KEY PERFORMANCE INDICATORS

### 9.1 Business Metrics to Track

| Metric | Formula | Purpose | Current State |
|--------|---------|---------|----------------|
| **Partner Onboarding Cycle Time** | End date - Start date | Track approval speed | ❌ No metric exists |
| **Approval Turnaround Time** | Sum of all approval durations | Measure efficiency | ❌ No metric exists |
| **Exception Approval Rate** | Approved / Total submitted | Success rate | ❌ Not tracked |
| **Rejection Rate** | Rejected / Total submitted | Quality gate metric | ❌ Not tracked |
| **Partners Onboarded** | Count(status=Approved) | Growth metric | ✓ Available in dashboard |
| **Active Partners** | Count where status=Active | Portfolio size | ✓ Available in dashboard |
| **APIs per Partner** | Avg(Count APIs/Partner) | Integration density | ❌ Not calculated |
| **Exception Duration** | Avg(EndDate - StartDate) | Exception scope | ☐ Partially available |

### 9.2 Technical Metrics to Monitor

| Metric | Message/Data | Current Visibility |
|--------|--------------|-------------------|
| **Database Performance** | Query execution time | No APM in place |
| **API Response Time** | JIRA integration latency | Not measured |
| **LDAP Auth Success Rate** | Login attempts vs success | LoginAttempts table |
| **Concurrent Users** | Active session count | Session table query |
| **Document Upload Size** | Avg/Max file sizes | File system scan needed |
| **Error Rates** | 500 errors, exceptions | Application logging |
| **Email Delivery** | Success/failure ratio | No email system tracking |

---

## SECTION 10: MIGRATION ROADMAP TO JAVA/PYTHON

### 10.1 Phase 1: Planning & Analysis (Weeks 1-2)
- [ ] Complete architectural documentation (✓ Done)
- [ ] Database schema analysis & optimization
- [ ] Identify custom business logic
- [ ] Create detailed technical specifications
- [ ] Team training on target platform

### 10.2 Phase 2: Foundation Setup (Weeks 3-5)
- [ ] Set up Java/Python project structure
- [ ] Implement data access layer (Hibernate/SQLAlchemy)
- [ ] Implement authentication (LDAP + JWT)
- [ ] Set up logging framework
- [ ] Database migration scripts
- [ ] Environment configuration

### 10.3 Phase 3: Core Feature Migration (Weeks 6-12)
- [ ] Partner Onboarding Module
- [ ] Partner Approval Module
- [ ] Partner Integration Module
- [ ] Partner Offboarding Module
- [ ] Dashboard & Reports

### 10.4 Phase 4: Supporting Features (Weeks 13-16)
- [ ] Exception Management
- [ ] Admin Module
- [ ] JIRA Integration
- [ ] Email notifications
- [ ] Activity logging

### 10.5 Phase 5: Testing & Optimization (Weeks 17-20)
- [ ] Unit testing (80%+ coverage target)
- [ ] Integration testing
- [ ] Performance testing
- [ ] Security testing (OWASP Top 10)
- [ ] UAT with business users

### 10.6 Phase 6: Deployment & Cutover (Weeks 21-24)
- [ ] Staging environment setup
- [ ] Data migration testing
- [ ] Parallel run period
- [ ] Production deployment
- [ ] Legacy system decommissioning

### 10.7 Post-Migration (Ongoing)
- [ ] Monitor performance & errors
- [ ] Gather user feedback
- [ ] Fix critical issues
- [ ] Plan Phase 2 enhancements

---

## SECTION 11: MIGRATION-SPECIFIC GUIDANCE

### 11.1 For Java Migration (Spring Boot)

**Project Structure:**
```
hunt-api-management/
├─ src/
│  ├─ main/
│  │  ├─ java/com/bank/hunt/
│  │  │  ├─ controllers/
│  │  │  │  ├─ PartnerOnboardingController.java
│  │  │  │  ├─ PartnerApprovalController.java
│  │  │  │  └─ ...
│  │  │  ├─ services/
│  │  │  │  ├─ PartnerOnboardingService.java
│  │  │  │  └─ ...
│  │  │  ├─ repositories/
│  │  │  │  ├─ PartnerRepository.java
│  │  │  │  └─ ...
│  │  │  ├─ entities/
│  │  │  │  ├─ PartnerOnboarding.java
│  │  │  │  └─ ...
│  │  │  ├─ dto/
│  │  │  │  ├─ PartnerOnboardingDTO.java
│  │  │  │  └─ ...
│  │  │  ├─ config/
│  │  │  │  ├─ SecurityConfig.java
│  │  │  │  ├─ JpaConfig.java
│  │  │  │  └─ ...
│  │  │  ├─ exception/
│  │  │  │  ├─ GlobalExceptionHandler.java
│  │  │  │  └─ ...
│  │  │  └─ HuntApplication.java
│  │  └─ resources/
│  │     ├─ application.yml
│  │     ├─ application-dev.yml
│  │     ├─ application-prod.yml
│  │     └─ db/
│  │        └─ migration/ (Flyway)
│  └─ test/
│     ├─ java/...
│     └─ resources/
├─ pom.xml
├─ Dockerfile
└─ README.md
```

**Key Dependencies:**
```xml
<dependency>
    <groupId>org.springframework.boot</groupId>
    <artifactId>spring-boot-starter-web</artifactId>
</dependency>
<dependency>
    <groupId>org.springframework.boot</groupId>
    <artifactId>spring-boot-starter-data-jpa</artifactId>
</dependency>
<dependency>
    <groupId>com.microsoft.sqlserver</groupId>
    <artifactId>mssql-jdbc</artifactId>
    <version>9.4.1.jre8</version>
</dependency>
<dependency>
    <groupId>org.springframework.security</groupId>
    <artifactId>spring-security-ldap</artifactId>
</dependency>
<dependency>
    <groupId>org.springframework.boot</groupId>
    <artifactId>spring-boot-starter-mail</artifactId>
</dependency>
<dependency>
    <groupId>io.jsonwebtoken</groupId>
    <artifactId>jjwt</artifactId>
    <version>0.11.5</version>
</dependency>
```

### 11.2 For Python Migration (Django/FastAPI)

**Project Structure (Django):**
```
hunt_api_management/
├─ hunt/          # Main project
│  ├─ settings.py
│  ├─ urls.py
│  └─ wsgi.py
├─ onboarding/    # App 1
│  ├─ models.py
│  ├─ views.py
│  ├─ urls.py
│  ├─ serializers.py
│  └─ tests.py
├─ approval/      # App 2
│  ├─ models.py
│  ├─ views.py
│  └─ ...
├─ exceptions/    # App 3
│  ├─ models.py
│  ├─ views.py
│  └─ ...
├─ admin_panel/   # App 4
├─ common/        # Shared utilities
│  ├─ authentication.py
│  ├─ permissions.py
│  ├─ tasks.py (Celery)
│  └─ email.py
├─ static/
├─ media/         # uploads/
├─ templates/
├─ manage.py
├─ requirements.txt
├─ Dockerfile
└─ README.md
```

**Key Dependencies:**
```
Django==4.2
djangorestframework==3.14
django-cors-headers
django-filter
python-ldap
PyJWT
celery
redis
pillow (image handling)
openpyxl (Excel)
requests (JIRA API)
python-dotenv (env config)
psycopg2-binary OR pyodbc (for SQL Server)
```

---

## SECTION 12: TESTING STRATEGY

### 12.1 Unit Testing Coverage

| Module | Test Cases | Coverage Target |
|--------|-----------|-----------------|
| PartnerOnboardingService | 25+ | 80% |
| ApprovalRoutingEngine | 30+ | 85% |
| ExceptionManagementService | 20+ | 80% |
| AuthenticationService | 15+ | 90% |
| NotificationService | 20+ | 75% |

### 12.2 Integration Test Scenarios

```
1. Partner Onboarding  Flow
   ├─ Create draft
   ├─ Add details
   ├─ Upload documents
   ├─ Submit for approval
   ├─ Approve all levels
   └─ Verify activate partner

2. Multi-Level Approval
   ├─ Submit case
   ├─ FH approves
   ├─ VH approves
   ├─ GH approves
   └─ Verify status progression

3. Feedback Loop
   ├─ Submit case
   ├─ Approver requests feedback
   ├─ Initiator responds
   ├─ Resubmit
   └─ Verify feedback history

4. Exception Management
   ├─ Create exception
   ├─ Route to approvers
   ├─ Multi-level approval
   └─ Verify exception active
```

### 12.3 Performance Testing

```
Load Profile:
- Concurrent Users: 100
- Peak Load: 500 users
- Duration: 1 hour
- Test Cases:
  ├─ Login (concurrent)
  ├─ Case submission (async)
  ├─ Dashboard queries
  └─ Report generation
```

### 12.4 Security Testing

```
OWASP Top 10 Coverage:
✓ Injection (SQL, Command)
✓ Broken Authentication
✓ Sensitive Data Exposure
✓ Access Control
✓ Security Misconfiguration
✓ XSS & CSRF
✓ Insecure Deserialization
✓ Insufficient Logging
```

---

## APPENDIX A: DATABASE ENTITY RELATIONSHIP DIAGRAM (Conceptual)

```
┌──────────────────────────────────────────────────────────────┐
│                PARTNER ONBOARDING SUBSYSTEM                  │
├──────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌─────────────────────────┐                                 │
│  │ tbl_API_HUNT_Partner_       │                                 │
│  │ Onboarding             │                                 │
│  │ ┌─────────────────────┐  │                                 │
│  │ │PartnerOnboarding_ID │◄─┼──────┐                         │
│  │ │ PartnerName         │  │      │                         │
│  │ │ PartnerType         │  │      │ 1:N                     │
│  │ │ PartnerEntityType   │  │      │                         │
│  │ │ PartnerRiskScore    │  │      │                         │
│  │ │ APIName             │  │      │                         │
│  │ │ APIRiskScore        │  │      │                         │
│  │ │ StatusCode          │  │      │                         │
│  │ │ Created/UpdatedDates│  │      │                         │
│  │ │ Documents           │  │      │                         │
│  │ └─────────────────────┘  │      │                         │
│  └─────────────────────────────────┼──────────────────────────
│                                     │                       │
│                      ┌──────────────┘ ┌─────────────────────┤
│                      │                │                     │
│                      │  ┌─────────────┴─────────┐           │
│                      │  │ tbl_API_HUNT_             │           │
│                      │  │ PartnerCaseService    │           │
│                      │  │ List                  │           │
│                      │  │ ┌─────────────────┐   │           │
│                      │  │ │CaseID (FK)      │   │           │
│                      │  │ │APIName          │   │           │
│                      │  │ │APIRiskClass     │   │           │
│                      │  │ │APIRiskScore     │   │           │
│                      │  │ │APIStatus        │   │           │
│                      │  │ └─────────────────┘   │           │
│                      │  └───────────────────────┘           │
│                      │                                       │
│                      │  ┌──────────────────────────┐         │
│                      │  │ tbl_API_HUNT_PO_             │         │
│                      │  │ ApprovalTrailTable       │         │
│                      │  │ ┌──────────────────────┐ │         │
│                      │  │ │ID (PK)               │ │         │
│                      │  │ │CaseID (FK)           │ │         │
│                      │  │ │Department            │ │ 1:N     │
│                      │  │ │ApproverLevel (FH/VH) │ │◄─────────
│                      │  │ │ApproverUserID        │ │         │
│                      │  │ │Sequence (1,2,3)      │ │         │
│                      │  │ │Status (Pending/Appr) │ │         │
│                      │  │ └──────────────────────┘ │         │
│                      │  └──────────────────────────┘         │
│                      │                                       │
│                      │  ┌──────────────────────────┐         │
│                      │  │ tbl_API_HUNT_POFeedback     │         │
│                      │  │ Reply_history           │         │
│                      │  │ ┌──────────────────────┐ │         │
│                      │  │ │ID (PK)               │ │         │
│                      │  │ │CaseID (FK)           │ │ 1:N     │
│                      │  │ │ApprovalID (FK)       │ │◄─────────
│                      │  │ │Feedback              │ │         │
│                      │  │ │FeedbackReply         │ │         │
│                      │  │ │Status (Pending/Res)  │ │         │
│                      │  │ └──────────────────────┘ │         │
│                      │  └──────────────────────────┘         │
│                      │                                       │
└──────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────┐
│            EXCEPTION MANAGEMENT SUBSYSTEM                    │
├──────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌────────────────────────────────┐                          │
│  │ tbl_API_HUNT_ExceptionManagement   │                          │
│  │ ┌──────────────────────────┐   │                          │
│  │ │ID (PK)                   │   │                          │
│  │ │CaseID                    │◄──┼───────┐                  │
│  │ │OriginalOnboardingGASID   │   │       │ 1:N             │
│  │ │ExceptionRequestor        │   │       │                 │
│  │ │APIProjectName            │   │       │                 │
│  │ │ReasonForException        │   │       │                 │
│  │ │StartDate / EndDate       │   │       │                 │
│  │ │ExceptionLevel (1/2/3)     │   │       │                 │
│  │ │ImpactOnBank              │   │       │                 │
│  │ │Status (Pending/Appr)     │   │       │                 │
│  │ │CreatedBy / UpdatedDates  │   │       │                 │
│  │ └──────────────────────────┘   │       │                 │
│  └────────────────────────────────────────┤                 │
│                                     │
│              ┌──────────────────────┤
│              │                      │
│    ┌─────────▼──────────────────────────────┐              │
│    │ tbl_API_HUNT_MstExceptionAp                │              │
│    │ provalMetrix (Routing Rules)           │              │
│    │ ┌────────────────────────────────────┐ │              │
│    │ │ExceptionLevel (ref: CaseID)        │ │ N:1          │
│    │ │ApproverType                        │ │◄─────────────┤
│    │ │ApproverLevel (FH/VH/GH)            │ │              │
│    │ │ApproverUserID                      │ │              │
│    │ │ApproverName                        │ │              │
│    │ │ApproverEmail                       │ │              │
│    │ └────────────────────────────────────┘ │              │
│    └─────────────────────────────────────────┘              │
│                                                               │
└──────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────┐
│                INTEGRATION SUBSYSTEM                          │
├──────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌─────────────────────────┐       1:N                       │
│  │ tbl_API_HUNT_Integration    │◄─────────────┐                 │
│  │ ┌─────────────────────┐ │              │                 │
│  │ │IntegrationId (PK)   │ │              │                 │
│  │ │ProjectName          │ │         ┌────▼──────────────┐  │
│  │ │ProjectManagerBTG    │ │         │ tbl_API_HUNT_Service  │  │
│  │ │ProjectManagerIT     │ │         │ Details           │  │
│  │ │PlannedGoLiveDate    │ │         │ ┌────────────────┐│  │
│  │ │BusinessJustif.      │ │         │ │ServiceID (PK)  ││  │
│  │ │BusinessSponsor      │ │         │ │IntegrationId   ││  │
│  │ │ExecutiveSponsor     │ │         │ │ServiceName     ││  │
│  │ │CostCenterCode       │ │         │ │Purpose         ││  │
│  │ │Status               │ │         │ │Existing_New    ││  │
│  │ │Documents            │ │         │ │Rest_Soap       ││  │
│  │ └─────────────────────┘ │         │ │Volume          ││  │
│  └─────────────────────────┘         │ │CreatedAt       ││  │
│                                      │ │UpdatedAt       ││  │
│                                      │ └────────────────┘│  │
│                                      └──┬────────────────┘  │
│                                         │ 1:N               │
│                                    ┌────▼──────────────────┐│
│                                    │tbl_API_HUNT_QusService    ││
│                                    │Details                ││
│                                    │┌─────────────────────┐││
│                                    ││QusSerID (PK)        │││
│                                    ││ServiceID (FK)       │││
│                                    ││QID (FK)             │││
│                                    ││OptionsID            │││
│                                    ││IsActive             │││
│                                    │└─────────────────────┘││
│                                    └───────────────────────┘│
│                                                               │
└──────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────┐
│                    USER & SECURITY                           │
├──────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌──────────────────────────┐    ┌──────────────────────────┐│
│  │ tbl_API_HUNT_USER        │    │ ProfileMaster           ││
│  │ ┌──────────────────────┐ │    │ ┌──────────────────────┐││
│  │ │Id (PK)               │ │    │ │ProfileId (PK)        │││
│  │ │EmpCode               │ │    │ │ProfileShortCode      │││
│  │ │Role                  │◄┼────┤ │ProfileName (USER/    │││
│  │ │EmailId               │ │    │ │APPROVER/ADMIN)       │││
│  │ │IsActive              │ │    │ │ProfileDescription    │││
│  │ │LastSuccessLoginDate  │ │    │ │Maker / MakerDate     │││
│  │ │CreatedBy / CreatedDate
                       │ │    │ │Authorised             │││
│  │ │UpdatedBy / UpdatedDate      │ │Authoriser / AuthDate │││
│  │ └──────────────────────┘ │    │ └──────────────────────┘││
│  └──────────────────────────┘    └──────────────────────────┘│
│                                                               │
│  ┌──────────────────────────────┐                            │
│  │ LoginAttempts (Audit)         │                            │
│  │ ┌──────────────────────────┐  │                            │
│  │ │Id (PK)                   │  │                            │
│  │ │Empcode / Empname         │  │                            │
│  │ │LoginTime / LogoutTime    │  │                            │
│  │ │Attempts                  │  │                            │
│  │ │ProfileId / ProfileName   │  │                            │
│  │ │Brcode / Brname           │  │                            │
│  │ │IpAddress / AssetCode     │  │                            │
│  │ │Flag                      │  │                            │
│  │ └──────────────────────────┘  │                            │
│  └──────────────────────────────┘                            │
│                                                               │
└──────────────────────────────────────────────────────────────┘
```

---

## SECTION 13: QUICK REFERENCE GUIDES

### 13.1 Status Codes Quick Reference

```
Partner Onboarding Status Codes:
1   = Created
2   = Drafted
3   = In Progress (Submitted for Approval)
10  = Awaiting For Reply (Feedback provided)
15  = Approved (All levels completed)
20  = Rejected (Any level rejected)

Exception Status:
Multiple status codes (need to verify in DB)
- Submitted
- Approved
- Rejected
- Expired (End date passed)
- Active (Approved and in duration)

API Status:
- ACTIVE
- INACTIVE
- DEPRECATED
```

### 13.2 Department Codes

```
HOPP   = Head of Payments & Processing
HOB    = Head of Business
HODB   = Head of Digital Banking
HOISG  = Head of IT Security & Governance
HOITDRM = Head of IT Delivery & Resource Management
```

### 13.3 Approver Level Codes

```
FH  = Functional Head (Department manager)
VH  = Vertical Head (Division head)
GH  = Global Head (Executive level)
```

### 13.4 Risk Classification Scale

```
Risk Scores: 1 (Lowest) → 5 (Highest)

Risk Labels:
- LOW      (Score: 1)
- MEDIUM   (Score: 2-3)
- HIGH     (Score: 4)
- CRITICAL (Score: 5)
```

---

## CONCLUSION

This comprehensive reverse engineering document provides:

✓ Complete architectural overview with system design patterns
✓ Detailed module specifications and features
✓ Complete database schema with all 50+ tables documented
✓ Data flows and process workflows with diagrams
✓ Identified missing features and recommendations
✓ Integration points and external dependencies
✓ Deployment and configuration guidance
✓ Migration roadmap for Java/Python implementation
✓ Testing strategy and security considerations
✓ Quick reference guides for common codes and statuses

**Key Takeaways for Migration:**

1. **Data Model**: Well-structured with clear entity relationships
2. **Workflows**: Complex multi-level approval logic - consider workflow engine
3. **Integrations**: LDAP, JIRA, Email - all need re-implementation
4. **Code Quality**: Legacy code mixed with newer implementation
5. **Testing**: Improve test coverage (currently low)
6. **Security**: Move credentials to configuration
7. **Performance**: Add caching and async processing
8. **Documentation**: This document serves as technical blueprint

**Estimated Effort for Full Migration: 20-24 weeks**

---

**Document Generated**: 2026-04-06
**Codebase Version**: ASP.NET Core 8.0
**Database**: SQL Server (Hunt)
**Target Audience**: Architects, Developers, Project Managers
