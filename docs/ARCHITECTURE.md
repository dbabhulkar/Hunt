# Hunt (API_HUNT) - Architecture & Institutional Knowledge

## What This Application Does

Hunt is an internal enterprise web application that manages the **full lifecycle of API partner relationships** — from initial onboarding through integration, approval, exception handling, and eventual offboarding. It serves as a governance tool ensuring that every external and internal API integration goes through proper risk assessment, multi-department approval chains, and audit logging.

The application is used by employees identified by their `EmpCode` (employee ID) and authenticated via Active Directory. Different user roles see different views and have different capabilities.

---

## System Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    Browser (Razor Views)                  │
└────────────┬────────────────────────────────┬────────────┘
             │                                │
     ┌───────▼───────┐              ┌─────────▼─────────┐
     │  Controllers   │              │   CustomFilter     │
     │  (MVC Actions) │◄─────────────│  (Session Guard)   │
     └───────┬───────┘              └───────────────────┘
             │
     ┌───────▼───────┐
     │  Repositories  │  (Scoped DI, Interface-based)
     └───────┬───────┘
             │
     ┌───────▼───────┐
     │ IDbConnection  │  (Singleton factory)
     │   Factory      │
     └───────┬───────┘
             │
     ┌───────▼───────┐       ┌──────────────────┐
     │   MySQL DB     │       │  JIRA REST API   │
     │ (via ADO.NET)  │       │  (jira.hunt.com) │
     └────────────────┘       └──────────────────┘
```

### Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core 8.0 MVC |
| Database | MySQL (via MySqlConnector, wrapped in `SqlCommand`/`SqlDataAdapter` patterns) |
| Auth | Active Directory / LDAP (`LDAP://ldap.hunt.com`) |
| Ticketing | JIRA REST API v2 (`https://jira.hunt.com`) |
| Excel Export | EPPlus |
| JSON | Newtonsoft.Json |
| Session | In-memory distributed cache, 30-minute idle timeout |

---

## Authentication & Authorization

### Login Flow (LoginController)

There are **two entry points** for login:

1. **SSO/Query String Login** (`GET /Login/Index?USERID=...&USERNAME=...`):
   - Receives AES-encrypted `USERID` and `USERNAME` as query parameters
   - Decrypts using `AppED.Crypto.AesBase64Wrapper` with key `"Hd\uFFFDC"`
   - Sets session and redirects to Home — no password validation
   - Used when users are redirected from an external portal (Mofee)

2. **Form Login** (`POST /Login/Index`):
   - Validates against Active Directory via LDAP (disabled in local/dev via `isValid = true` bypass)
   - Checks `UserMaster` table for account status (Active/Locked/Dormant/Disabled)
   - **Concurrent session check**: If `LastLogoutDate < LastLoginDate` and login was within last 5 minutes, blocks with "User is already Logged-in"
   - Failed AD attempts are counted; **3 failures lock the account** (`LockUserId`)

### Role System

Roles are stored in `tbl_API_HUNT_USER.Role` and set in session as `"Role"`. Known roles:

| Role | Access Pattern |
|------|---------------|
| `USER` | Standard user — sees onboarding/integration creation views |
| `BTGUSER` | Business Technology Group user — first-level reviewer for integrations |
| `ITUSER` | IT User — second-level reviewer |
| `ITARCHITECH` | IT Architect — final technical reviewer |
| `ADMIN` | Admin panel access (User/App master CRUD) |

The `UserRole` from `TBL_OVI_RM_Hierarchy_Mapping` is a separate field stored in session as `"UserRole"` — this is the organizational role, not the application role.

### Session Variables

| Key | Source | Purpose |
|-----|--------|---------|
| `EmpId` | Login | Employee code — primary user identifier throughout the app |
| `UserName` | UserMaster.EmpName | Display name |
| `UserRole` | TBL_OVI_RM_Hierarchy_Mapping.EmpRole | Organizational role |
| `Role` | tbl_API_HUNT_USER.Role | Application role (drives access control) |
| `LoginTime` | DateTime.Now | Audit timestamp |
| `Logid` | ConnectionDB.LoginUpdate result | Session tracking ID |
| `AccessRight` | HomeController logic | Dynamic access flag ("AccessRight" or "No AccessRight") |
| `folderName` | HomeController.newintegration | Current document folder for file uploads |

