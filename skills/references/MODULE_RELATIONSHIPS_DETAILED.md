# HUNT System - Module Relationships & Feature Matrix

## SECTION 1: MODULE INTERACTION MATRIX

### 1.1 Direct Module Dependencies Table

```
┌─────────────────────────────────────────────────────────────────────────────────────────────────────────┐
│ MODULE INTERACTIONS & DEPENDENCIES                                                                      │
├────────────────────────────┬──────────────┬──────────────┬──────────────┬──────────────┬──────────────┤
│ Source Module              │ Partner      │ Approval     │ Integration  │ Exception    │ Admin        │
│                            │ Onboarding   │ Workflow     │              │ Mgmt         │              │
├────────────────────────────┼──────────────┼──────────────┼──────────────┼──────────────┼──────────────┤
│ Partner Onboarding         │      -       │    CREATE    │    LINKED    │    LINKED    │    READ      │
│                            │              │    (Submit)  │   (FK)       │   (FK)       │   (UserRef)  │
├────────────────────────────┼──────────────┼──────────────┼──────────────┼──────────────┼──────────────┤
│ Partner Approval           │    READ      │      -       │    READ      │    READ      │    READ      │
│                            │    (Ref)     │              │   (Status)   │   (Ref)      │   (Approver) │
├────────────────────────────┼──────────────┼──────────────┼──────────────┼──────────────┼──────────────┤
│ Partner Integration        │    READ      │    CREATE    │      -       │    LINKED    │    READ      │
│                            │    (Ref)     │    (Submit)  │              │   (FK)       │   (UserRef)  │
├────────────────────────────┼──────────────┼──────────────┼──────────────┼──────────────┼──────────────┤
│ Partner Offboarding        │    READ      │    CREATE    │    READ      │    N/A       │    READ      │
│                            │    (Ref)     │    (Submit)  │   (Ref)      │              │   (UserRef)  │
├────────────────────────────┼──────────────┼──────────────┼──────────────┼──────────────┼──────────────┤
│ Exception Management       │    READ      │    CREATE    │    READ      │      -       │    READ      │
│                            │    (FK)      │    (Submit)  │   (FK)       │              │   (UserRef)  │
├────────────────────────────┼──────────────┼──────────────┼──────────────┼──────────────┼──────────────┤
│ JIRA Integration           │    LINK      │    CREATE    │    LINK      │    LINK      │    CONFIG    │
│                            │   (JIRA_ID)  │   (JIRA)     │   (JIRA_ID)  │  (JIRA_ID)   │  (Settings)  │
├────────────────────────────┼──────────────┼──────────────┼──────────────┼──────────────┼──────────────┤
│ Email Notifications        │    SEND      │    SEND      │    SEND      │    SEND      │    SEND      │
│                            │ (Approvers)  │   (Status)   │ (Approvers)  │ (Approvers)  │  (All Users) │
├────────────────────────────┼──────────────┼──────────────┼──────────────┼──────────────┼──────────────┤
│ Authentication/LDAP        │   VERIFY     │   VERIFY     │   VERIFY     │   VERIFY     │   VERIFY     │
│                            │   (User)     │   (User)     │   (User)     │   (User)     │   (User)     │
├────────────────────────────┼──────────────┼──────────────┼──────────────┼──────────────┼──────────────┤
│ Activity Logging           │    LOG       │    LOG       │    LOG       │    LOG       │    LOG       │
│                            │ (All actions)│ (All actions)│ (All actions)│ (All actions)│ (All actions)│
├────────────────────────────┼──────────────┼──────────────┼──────────────┼──────────────┼──────────────┤
│ File Upload/Download       │    STORE     │   RETRIEVE   │    STORE     │   RETRIEVE   │     N/A      │
│                            │  (Documents) │  (Documents) │  (Documents) │ (Documents)  │              │
└────────────────────────────┴──────────────┴──────────────┴──────────────┴──────────────┴──────────────┘

Legend:
CREATE   = Module creates data for target module
READ     = Module reads/references target module data
LINK     = Module creates link to target module
VERIFY   = Validation against target module
SEND     = Send communication to target module
LOG      = Log activity to target module
STORE    = Store files for target module
RETRIEVE = Retrieve files from target module
```

---

## SECTION 2: COMPLETE FEATURE MATRIX

### 2.1 All Features by Module

```
╔════════════════════════════════════════════════════════════════════════════════════════════════════════╗
║ FEATURE MATRIX - HUNT API MANAGEMENT SYSTEM                                                          ║
║ Status: ✓ Complete | ☐ Planned | ☐ Partial/Incomplete | ❌ Missing                                   ║
╠════════════════════════════════════════════════════════════════════════════════════════════════════════╣

┌──────────────────────────────────────────────────────────────────────────────────────────────────────┐
│ PARTNER ONBOARDING MODULE (PartnerOnboardingController)                                             │
├──────────────────────────────────────────────────────────────────────────────────────────────────────┤
│ ✓ Create Partner Case                          │ ✓ Draft Management                                  │
│ ✓ Partner Details Form                         │ ✓ API Selection & Association                       │
│ ✓ Risk Assessment (Partner & API)              │ ✓ Document Upload (3 types)                         │
│ ✓ Multi-step Wizard                            │ ✓ Submit for Approval                               │
│ ✓ Status Tracking (6 statuses)                 │ ✓ Partner Type Master                               │
│ ✓ Tentative Go-Live Date                       │ ✓ TPRM Assessment Flag                              │
│ ✓ Approval Chain Definition                    │ ☐ Partner Lookup/Search                             │
│ ☐ Bulk Partner Import                          │ ☐ Partner Profile View                              │
│ ☐ Partner History View                         │ ☐ Duplicate Detection                               │
└──────────────────────────────────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────────────────────────────────┐
│ PARTNER APPROVAL WORKFLOW MODULE (PartnerApprovalController)                                        │
├──────────────────────────────────────────────────────────────────────────────────────────────────────┤
│ ✓ View Pending Approvals                       │ ✓ Filter by Department                              │
│ ✓ Filter by Status                             │ ✓ View Partner Details                              │
│ ✓ View Approval Matrix                         │ ✓ Download Documents                                │
│ ✓ Approve Action                               │ ✓ Reject Action                                     │
│ ✓ Provide Feedback                             │ ✓ View Approval Trail                               │
│ ✓ Department-level Routing                     │ ✓ Hierarchy-level Approval (FH→VH→GH)              │
│ ✓ Feedback Reply History                       │ ✓ Auto-route to Next Level                          │
│ ☐ Approval SLA Tracking                        │ ☐ Escalation on Timeout                             │
│ ☐ Bulk Approval                                │ ☐ Approval Delegation                               │
│ ☐ Conditional Routing Rules                    │ ❌ Approval Rollback                                │
└──────────────────────────────────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────────────────────────────────┐
│ PARTNER INTEGRATION MODULE (PartnerIntegrationController)                                           │
├──────────────────────────────────────────────────────────────────────────────────────────────────────┤
│ ✓ Integration Case Creation                    │ ✓ Project Management Details                        │
│ ✓ Service Selection                            │ ✓ Service Questionnaire                             │
│ ✓ Consumer/Producer Application                │ ✓ Document Management                               │
│ ✓ REST/SOAP Selection                          │ ✓ Volume & SLA Definition                           │
│ ✓ Approval Workflow                            │ ✓ API Gateway Request Flag                          │
│ ✓ Status Tracking                              │ ✓ Platform Assignment                               │
│ ✓ Multi-Service Integration                    │ ☐ Service Template Library                          │
│ ☐ Integration History                          │ ☐ Service Reusability                               │
│ ☐ Integration Performance Metrics              │ ❌ Test Data Management                             │
└──────────────────────────────────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────────────────────────────────┐
│ PARTNER OFFBOARDING MODULE (PartnerOffboardingController)                                           │
├──────────────────────────────────────────────────────────────────────────────────────────────────────┤
│ ✓ Partner Selection                            │ ✓ API Selection for Decommission                    │
│ ✓ Exit Scenario Definition                     │ ✓ Partner Checklist                                 │
│ ✓ Document Upload                              │ ✓ API Status Change (Active→Inactive)               │
│ ✓ Approval Workflow                            │ ✓ Offboarding Status Tracking                       │
│ ☐ Offboarding Timeline                         │ ☐ Data Migration/Archive                            │
│ ☐ Offboarding Checklist Validation             │ ❌ Automatic Data Purge                             │
└──────────────────────────────────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────────────────────────────────┐
│ EXCEPTION MANAGEMENT MODULE (ExceptionManagementController + ExceptionApprovalController)           │
├──────────────────────────────────────────────────────────────────────────────────────────────────────┤
│ ✓ Create Exception Request                     │ ✓ Exception Level Selection (1-3)                   │
│ ✓ Exception Details Entry                      │ ✓ Impact Assessment                                 │
│ ✓ Start/End Date Definition                    │ ✓ Multi-Level Approval                              │
│ ✓ Exception Status Tracking                    │ ✓ Level-wise Approval Routing                       │
│ ✓ Audit Trail                                  │ ✓ Approval Feedback                                 │
│ ✓ Related Onboarding Reference                 │ ✓ Exception History View                            │
│ ☐ Exception Template Library                   │ ☐ Exception Impact Analytics                        │
│ ☐ Exception Metrics Dashboard                  │ ❌ Automatic Expiry (End Date)                     │
│ ❌ Exception Duration Extension                │ ❌ Exception Approval Timeline                      │
└──────────────────────────────────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────────────────────────────────┐
│ AUTHENTICATION & AUTHORIZATION MODULE (LoginController)                                             │
├──────────────────────────────────────────────────────────────────────────────────────────────────────┤
│ ✓ LDAP Authentication                          │ ✓ User Role Assignment (USER/APPROVER/ADMIN)        │
│ ✓ User Master Lookup                           │ ✓ Active/Inactive User Check                        │
│ ✓ Session Initialization                       │ ✓ Concurrent Login Control                          │
│ ✓ Login Attempt Tracking                       │ ✓ Last Login Date Update                            │
│ ✓ Role-based Access Control                    │ ✓ Department Association                            │
│ ☐ Session Timeout Configuration                │ ☐ Password Policy Enforcement                       │
│ ☐ Failed Login Attempt Limit                   │ ❌ Multi-Factor Authentication                      │
│ ❌ OAuth/SSO Integration                       │ ❌ Session Token (JWT)                              │
│ ❌ Password Reset Self-Service                 │ ❌ Account Lockout Mechanism                        │
└──────────────────────────────────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────────────────────────────────┐
│ DASHBOARD & REPORTING MODULE (PartnerDashboardController)                                           │
├──────────────────────────────────────────────────────────────────────────────────────────────────────┤
│ ✓ Partner Summary Count                        │ ✓ Status Breakdown (Created/Draft/InProgress)       │
│ ✓ Approval Status Charts                       │ ✓ Case Status Summary                               │
│ ✓ Partner List Export (Excel)                  │ ✓ Active Partners Grid                              │
│ ✓ Approval Trail View                          │ ✓ Filterable Partner Table                          │
│ ☐ Approval Turnaround Time Metric              │ ☐ Partner Onboarding Timeline Chart                 │
│ ☐ Approval SLA Dashboard                       │ ☐ Exception Rate Analysis                           │
│ ☐ Top Approvers Performance                    │ ☐ API Adoption Rate                                 │
│ ❌ Real-time Dashboard Refresh                 │ ❌ Custom Report Builder                            │
│ ❌ Scheduled Report Distribution               │ ❌ Predictive Analytics                             │
└──────────────────────────────────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────────────────────────────────┐
│ ADMIN MODULE (AdminController)                                                                     │
├──────────────────────────────────────────────────────────────────────────────────────────────────────┤
│ ✓ User Master Management (CRUD)                │ ✓ Role Assignment per User                          │
│ ✓ User Activation/Deactivation                 │ ✓ Application Master Maintenance                    │
│ ✓ SPOC Management                              │ ✓ Audit Log View                                    │
│ ✓ User Search & Filter                         │ ☐ Approval Matrix Configuration                     │
│ ☐ Batch User Import                            │ ☐ System Parameters Configuration                   │
│ ☐ Email Template Management                    │ ☐ Audit Log Export                                  │
│ ☐ Security Policy Configuration                │ ☐ Backup & Recovery Management                      │
│ ❌ Role-based Permission Builder               │ ❌ License/Account Management                       │
│ ❌ Usage Analytics & Reporting                 │ ❌ Integration Configuration UI                     │
└──────────────────────────────────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────────────────────────────────┐
│ JIRA INTEGRATION MODULE (JIRACreatorController)                                                     │
├──────────────────────────────────────────────────────────────────────────────────────────────────────┤
│ ✓ JIRA Issue Creation                          │ ✓ Issue Type Selection                              │
│ ✓ Issue Basic Authentication                   │ ✓ Issue Linking to Case                             │
│ ✓ HTTP Client Integration                      │ ✓ REST API v2 Usage                                 │
│ ☐ JIRA Status Sync (Bi-directional)            │ ☐ Comment/Update Propagation                       │
│ ☐ JIRA Workflow Integration                    │ ☐ Custom Field Mapping                              │
│ ☐ JIRA Project Selection UI                    │ ❌ Automated Issue Closure                          │
│ ❌ JIRA Epic/Story Hierarchy                   │ ❌ JIRA Agile Integration                           │
│ ❌ Time Tracking Integration                   │ ❌ Webhook Support for Real-time Updates            │
└──────────────────────────────────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────────────────────────────────┐
│ SUPPORTING FEATURES (Cross-module)                                                                 │
├──────────────────────────────────────────────────────────────────────────────────────────────────────┤
│ ✓ Email Notifications                          │ ✓ File Upload/Download                              │
│ ✓ Activity Logging                             │ ✓ Session Management                                │
│ ✓ Encryption (AES)                             │ ✓ Excel Export                                      │
│ ✓ Error Handling (Try-Catch)                   │ ✓ HTTP Request/Response                             │
│ ✓ Date/Time Handling                           │ ✓ User Context Tracking                             │
│ ✓ Status Code Management                       │ ✓ Lookup/Master Data                                │
│ ☐ Centralized Logging (Serilog)                │ ☐ API Rate Limiting                                 │
│ ☐ Distributed Caching                          │ ☐ Request Validation Framework                      │
│ ☐ Scheduled Jobs/Background Tasks              │ ☐ Health Check Endpoint                             │
│ ❌ Audit Event Publishing                      │ ❌ Message Queue Integration                        │
└──────────────────────────────────────────────────────────────────────────────────────────────────────┘

╚════════════════════════════════════════════════════════════════════════════════════════════════════════╝
```