### CustomFilter (Session Guard)

`Models/CustomFilter.cs` — An `ActionFilterAttribute` applied to controllers (e.g., `HomeController`) that checks `Session["Role"]`. If empty/null, redirects to `/Login/Index`. This is the only authentication check; there is no middleware-level auth.

---

## Core Business Workflows

### 1. Partner Onboarding (`PartnerOnboardingController`)

**Purpose**: Register new API partners before any integrations can be created.

**Two generations of this workflow exist in the codebase**:
- **Legacy** (`Partner_IntegrationController` + `IPartnerRepository`): Uses `PartnerOnboarding` model, stored procedure `Usp_HUNT_PartnerOnBoarding`, uploads to `wwwroot/UploadPO/`
- **New** (`PartnerOnboardingController` + `IPartnerOnboardingNewRepository`): Uses `PartnerOnboardingNew` model, uploads to `wwwroot/UploadPONew/`

Both are active in the codebase. The "New" version is the primary path.

**Data captured**:
- Partner name, type, sub-type, entity type
- TPRM (Third-Party Risk Management) assessment applicability
- Risk score and classification
- Client profile sheet and risk assessment sheet (file uploads)
- Cost centre

**Status flow**: `Created` → (approval) → `Approved` / `Reject`

### 2. Partner Integration (`PartnerIntegrationController`)

**Purpose**: Create integration cases linking a partner to specific APIs with risk assessments.

**Key concepts**:
- Each integration is a "Case" with a `CaseID`
- Links to an existing partner via `PartnerID`
- Captures API risk classification alongside partner risk
- Supports document attachments (Journey Documents, Other Documents)

**Status flow**: `Drafted` → `Created`/`Submit` → `In Progress` → `Approved`/`Reject` or `Awaiting For Reply`

### 3. Multi-Department Approval Matrix

**This is the most complex part of the system**. Both partner onboarding and integration cases go through a hierarchical approval process across **5 departments**, each with **3 levels**:

**Departments** (encoded as abbreviations):
| Code | Full Name |
|------|-----------|
| `HOPP` | Head Office - Partnership & Products |
| `HOB` | Head Office - Business |
| `HODB` | Head Office - Digital Banking |
| `HOISG` | Head Office - Information Security Group |
| `HOITDRM` | Head Office - IT Digital Risk Management |

**Approval Levels** (sequential within each department):
| Code | Level | Sequence |
|------|-------|----------|
| `FH` | Function Head | 1 |
| `VH` | Vertical Head | 2 |
| `GH` | Group Head | 3 |

Approvers are populated from `PartnerCaseApprovalMetrix` based on partner and API risk classifications. The approval matrix is stored as rows in an approval trail table with `(CaseId, Department, ApproverLevel, ApproverUserID, Status, Sequence)`.

**Feedback mechanism**: Approvers can send feedback instead of approving/rejecting. Status becomes `"Awaiting For Reply"` and the case creator can respond via `FeedbackReply`.

### 4. New Integration Workflow (`HomeController.NewIntegrationList` / `newintegration`)

**Purpose**: The original API integration tracking system (predates the Partner Integration workflow).

**This is the largest controller** (~1500+ lines). It manages a multi-stage review pipeline:

```
USER creates → BTGUSER reviews → ITUSER reviews → ITARCHITECH reviews → Closed/Rejected
```

Each role only sees items assigned to them by default. The `AccessRight` session variable controls edit permissions.

**Workflow status codes** (numeric):
- `12` = Draft
- Other statuses map to: "Review To BTGUser", "Review To ITUSER", "Review To ITARCHITECH", "Rejected", "Closed"
- Feedback statuses: "Feedback by BTG User", "Feedback by ITUser", "Feedback by IT ARCHITECH"

**Integration ID format**: `"API" + CreatedDate(ddMMMyyyy) + IntegrationId` (e.g., `API20Apr2026123`)

**Document storage**: `wwwroot/APIHuntDoc/NewIntegrations/{FolderName}/`

### 5. Exception Management (`ExceptionManagementController`)

**Purpose**: Handle exceptions to standard API governance policies.

**Three escalation levels** with different approver compositions:

| Level | Approvers Required |
|-------|-------------------|
| Level 1 | Business VH, Business GH, CIO VH |
| Level 2 | Level 1 + ITDRM VH, ITDRM GH, CISO GH |
| Level 3 | IT VH, BSG VH, Business VH, Compliance VH, ITDRM VH, ISG VH, APEX Steering Committee |

The exception level is determined by `ImpactOnBank` classification (called via AJAX `GetExceptionLevel`).

Exception cases reference an `OriginalOnboardingGASID` linking back to the original integration case.

### 6. JIRA Integration (`JIRACreatorController`)

**Purpose**: Automatically create JIRA tickets for approved integrations.

**Authentication**: Basic auth with hardcoded credentials (`genobpenh` / `Bank@2023`)

**Ticket creation logic** based on platform:
- **External** (`IN_Platform == "External"`):
  - Existing APIs → `PayloadForAPIINT` (API Integration ticket)
  - New APIs → `PayloadForAPIGATE` (API Gateway ticket)
- **Internal** (`IN_Platform == "Internal"`):
  - SOA middleware (ID 82) → `PayloadForSOAENHN`
  - OBP middleware (ID 83), existing → `PayloadForOBPINT`
  - OBP middleware (ID 83), new → `PayloadForOBPENH`

After ticket creation, documents are zipped and uploaded as JIRA attachments via the attachments API.

**Middleware IDs** (magic numbers):
- `82` = SOA
- `83` = OBP (Oracle Banking Platform)

**Existing_New_Id values**:
- `1` = Existing service
- `2` = New service

### 7. Partner Offboarding (`PartnerOffboardingController`)

**Purpose**: Deactivate APIs when a partner relationship ends.

Minimal implementation — searches for partners by name, lists their APIs, and updates API status to inactive.

### 8. Health Diagnostics (`HomeController.DiagnoseIssue`)

**Purpose**: Quick-check health of middleware platforms.

Redirects to monitoring URLs stored in the database, mapped by platform:
- OBP UAT / OBP Live
- SOA UAT / SOA Live  
- APIGW UAT / APIGW Live

---

## Data Access Patterns

### Connection Factory

`DbConnectionFactory` wraps `SqlConnection` (actually MySqlConnector) and automatically executes `SET NAMES utf8mb4 COLLATE utf8mb4_0900_ai_ci` on every connection open. This ensures proper Unicode support.

### Query Pattern (used everywhere)

```csharp
using var connection = _connectionFactory.CreateConnection();
using var cmd = new SqlCommand("SELECT ... WHERE col = @p_col", connection);
cmd.Parameters.Add("@p_col", SqlDbType.VarChar).Value = value;
var da = new SqlDataAdapter(cmd);
var ds = new DataSet();
connection.Open();
da.Fill(ds);
// Iterate ds.Tables[0].Rows
```

All parameters use `@p_` prefix convention. `CommandTimeout = 0` (infinite) is common.

### Activity Logging

Every significant action is logged to `tbl_API_HUNT_Activity_Log_Tracker` via `IActivityLogRepository.LogActivity()`. Fields: `Emp_Code`, `Form_Name`, `Module_Name`, `Total_Count`, `Activity`, `Activity_Details`, `Activity_Date`.

There is also a legacy `CaptureProductivityDetails` method in `Partner_IntegrationController` that calls a stored procedure `USP_Insert_Data_In_Activity_Log_Tracker_API_HUNT` directly.

---

## Database Tables (inferred from queries)