---

## SECTION 3: DATA RELATIONSHIPS RAW FORMAT

### 3.1 Table Relationship Definitions

```
┌─────────────────────────────────────────────────────────────────────────┐
│ RELATIONSHIP DEFINITIONS (SQL DDL Format)                              │
├─────────────────────────────────────────────────────────────────────────┤

FOREIGN KEY RELATIONSHIPS (RECOMMENDED TO ADD):

1. PARTNER ONBOARDING RELATIONSHIPS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛

ALTER TABLE [tbl_API_HUNT_PartnerCaseServiceList]
ADD CONSTRAINT FK_PartnerCase_ServiceList
FOREIGN KEY (CaseID)
REFERENCES [tbl_API_HUNT_Partner_Onboarding] ([PartnetOnboading_ID])
ON DELETE CASCADE
ON UPDATE CASCADE;

ALTER TABLE [tbl_API_HUNT_PO_ApiDetail]
ADD CONSTRAINT FK_PartnerCase_APIDetails
FOREIGN KEY ([CaseId])
REFERENCES [tbl_API_HUNT_Partner_Onboarding] ([PartnetOnboading_ID])
ON DELETE CASCADE
ON UPDATE CASCADE;

ALTER TABLE [tbl_API_HUNT_PO_ApprovalTrailTable]
ADD CONSTRAINT FK_PartnerCase_ApprovalTrail
FOREIGN KEY ([CaseId])
REFERENCES [tbl_API_HUNT_Partner_Onboarding] ([PartnetOnboading_ID])
ON DELETE CASCADE
ON UPDATE CASCADE;

ALTER TABLE [tbl_API_HUNT_POFeedbackReply_history]
ADD CONSTRAINT FK_PartnerCase_Feedback
FOREIGN KEY ([CaseID])
REFERENCES [tbl_API_HUNT_Partner_Onboarding] ([PartnetOnboading_ID])
ON DELETE CASCADE
ON UPDATE CASCADE;

ALTER TABLE [tbl_API_HUNT_POFeedbackReply_history]
ADD CONSTRAINT FK_ApprovalTrail_Feedback
FOREIGN KEY ([ApprovalId])
REFERENCES [tbl_API_HUNT_PO_ApprovalTrailTable] ([Id])
ON DELETE CASCADE
ON UPDATE CASCADE;

2. EXCEPTION MANAGEMENT RELATIONSHIPS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

ALTER TABLE [tbl_API_HUNT_ExceptionManagement]
ADD CONSTRAINT FK_ExceptionMgmt_PartnerCase
FOREIGN KEY ([OriginalOnboardingGASID])
REFERENCES [tbl_API_HUNT_Partner_Onboarding] ([PartnetOnboading_ID])
ON DELETE SET NULL
ON UPDATE CASCADE;

ALTER TABLE [tbl_API_HUNT_Audit_log]
ADD CONSTRAINT FK_ExceptionAudit_ExceptionCase
FOREIGN KEY ([CaseID])
REFERENCES [tbl_API_HUNT_ExceptionManagement] ([ID])
ON DELETE CASCADE
ON UPDATE CASCADE;

3. INTEGRATION RELATIONSHIPS
━━━━━━━━━━━━━━━━━━━━━━━━━━

ALTER TABLE [tbl_API_HUNT_ServiceDetails]
ADD CONSTRAINT FK_Integration_Services
FOREIGN KEY ([IntegrationId])
REFERENCES [tbl_API_HUNT_Integration] ([IntegrationId])
ON DELETE CASCADE
ON UPDATE CASCADE;

ALTER TABLE [tbl_API_HUNT_QusServiceDetails]
ADD CONSTRAINT FK_Service_Questions
FOREIGN KEY ([ServiceID])
REFERENCES [tbl_API_HUNT_ServiceDetails] ([ServiceID])
ON DELETE CASCADE
ON UPDATE CASCADE;

4. USER & AUTHENTICATION RELATIONSHIPS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

ALTER TABLE [tbl_API_HUNT_USER]
ADD CONSTRAINT FK_User_Profile
FOREIGN KEY ([Role])
REFERENCES [ProfileMaster] ([ProfileName])
ON DELETE RESTRICT
ON UPDATE CASCADE;

5. API MASTER RELATIONSHIPS
━━━━━━━━━━━━━━━━━━━━━━━━━

ALTER TABLE [tbl_API_ApplicationsSPOC]
ADD CONSTRAINT FK_Application_SPOC
FOREIGN KEY ([APPID])
REFERENCES [tbl_API_ApplicationsSPOC] ([APPID])
ON DELETE RESTRICT
ON UPDATE CASCADE;

```

### 3.2 Relational Model (Entity-Relationship Format)