| Table | Purpose |
|-------|---------|
| `UserMaster` | Employee records — name, code, profile, active/locked/dormant status, login tracking |
| `tbl_API_HUNT_USER` | Application-level user records — role assignment, active status |
| `TBL_OVI_RM_Hierarchy_Mapping` | Organizational hierarchy — maps EmpCode to EmpRole |
| `tbl_API_HUNT_Activity_Log_Tracker` | Audit trail for all user actions |
| `tbl_Mofee_Url` | External portal URL (for logout redirect) |

---

## File Upload Architecture

Three separate upload directories exist under `wwwroot/`:

| Directory | Used By | Naming Convention |
|-----------|---------|-------------------|
| `UploadPO/` | Legacy Partner_IntegrationController | `{CaseID}_{DocType}{ext}` |
| `UploadPONew/` | PartnerOnboardingController | `{PartnerID}_{DocType}{ext}` |
| `UploadPINew/` | PartnerIntegrationController, PartnerApprovalController | `{CaseID}_{DocType}{ext}` |
| `APIHuntDoc/NewIntegrations/` | HomeController (New Integration) | `API{date}{id}/` folder structure |

**File serving**: All controllers iterate through a hardcoded list of extensions (`.png`, `.jpg`, `.jpeg`, `.gif`, `.pdf`, `.docx`, `.xlsx`, `.txt`, `.xls`, `.zip`, `.7z`, `.doc`) to find matching files, since the extension is not stored in the database.

---

## Key Design Decisions & Quirks

### Two parallel partner systems
The codebase has **two complete implementations** of partner management:
- `Partner_IntegrationController` + `IPartnerRepository` (legacy, uses `PartnerOnboarding` model)
- `PartnerOnboardingController` + `PartnerIntegrationController` + `IPartnerOnboardingNewRepository` (new, uses `PartnerOnboardingNew`/`PartnerIntegrationNew`)

Both are wired up in DI and have active routes. The "new" system separated onboarding (partner registration) from integration (API case creation) into distinct controllers.

### HomeController is registered as Transient
`HomeController` is registered as `AddTransient` (not `AddScoped` like other controllers) because `JIRACreatorController` directly depends on it to call `Table_Export_IntegratedXL()` for generating Excel documents uploaded to JIRA.

### CommonController uses a different namespace
`CommonController` is in namespace `OneViewIndicator.Controllers` (not `API_HUNT.Controllers`) and references a static `Startup.connectionstring` — this is legacy code from before the DI refactor.

### CaseId vs CaseID
The model `PartnerIntegrationNew` has **both** `CaseId` and `CaseID` properties. These are used inconsistently across the codebase. Similarly, `status` vs `Status` appear as separate properties.

### Encryption
- `EncryptDecrypt` class handles password encryption for login
- `AppED.Crypto.AesBase64Wrapper` handles SSO token encryption/decryption
- `AESEncrytDecry` model file exists but is empty

### Email
`SendEmail` class exists with method signatures but all methods return `false` or are empty — email functionality is stubbed out.

---

## Project Structure Reference