```
HUNT RELATIONAL MODEL - NORMALIZED STRUCTURE

┌────────────────────────────────────────────────────────────────────────────┐
│ CORE ENTITIES                                                              │
├────────────────────────────────────────────────────────────────────────────┤

ENTITY: Partner_Case
┌─────────────────────────────────────────────────────┐
│ Attributes:                                         │
│  PK: CaseID (INT, AUTO_INCREMENT)                   │
│  PartnerName (VARCHAR 50)                           │
│  PartnerType_ID (INT, FK → PartnerType)             │
│  PartnerEntityType (VARCHAR 100)                    │
│  PartnerRiskLevel_ID (INT, FK → RiskLevel)          │
│  TentativeGoLiveDate (DATE)                         │
│  StatusCode (INT) [1-20]                            │
│  CreatedBy_ID (INT, FK → User)                      │
│  CreatedDate (DATETIME)                             │
│  UpdatedBy_ID (INT, FK → User)                      │
│  UpdatedDate (DATETIME)                             │
├─────────────────────────────────────────────────────┤
│ Relationships:                                      │
│  1:N → API_List                                     │
│  1:N → Approval_Trail                               │
│  1:N → Feedback_History                             │
│  1:N → Uploaded_Documents                           │
│  N:1 ← Exception_Management (optional)              │
│  N:1 → User (created_by)                            │
│  N:1 → PartnerType (lookup)                         │
│  N:1 → RiskLevel (lookup)                           │
└─────────────────────────────────────────────────────┘

ENTITY: API_List
┌─────────────────────────────────────────────────────┐
│ Attributes:                                         │
│  PK: APIListID (INT)                                │
│  FK: CaseID (INT) → Partner_Case.CaseID             │
│  APIName (VARCHAR 100)                              │
│  APIRiskLevel_ID (INT, FK → RiskLevel)              │
│  APIStatus (VARCHAR 20) [ACTIVE/INACTIVE/DEPRECATED]│
│  CreatedDate (DATETIME)                             │
│  UpdatedDate (DATETIME)                             │
├─────────────────────────────────────────────────────┤
│ Relationships:                                      │
│  N:1 → Partner_Case                                 │
│  N:1 → RiskLevel                                    │
│  1:N → Service_Details (in Integration)             │
└─────────────────────────────────────────────────────┘

ENTITY: Approval_Trail
┌─────────────────────────────────────────────────────┐
│ Attributes:                                         │
│  PK: ApprovalTrailID (INT)                          │
│  FK: CaseID (INT) → Partner_Case.CaseID             │
│  Department (VARCHAR 100) [HOPP/HOB/etc]            │
│  ApproverLevel (VARCHAR 20) [FH/VH/GH]              │
│  FK: ApproverUserID (INT) → User.UserID             │
│  Sequence (INT) [1,2,3]                             │
│  Status (VARCHAR 20) [PENDING/APPROVED/REJECTED]    │
│  ApprovedDate (DATETIME)                            │
│  Comments (VARCHAR MAX)                             │
├─────────────────────────────────────────────────────┤
│ Relationships:                                      │
│  N:1 → Partner_Case                                 │
│  N:1 → User (approver)                              │
│  1:N → Feedback_History                             │
└─────────────────────────────────────────────────────┘

ENTITY: Feedback_History
┌─────────────────────────────────────────────────────┐
│ Attributes:                                         │
│  PK: FeedbackID (INT)                               │
│  FK: CaseID (INT) → Partner_Case.CaseID             │
│  FK: ApprovalTrailID (INT) → Approval_Trail        │
│  FK: FromUserID (INT) → User (approver)             │
│  FeedbackText (VARCHAR MAX)                         │
│  FeedbackDate (DATETIME)                            │
│  Status (VARCHAR 20) [PENDING/RESOLVED]             │
│  ReplyText (VARCHAR MAX)                            │
│  FK: ReplyByUserID (INT) → User (initiator)         │
│  ReplyDate (DATETIME)                               │
├─────────────────────────────────────────────────────┤
│ Relationships:                                      │
│  N:1 → Partner_Case                                 │
│  N:1 → Approval_Trail                               │
│  N:1 → User (from)                                  │
│  N:1 → User (reply_by)                              │
└─────────────────────────────────────────────────────┘

ENTITY: Exception_Management
┌─────────────────────────────────────────────────────┐
│ Attributes:                                         │
│  PK: ExceptionID (INT)                              │
│  ExceptionCode (VARCHAR 50)                         │
│  FK: PartnerCaseID (INT) → Partner_Case.CaseID      │
│  ExceptionReason (VARCHAR MAX)                      │
│  ExceptionLevel (INT) [1/2/3]                       │
│  StartDate (DATE)                                   │
│  EndDate (DATE)                                     │
│  ImpactScope (VARCHAR 100)                          │
│  Status (VARCHAR 20)                                │
│  FK: RequestedBy_ID (INT) → User                    │
│  CreatedDate (DATETIME)                             │
│  ApprovedDate (DATETIME)                            │
├─────────────────────────────────────────────────────┤
│ Relationships:                                      │
│  N:1 → Partner_Case                                 │
│  N:1 → User (requested_by)                          │
│  1:N → Exception_Approval_Trail                     │
│  1:N → Audit_Log                                    │
└─────────────────────────────────────────────────────┘

ENTITY: Integration_Case
┌─────────────────────────────────────────────────────┐
│ Attributes:                                         │
│  PK: IntegrationID (INT)                            │
│  ProjectName (VARCHAR 100)                          │
│  ProjectID (VARCHAR 100)                            │
│  PlannedGoLiveDate (DATETIME)                       │
│  BusinessJustification (VARCHAR MAX)                │
│  FK: ConsumerAppID (INT) → Application.AppID        │
│  FK: ProducerAppID (INT) → Application.AppID        │
│  Status (VARCHAR 20)                                │
│  FK: CreatedBy_ID (INT) → User                      │
│  CreatedDate (DATETIME)                             │
├─────────────────────────────────────────────────────┤
│ Relationships:                                      │
│  1:N → Service_Details                              │
│  N:1 → User (created_by)                            │
│  N:1 → Application (consumer)                       │
│  N:1 → Application (producer)                       │
│  1:N → Approval_Trail                               │
└─────────────────────────────────────────────────────┘

ENTITY: Service_Details
┌─────────────────────────────────────────────────────┐
│ Attributes:                                         │
│  PK: ServiceID (INT)                                │
│  FK: IntegrationID (INT) → Integration_Case        │
│  ServiceName (VARCHAR 5000)                         │
│  ServiceType (VARCHAR 50) [REST/SOAP/gRPC]          │
│  Purpose (VARCHAR 100)                              │
│  ConsumerAppID (INT)                                │
│  ProducerAppID (INT)                                │
│  ExpectedVolume (INT)                               │
│  RequiresAPIGW (BIT)                                │
│  Status (VARCHAR 20)                                │
│  CreatedDate (DATETIME)                             │
├─────────────────────────────────────────────────────┤
│ Relationships:                                      │
│  N:1 → Integration_Case                             │
│  1:N → Service_Questions                            │
│  N:1 → API_Master (if linked)                       │
└─────────────────────────────────────────────────────┘

ENTITY: User
┌─────────────────────────────────────────────────────┐
│ Attributes:                                         │
│  PK: UserID (INT)                                   │
│  EmpCode (VARCHAR 25) [LDAP username]               │
│  FullName (VARCHAR 100)                             │
│  EmailID (VARCHAR 150)                              │
│  Role_ID (INT, FK → Role) [USER/APPROVER/ADMIN]    │
│  Department (VARCHAR 50)                            │
│  IsActive (BIT)                                     │
│  LastLoginDate (DATETIME)                           │
│  CreatedDate (DATETIME)                             │
├─────────────────────────────────────────────────────┤
│ Relationships:                                      │
│  1:N → Partner_Case (created_by)                    │
│  1:N → Approval_Trail (as approver)                 │
│  1:N → Feedback_History (as responder)              │
│  1:N → Activity_Log (as actor)                      │
│  N:1 → Role                                         │
└─────────────────────────────────────────────────────┘

ENTITY: Role
┌─────────────────────────────────────────────────────┐
│ Attributes:                                         │
│  PK: RoleID (INT)                                   │
│  RoleName (VARCHAR 50) [USER/APPROVER/ADMIN]        │
│  RoleDescription (VARCHAR 500)                      │
│  CreatedDate (DATETIME)                             │
│  IsActive (BIT)                                     │
├─────────────────────────────────────────────────────┤
│ Relationships:                                      │
│  1:N → User                                         │
│  1:N → Role_Permission                              │
└─────────────────────────────────────────────────────┘
```