```
Hunt/
├── Controllers/
│   ├── LoginController.cs              # Auth, SSO, logout
│   ├── HomeController.cs               # Dashboard, diagnostics, integration workflow (HUGE)
│   ├── AdminController.cs              # User/App master CRUD
│   ├── PartnerOnboardingController.cs  # New: partner registration
│   ├── PartnerIntegrationController.cs # New: integration case management
│   ├── PartnerApprovalController.cs    # New: multi-dept approval workflow
│   ├── Partner_IntegrationController.cs# Legacy: combined partner/integration
│   ├── PartnerOffboardingController.cs # Partner deactivation
│   ├── PartnerDashboardController.cs   # Analytics (view-only, no logic)
│   ├── ExceptionManagementController.cs# Exception case CRUD
│   ├── ExceptionApprovalController.cs  # Exception approval view
│   ├── JIRACreaterController.cs        # JIRA ticket creation
│   └── CommonController.cs            # Legacy: session expiry partial
│
├── Models/
│   ├── DataBaseConnection.cs     # Connection string factory
│   ├── DbConnectionFactory.cs    # IDbConnectionFactory implementation
│   ├── IDbConnectionFactory.cs   # Connection factory interface
│   ├── CustomFilter.cs           # Session-based auth filter
│   ├── LoginModel.cs             # Login form model
│   ├── LoginUserRecord.cs        # UserMaster query result
│   ├── LoginRepository.cs        # Login data access
│   ├── ILoginRepository.cs       # Login repository interface
│   ├── AdminMaster.cs            # Admin models (App/User master)
│   ├── NewIntegration.cs         # Integration workflow models (large)
│   ├── PartnerOnboardingNew.cs   # New partner models
│   ├── PartnerIntegrationNew.cs  # New integration models
│   ├── PartnerApprovalNew.cs     # New approval models
│   ├── PartnerOnboarding.cs      # Legacy partner models
│   ├── ExceptionLevels.cs        # Exception management models
│   ├── AddEditExceptionModel.cs  # Exception form model
│   ├── JiraIssueModel.cs         # JIRA payload models
│   ├── JIRAResponseModel.cs      # JIRA API response model
│   ├── SendEmail.cs              # Email (stubbed)
│   ├── AESEncrytDecry.cs         # Encryption (empty)
│   ├── AppEDCrypto.cs            # AES encryption for SSO
│   ├── ActivityLogRepository.cs  # Audit logging
│   ├── PartnerOnboardingNewRepository.cs # New partner data access
│   ├── PartnerRepository.cs      # Legacy partner data access
│   ├── PartnerApprovalRepository.cs
│   ├── PartnerOffboardingRepository.cs
│   ├── ExceptionRepository.cs
│   ├── SubmitRepository.cs       # Integration workflow data access
│   ├── JIRARepository.cs         # JIRA payload builders
│   └── ... (interfaces: I*.cs)
│
├── Repositories/
│   ├── Interfaces/IAdminRepository.cs
│   └── Implementations/AdminRepository.cs
│
├── Views/
│   ├── Login/Index.cshtml
│   ├── Home/ (Index, DiagnoseIssue, search, newintegration, etc.)
│   ├── Admin/ (Index, Admins)
│   ├── PartnerOnboarding/ (Add, Edit, List, View)
│   ├── PartnerIntegration/ (Add, Edit, List, View)
│   ├── PartnerApproval/ (ListofPartner, ApprovedPartner)
│   ├── Partner_Integration/ (legacy views)
│   ├── PartnerOffboarding/
│   ├── PartnerDashboard/
│   ├── ExceptionManagement/ (multiple level views)
│   ├── ExceptionApproval/
│   └── Shared/ (_Layout, _SessionExpiry, Error)
│
├── wwwroot/
│   ├── css/, js/, fonts/, images/
│   ├── UploadPO/       # Legacy partner docs
│   ├── UploadPONew/    # New partner docs
│   └── UploadPINew/    # Integration docs
│
├── Program.cs           # App startup, DI registration
├── startupclass.cs      # Legacy Startup.cs (reference only)
└── Collections/postman_collection.json
```

---

## Risk & Compliance Context

This application exists to enforce **API governance** in what appears to be a **banking/financial services organization**. Key indicators:

- TPRM (Third-Party Risk Management) assessment is mandatory for partners
- Risk scoring at both partner and API level
- Multi-level, multi-department approval chains reflect regulatory requirements
- Exception management with escalation up to "APEX Steering Committee"
- Departments like CISO, ISG, ITDRM, Compliance indicate financial regulatory compliance
- ITGRC (IT Governance, Risk, and Compliance) codes are tracked for applications
- All user actions are audit-logged

---

## External Systems

| System | Endpoint | Purpose |
|--------|----------|---------|
| Active Directory | `LDAP://ldap.hunt.com` | User authentication |
| JIRA | `https://jira.hunt.com/rest/api/2/` | Ticket creation for approved integrations |
| Mofee Portal | URL from `tbl_Mofee_Url` | Parent application portal (redirects on logout) |
| Monitoring URLs | From database | OBP/SOA/APIGW health dashboards (UAT and Live) |