---

## SECTION 4: CROSS-CUTTING CONCERNS & SHARED SERVICES

### 4.1 Shared Service Dependencies

```
┌───────────────────────────────────────────────────────────────────────┐
│ SHARED SERVICES MATRIX                                                │
├───────┬──────────┬──────────┬──────────┬──────────┬──────────┬────────┤
│ Service      │Onboarding│Approval  │Exception │Admin     │JIRA    │ Dashboard│
├───────┬──────┼──────────┼──────────┼──────────┼──────────┼────────┤
│ Email        │ SEND     │ SEND     │ SEND     │ SEND     │ SEND   │ NONE   │
│ Notification │ Approvers│To Status │To Status │To Users  │Updates │        │
├───────┼──────┼──────────┼──────────┼──────────┼──────────┼────────┤
│ Activity     │ LOG      │ LOG      │ LOG      │ LOG      │ LOG    │ READ   │
│ Logging      │ Case Ops │Approval  │Exception │User Ops  │Linking │ History│
├───────┼──────┼──────────┼──────────┼──────────┼──────────┼────────┤
│ LDAP Auth    │ VERIFY   │ VERIFY   │ VERIFY   │ VERIFY   │        │ VERIFY │
│              │ User     │ Approver │User      │ Admin    │        │ User   │
├───────┼──────┼──────────┼──────────┼──────────┼──────────┼────────┤
│ File Upload  │ STORE    │ RETRIEVE │ NONE     │ NONE     │        │ NONE   │
│ /Download    │ Docs     │ Docs     │          │          │        │        │
├───────┼──────┼──────────┼──────────┼──────────┼──────────┼────────┤
│ Encryption   │ ENCRYPT  │ NONE     │ NONE     │ ENCRYPT  │ NONE   │ NONE   │
│ (AES)        │ Password │          │          │ Data     │        │        │
├───────┼──────┼──────────┼──────────┼──────────┼──────────┼────────┤
│ Excel Export │ EXPORT   │ EXPORT   │ NONE     │ EXPORT   │ NONE   │ EXPORT │
│              │ Cases    │ Approvals│          │ Users    │        │ Report │
├───────┼──────┼──────────┼──────────┼──────────┼──────────┼────────┤
│ Session Mgmt │ CREATE   │ VERIFY   │ VERIFY   │ VERIFY   │ VERIFY │ VERIFY │
│              │ Session  │ Session  │ Session  │ Session  │Session │ Session│
└───────┴──────┴──────────┴──────────┴──────────┴──────────┴────────┘
```

### 4.2 Common Utility Classes

```
┌────────────────────────────────────────────────────┐
│ UTILITY CLASSES (Cross-Cutting Concerns)           │
├────────────────────────────────────────────────────┤
│ SendEmail                                          │
│  ├─ EmailTo: Recipient email address               │
│  ├─ Subject: Email subject                         │
│  ├─ Body: Email body (HTML)                        │
│  ├─ Attachments: File paths                        │
│  ├─ Method: Send() → bool Success/Failure          │
│  └─ Logger: Not implemented                        │
│                                                    │
│ AESEncryptDecrypt                                  │
│  ├─ Encrypt(plaintext) → ciphertext                │
│  ├─ Decrypt(ciphertext) → plaintext                │
│  ├─ Key: Hardcoded (SECURITY RISK)                │
│  └─ Method: AES-256-CBC (assumed)                  │
│                                                    │
│ CustomFilter (Authentication Filter)              │
│  ├─ Purpose: Role-based access control             │
│  ├─ Implementation: ActionFilter                   │
│  └─ Check: HttpContext.Session["Role"]             │
│                                                    │
│ ResponseContent (API Response Wrapper)             │
│  ├─ isSuccess: bool                                │
│  ├─ Data: object                                   │
│  ├─ Message: string                                │
│  └─ Url: string (for redirects)                    │
│                                                    │
│ GetConnectionString (Database Connection)         │
│  ├─ Static Property: Startup.connectionstring      │
│  ├─ Type: SqlConnection                            │
│  └─ Source: Not visible in codebase                │
│                                                    │
│ Activity Logging (Manual Implementation)           │
│  ├─ Table: tbl_API_HUNT_Activity_Log_Tracker       │
│  ├─ Log Data:                                      │
│  │  ├─ EmpCode                                     │
│  │  ├─ FormName                                    │
│  │  ├─ ModuleName                                  │
│  │  ├─ Activity (CREATE/UPDATE/APPROVE)            │
│  │  ├─ ActivityDate (timestamp)                    │
│  │  └─ ActivityDetails                             │
│  └─ No Centralized Logger (SeriLog)                │
│                                                    │
│ Master Data Lookup Utility (Implicit)              │
│  ├─ Partner Types (Master query)                   │
│  ├─ Risk Levels (1-5 scale)                        │
│  ├─ Department Codes (HOPP, etc.)                  │
│  ├─ Approver Levels (FH, VH, GH)                   │
│  └─ Status Codes (1-20)                            │
└────────────────────────────────────────────────────┘
```

---

## SECTION 5: WORKFLOW STATE DIAGRAMS

### 5.1 Partner Onboarding State Machine

```
┌─────────────────────────────────────────────────────────────────────┐
│ PARTNER ONBOARDING STATE MACHINE                                    │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│                         ┌──────────────┐                           │
│                         │   CREATED    │                           │
│                         │  Status: 1   │                           │
│                         └────────┬─────┘                           │
│                                  │ User clicks "Save Draft"        │
│                                  ▼                                 │
│                         ┌──────────────┐                           │
│                         │   DRAFTED    │                           │
│                         │  Status: 2   │                           │
│                         └────────┬─────┘                           │
│                      ┌───────────┴───────────┐                     │
│                      │ User clicks "Submit"  │                     │
│                      ▼                       ▼                     │
│           ┌───────────────────┐  ┌──────────────────┐             │
│           │   IN PROGRESS     │  │   AWAITING LINK  │             │
│           │   Status: 3       │  │   (DRAFT)        │             │
│           └─────┬─────────┬───┘  └──────────────────┘             │
│                 │         │ Routed to           Edited by user    │
│                 │         │ Approvers            (resubmit)       │
│  ┌──────────────┤         │                                       │
│  │              │         │                                       │
│  │              │         └─────────┬──────────┐                  │
│  │              │                   │          │                  │
│  │              │   (FH Approves)   │          │                  │
│  │              │        │          │          │                  │
│  │              │        ▼          │          │                  │
│  │              │  ┌──────────────┐ │          │                  │
│  │              │  │              │ │          │                  │
│  │Resubmit   ┌──────────────┐     │          │                  │
│  │After      │              │     │  ┌───────▼─────────┐         │
│  │Feedback   │  (VH Approves)?   │  │                 │         │
│  │           │              │     │  │   AWAITING FOR  │         │
│  │           └──────────────┘     │  │    REPLY        │         │
│  │              │     │            │  │                │         │
│  │              │     │ Yes        │  │   Status: 10    │         │
│  │              │     ▼            │  └─────────────────┘         │
│  │              │  ┌──────────────┐│      ▲                      │
│  │              │  │              ││      │                      │
│  │              │  │  (GH Approves)       │                      │
│  │              │  │              ││      │ Approver              │
│  │              │  └──────────────┘│   provides feedback          │
│  │              │              │    │      │                      │
│  │              │              │    └──────┘                      │
│  │              │              │                                  │
│  │      ┌───────┴──────────┬──┴──────┐                            │
│  │      │                  │          │                           │
│  │      ▼                  ▼          ▼                           │
│  │  ┌─────────┐      ┌─────────┐  ┌──────────┐                   │
│  │  │APPROVED │      │REJECTED │  │ FEEDBACK │                   │
│  │  │Status:15│      │Status:20│  │ Required │                   │
│  │  └─────────┘      └─────────┘  └──────────┘                   │
│  │      │                  │            │                        │
│  │      └──────────────────┴────────────┘                        │
│  │                         │                                      │
│  └─────────────────────────┘                                     │
│                                                                   │
│  ACTIONS AT EACH STATE:                                          │
│  ════════════════════════════════════════════════════             │
│  CREATED:    Add partner details, APIs, upload docs              │
│  DRAFTED:    Edit details, upload docs, submit for approval      │
│  IN PROGRESS: (Read-only during multi-level approval)            │
│  AWAITING FOR REPLY: Respond to feedback, resubmit              │
│  APPROVED:   Partner activated, JIRA ticket created              │
│  REJECTED:   (Read-only) case closed                             │
│                                                                   │
└─────────────────────────────────────────────────────────────────────┘
```

### 5.2 Exception Management Approval State Machine

```
┌─────────────────────────────────────────────────────────────────┐
│ EXCEPTION MANAGEMENT APPROVAL FLOW                              │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│          ┌──────────────────────────┐                           │
│          │  EXCEPTION CREATED       │                           │
│          │  Status: SUBMITTED       │                           │
│          │  ExceptionLevel: 1,2,3   │                           │
│          └────────────┬─────────────┘                           │
│                       │ Route based on Level                    │
│                       ▼                                         │
│  ┌────────────────────────────────────────────┐                │
│  │ LEVEL-wise ROUTING (from Master Matrix)    │                │
│  │                                            │                │
│  │ Level 1 → Routed to:                       │                │
│  │   FH (Functional Head)                     │                │
│  │                                            │                │
│  │ Level 2 → Routed to:                       │                │
│  │   FH → VH (Vertical Head)                  │                │
│  │                                            │                │
│  │ Level 3 → Routed to:                       │                │
│  │   FH → VH → GH (Global Head)               │                │
│  └────────────────────────────────────────────┘                │
│                       │                                         │
│         ┌─────────────┼─────────────┐                           │
│         │             │             │                           │
│         ▼             │             │                           │
│  ┌──────────┐         │             │                           │
│  │   FH     │         │             │                           │
│  │ APPROVES │         │             │                           │
│  │   OR     │         │             │                           │
│  │ REJECTS  │         │             │                           │
│  └────┬─────┘         │             │                           │
│       │               │             │                           │
│   ┌───┴────┐          │             │                           │
│   │         │         │             │                           │
│  YES     NO │         │             │                           │
│   │     │   │         │             │                           │
│   │     │   └─────────┼──────────────┼────┐                     │
│   │     │             │              │    │                     │
│   │     └─────────────┼──────→ REJECTED   │                     │
│   │                   │                   │                     │
│   ├─(if Level≥2)──────┼──────────────────┤                     │
│   │                   ▼                   │                     │
│   │            ┌──────────┐               │                     │
│   │            │    VH    │               │                     │
│   │            │ APPROVES │               │                     │
│   │            │   OR     │               │                     │
│   │            │ REJECTS  │               │                     │
│   │            └─────┬────┘               │                     │
│   │                  │                    │                     │
│   │              ┌───┴────┐               │                     │
│   │              │         │              │                     │
│   │            YES       NO│              │                     │
│   │              │         │              │                     │
│   │              │         └──→ REJECTED  │                     │
│   │              │                        │                     │
│   │     ┌────────┴─(if Level=3)──────┐   │                     │
│   │     │                            │   │                     │
│   │     ▼                            ▼   │                     │
│   │  ┌──────────┐                    │   │                     │
│   │  │    GH    │                    │   │                     │
│   │  │ APPROVES │                    │   │                     │
│   │  │   OR     │                    │   │                     │
│   │  │ REJECTS  │                    │   │                     │
│   │  └────┬─────┘                    │   │                     │
│   │       │                          │   │                     │
│   │   ┌───┴────┐                     │   │                     │
│   │   │         │                    │   │                     │
│   │  YES       NO │                  │   │                     │
│   │   │   │    └──→ REJECTED ←───────┴─┴─┘                     │
│   │   │   │                                                     │
│   └───┼───┼─→ APPROVED (All levels cleared)                    │
│       │   │                                                    │
│       ▼   ▼                                                    │
│  ┌──────────────┐     ┌─────────────┐                         │
│  │  APPROVED    │     │  REJECTED   │                         │
│  │  Exception   │     │  Exception  │                         │
│  │  Active      │     │  Denied     │                         │
│  │  (until      │     │  (read-only)│                         │
│  │   EndDate)   │     └─────────────┘                         │
│  │              │                                             │
│  └────┬─────────┘                                             │
│       │ (Auto on EndDate)                                      │
│       ▼                                                        │
│  ┌─────────────┐                                              │
│  │   EXPIRED   │                                              │
│  │  (Read-only)│                                              │
│  └─────────────┘                                              │
│                                                                │
└─────────────────────────────────────────────────────────────────┘
```

---

## SECTION 6: APPROVAL MATRIX ALGORITHM

### 6.1 Partner Case Approval Routing Algorithm

```
ALGORITHM: Route_Partner_Case_for_Approval(CaseID)
═══════════════════════════════════════════════════════════════════

INPUT:  CaseID (int)
        Partner_Risk_Score (string: LOW/MEDIUM/HIGH/CRITICAL)
        API_Risk_Score (string: LOW/MEDIUM/HIGH/CRITICAL)

OUTPUT: Approval_Trail entries with (Department, Level, Approver)

PROCESS:
────────

1. RETRIEVE CASE DATA
   ├─ SELECT * FROM tbl_API_HUNT_Partner_Onboarding
   │   WHERE PartnetOnboading_ID = CaseID
   ├─ Determine: Partner_Risk_Classification
   ├─ Determine: API_Risk_Classification
   └─ LOOP: For each API in case
       ├─ Get highest API_Risk_Score (max of all APIs)
       └─ Use max(API_Risk) for routing

2. LOOKUP APPROVAL MATRIX
   ├─ SELECT * FROM tbl_API_HUNT_MstPartnerCaseApprovalMetrix
   │   WHERE PartnerRiskClassification = Partner_Risk_Classification
   │     AND APIRiskClassification = API_Risk_Classification
   │
   ├─ Return set of routing rules:
   │   [
   │     {ApproverType: "HOPP", ApproverLevel: "FH", ...},
   │     {ApproverType: "HOPP", ApproverLevel: "VH", ...},
   │     {ApproverType: "HOB",  ApproverLevel: "FH", ...},
   │     {ApproverType: "HODB", ApproverLevel: "FH", ...},
   │     ...
   │   ]

3. CREATE APPROVAL TRAIL ENTRIES
   ├─ FOR EACH routing rule in sorted order
   │   ├─ Group by Department
   │   ├─ Sort by Sequence/Level (FH before VH before GH)
   │   ├─ INSERT INTO tbl_API_HUNT_PO_ApprovalTrailTable (
   │   │     CaseID = CaseID,
   │   │     Department = ApproverType,
   │   │     ApproverLevel = ApproverLevel,
   │   │     ApproverUserID = ApproverUserID (from matrix),
   │   │     Sequence = Sequential_Order,
   │   │     Status = 'PENDING',
   │   │     CreatedDate = NOW()
   │   │   )
   │   └─ Increment sequential counter

4. SEND NOTIFICATIONS
   ├─ GET all entries with Status='PENDING' AND Sequence=1
   ├─ SEND email to first-level approvers
   │   ├─ Subject: "Partner Onboarding Approval Required"
   │   ├─ Body: Partner details + link to approval
   │   └─ Recipient: Approver_Email from matrix
   └─ Logger: Log routing decision in activity log

5. UPDATE CASE STATUS
   ├─ UPDATE tbl_API_HUNT_Partner_Onboarding
   │   SET statusCode = 3 (IN_PROGRESS)
   │       Updated_date = NOW()
   │   WHERE PartnetOnboading_ID = CaseID
   └─ Return: Success/Failure

COMPLEXITY: O(n) where n = number of routing matrix entries
RESULT: Partner case routed to multi-level approvers
```

### 6.2 Exception Level Approval Routing

```
ALGORITHM: Route_Exception_for_Approval(ExceptionID)
════════════════════════════════════════════════════════════════

INPUT:  ExceptionID (int)
        ExceptionLevel (int: 1, 2, or 3)

OUTPUT: Exception approval trail with level-specific approvers

PROCESS:
────────

1. RETRIEVE EXCEPTION
   ├─ SELECT * FROM tbl_API_HUNT_ExceptionManagement
   │   WHERE ID = ExceptionID
   ├─ Extract: ExceptionLevel
   └─ If ExceptionLevel NOT IN (1,2,3) → ERROR

2. LOOKUP LEVEL-to-APPROVERS MAPPING
   ├─ SELECT * FROM tbl_API_HUNT_MstExceptionApprovalMetrix
   │   WHERE ExceptionLevel = @ExceptionLevel
   │   ORDER BY ApproverSequence ASC
   │
   ├─ Return approvers for this level:
   │   [
   │     {ApproverLevel: "FH", ApproverUserID: "U001", ...},
   │     {ApproverLevel: "VH", ApproverUserID: "U002", ...},
   │     {ApproverLevel: "GH", ApproverUserID: "U003", ...},
   │   ]

3. BUILD APPROVAL HIERARCHY
   ├─ Level 1 Exception:
   │   └─ FH approval only
   │
   ├─ Level 2 Exception:
   │   └─ FH approval → VH approval
   │
   ├─ Level 3 Exception:
   │   └─ FH approval → VH approval → GH approval

4. CREATE APPROVAL ENTRIES
   ├─ FOR EACH approver in hierarchy ORDER
   │   ├─ INSERT exception approval trail entry
   │   ├─ Status: PENDING
   │   └─ Sequence: 1, 2, 3 (based on order)

5. SEND NOTIFICATIONS
   ├─ Send email to Level 1 (first) approver only
   ├─ Next level notified only after Level 1 approval
   └─ Email: Exception details + approval link

6. STATUS UPDATE
   ├─ UPDATE ExceptionManagement.Status = 'PENDING'
   └─ Return: Approvals created successfully
```

---

## SECTION 7: MASTER DATA DEPENDENCIES

### 7.1 Master Data Lookup Tables

```
MASTER DATA HIERARCHY
═════════════════════════════════════════════════════════════════

┌─────────────────────────────────────────────────────────────────┐
│ PARTNER TYPE MASTER (tbl_API_HUNT_MSTPartnerType)                   │
├──────────────────┬──────────────────┬──────────────────────────┤
│ PartnerType      │ PartnerEntityType│ TPRMAapplicable         │
├──────────────────┼──────────────────┼──────────────────────────┤
│ Technology       │ Individual       │ YES                     │
│ Partner          │ Company          │ NO                      │
├──────────────────┼──────────────────┼──────────────────────────┤
│ Vendor           │ Individual       │ YES                     │
│                  │ Company          │ YES                     │
│                  │ Division         │ NO                      │
├──────────────────┼──────────────────┼──────────────────────────┤
│ Business Partner │ Individual       │ YES                     │
│                  │ Company          │ YES                     │
│                  │ Division         │ YES                     │
└──────────────────┴──────────────────┴──────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│ APPROVAL MATRIX (tbl_API_HUNT_MstPartnerCaseApprovalMetrix)         │
├──────────────┬────────────┬──────────┬──────────┬──────────────┤
│ Partner Risk │ API Risk   │Approver  │ Approver │ Approver     │
│ Class        │ Class      │Type      │ Level    │ User ID      │
├──────────────┼────────────┼──────────┼──────────┼──────────────┤
│ LOW          │ LOW        │ HOPP     │ FH       │ EMP001       │
│              │            │ HOB      │ FH       │ EMP002       │
├──────────────┼────────────┼──────────┼──────────┼──────────────┤
│ LOW          │ MEDIUM     │ HOPP     │ FH→VH    │ EMP001→EMP11 │
│              │            │ HOB      │ FH→VH    │ EMP002→EMP12 │
├──────────────┼────────────┼──────────┼──────────┼──────────────┤
│ MEDIUM       │ HIGH       │ HOPP     │ FH→VH→GH │ EMP001...    │
│              │            │ HOB      │ FH→VH→GH │ EMP002...    │
│              │            │ HODB     │ FH→VH→GH │ EMP003...    │
│              │            │ HOISG    │ FH→VH→GH │ EMP004...    │
│              │            │ HOITDRM  │ FH→VH→GH │ EMP005...    │
├──────────────┼────────────┼──────────┼──────────┼──────────────┤
│ HIGH         │ CRITICAL   │ ALL      │ FH→VH→GH │ ALL Approvers│
│ CRITICAL     │ CRITICAL   │ ALL      │ FH→VH→GH │ ALL Approvers│
└──────────────┴────────────┴──────────┴──────────┴──────────────┘

┌──────────────────────────────────────────────────────────────────┐
│ DEPARTMENT CODES (Reference)                                     │
├──────────────────────────────────────────────────────────────────┤
│ HOPP  = Head of Payments & Processing (Finance)                  │
│ HOB   = Head of Business (Retail/Commercial)                     │
│ HODB  = Head of Digital Banking (Digital Channel)                │
│ HOISG = Head of IT Security & Governance (Security)              │
│ HOITDRM = Head of IT Delivery & Resource Management (IT Ops)     │
└──────────────────────────────────────────────────────────────────┘
```

---

## SECTION 8: SYSTEM METRICS & DIAGNOSTICS

### 8.1 Approval Turnaround Metrics Table

```
CALCULATED METRICS (Based on Activity Log & Approval Trail)
════════════════════════════════════════════════════════════════════

Sample Data Structure:
┌──────────┬──────────────┬──────────────┬──────────────┬──────────┐
│ CaseID   │ Submitted    │ FirstApprove │ LastApprove  │ Turnaround│
│          │ Date         │ Date         │ Date         │ (Days)   │
├──────────┼──────────────┼──────────────┼──────────────┼──────────┤
│ CASE001  │ 2024-01-05   │ 2024-01-08   │ 2024-01-15   │ 10       │
│ CASE002  │ 2024-01-10   │ 2024-01-11   │ 2024-01-12   │ 2        │
│ CASE003  │ 2024-01-15   │ 2024-01-18   │ 2024-02-02   │ 18       │
│ CASE004  │ 2024-01-20   │ 2024-01-21   │ 2024-01-23   │ 3        │
│ CASE005  │ 2024-01-25   │ 2024-01-25   │ 2024-01-28   │ 3        │
└──────────┴──────────────┴──────────────┴──────────────┴──────────┘

QUERIES FOR KPI CALCULATION:

-- Average Approval Turnaround Time
SELECT AVG(DATEDIFF(day, SubmittedDate, ApprovedDate)) as Avg_Days
FROM tbl_API_HUNT_Partner_Onboarding
WHERE statusCode IN (15, 20);  -- Approved or Rejected

-- Approval Rate
SELECT
  COUNT(CASE WHEN statusCode = 15 THEN 1 END) * 100.0 /
  COUNT(*) as Approval_Rate_Percent
FROM tbl_API_HUNT_Partner_Onboarding;

-- Partners Onboarded Per Month
SELECT
  YEAR(created_date) as Year,
  MONTH(created_date) as Month,
  COUNT(*) as Partners_Onboarded
FROM tbl_API_HUNT_Partner_Onboarding
WHERE statusCode = 15  -- Approved only
GROUP BY YEAR(created_date), MONTH(created_date);

-- Approver Performance
SELECT
  a.ApproverUserID,
  u.EmpCode,
  COUNT(*) as Total_Approvals,
  AVG(DATEDIFF(hour, a.CreatedDate, GETDATE())) as Avg_Pending_Hours
FROM tbl_API_HUNT_PO_ApprovalTrailTable a
JOIN tbl_API_HUNT_USER u ON a.ApproverUserID = u.EmpCode
WHERE a.Status = 'PENDING'
GROUP BY a.ApproverUserID, u.EmpCode;
```

---

## CONCLUSION

This detailed module relationship document provides:

✓ Complete module interaction matrix
✓ Feature matrix with all features and their status
✓ Database relationships in SQL format
✓ Entity relationship diagram for core entities
✓ Shared service dependencies
✓ Complete state machine diagrams
✓ Approval routing algorithms
✓ Master data dependencies
✓ Metrics and diagnostic queries

**Key Integration Points:**
- All modules depend on: Authentication, Activity Logging, Session Management
- Notification flow: All case updates → Email notifications
- File management: Onboarding, Approval, Integration modules use file uploads
- JIRA integration: Created for Partner, Exception, and Integration cases
- Data model: Normalized with clear relationships between entities

---

**Document Generated**: 2026-04-06
**Codebase**: ASP.NET Core 8.0 (Hunt)
**Target**: Migration Planning & Architecture Documentation
