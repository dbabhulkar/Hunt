# API Endpoints & Route Reference

All routes follow the pattern `/{Controller}/{Action}/{id?}`. Default route: `/Login/Index`.

## LoginController

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/Login/Index` | Login page (or SSO via `?USERID=&USERNAME=` query params) |
| POST | `/Login/Index` | Form login with EmpCode + password |
| GET | `/Login/LogoutUser` | Logout, clear session, redirect to Mofee |
| POST | `/Login/RedirectToLogin` | Redirect to Login page|

## HomeController `[CustomFilter]`

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/Home/Index` | Main dashboard |
| GET | `/Home/DiagnoseIssue` | Health diagnostics page (UAT) |
| POST | `/Home/DiagnoseIssue` | Redirect to UAT monitoring URL |
| GET | `/Home/DiagnoseIssueLive` | Health diagnostics page (Live) |
| POST | `/Home/DiagnoseIssueLive` | Redirect to Live monitoring URL |
| GET | `/Home/search` | API search page |
| GET | `/Home/NewIntegrationList` | Integration list (filtered by role + status) |
| GET | `/Home/newintegration` | Create/edit integration form |
| GET | `/Home/TestAPI` | API testing page |
| GET | `/Home/faq` | FAQ page |
| GET | `/Home/apiDetails` | API details view |

## AdminController

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/Admin/Index` | Admin panel landing |
| GET | `/Admin/Admins` | User & App master list |
| POST | `/Admin/Admins` | Admin form submission |
| POST | `/Admin/GetUserDetail` | AJAX: Load user details for edit |
| POST | `/Admin/GetAppDetail` | AJAX: Load app details for edit |
| GET | `/Admin/GetAppDetailpoc` | AJAX: Load app details by ID + name |
| POST | `/Admin/AddUpdateAppMaster` | AJAX: Insert or update app master |
| POST | `/Admin/AddUpdateUserMaster` | AJAX: Insert or update user master |
| POST | `/Admin/GetUserName` | AJAX: Lookup user name by ID |

## PartnerOnboardingController

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/PartnerOnboarding/ListofPartner` | Partner list (USER role only) |
| GET | `/PartnerOnboarding/AddPartnerOnBoarding` | New partner form |
| GET | `/PartnerOnboarding/GetMstPartnerType` | AJAX: Cascade partner type dropdowns |
| GET | `/PartnerOnboarding/SaveAddPartneronBoarding` | Save new partner (JSON + files) |
| GET | `/PartnerOnboarding/ViewPartneronBoarding?id=` | View partner details |
| GET | `/PartnerOnboarding/EditPartneronBoarding?id=` | Edit partner form |
| GET | `/PartnerOnboarding/SaveEditPartneronBoarding` | Save partner edits |
| GET | `/PartnerOnboarding/DownloadFile?fileName=` | Download uploaded document |
| GET | `/PartnerOnboarding/DisplayFile?fileName=` | Display/preview document |

## PartnerIntegrationController

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/PartnerIntegration/ListofPartner?Status=` | Integration case list with status filter |
| GET | `/PartnerIntegration/AddPartnerIntegration` | New integration case form |
| GET | `/PartnerIntegration/GetPartnerDetails?partnerID=` | AJAX: Load partner details for selected partner |
| GET | `/PartnerIntegration/PartnerCaseApprovalMetrix` | AJAX: Load approval matrix based on risk |
| POST | `/PartnerIntegration/GetlstApiDetail` | AJAX: API name autocomplete |
| POST | `/PartnerIntegration/GetApiDetail` | AJAX: Load API risk details |
| POST | `/PartnerIntegration/SavePartnerIntegration` | Save new integration case |
| GET | `/PartnerIntegration/EditPartnerIntegration?id=&status=` | Edit integration case |
| GET | `/PartnerIntegration/ViewPartnerIntegration?id=` | View integration case |
| POST | `/PartnerIntegration/SaveEditPartnerIntegration` | Save integration edits |
| GET | `/PartnerIntegration/DeletePartneronBoarding?id=` | Delete draft integration |
| GET | `/PartnerIntegration/DownloadFile?fileName=` | Download document |
| GET | `/PartnerIntegration/DisplayFile?fileName=` | Display/preview document |

## PartnerApprovalController

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/PartnerApproval/ListofPartner` | Pending approvals list (current user) |
| GET | `/PartnerApproval/ApprovedPartner?id=&status=` | Approval detail/action page |
| POST | `/PartnerApproval/SaveAddPartnerApproval` | Submit approval/rejection/feedback |
| GET | `/PartnerApproval/ShowImage?fileName=` | Display uploaded image |
| GET | `/PartnerApproval/DownloadFile?fileName=` | Download document |
| GET | `/PartnerApproval/DisplayFile?fileName=` | Display/preview document |

## Partner_IntegrationController (Legacy)

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/Partner_Integration/ListofPartnerIntegration?Status=` | Legacy partner integration list |
| GET | `/Partner_Integration/AddEditIntegration` | Legacy add/edit form |
| POST | `/Partner_Integration/SaveAddPartneronBoarding` | Legacy save partner |
| GET | `/Partner_Integration/EditPartnerIntegration?id=&status=` | Legacy edit |
| POST | `/Partner_Integration/SaveEditPartner` | Legacy save edits |
| GET | `/Partner_Integration/ViewPartnerIntegration?id=` | Legacy view |
| POST | `/Partner_Integration/GetMstPartnerType` | AJAX: Partner type cascade |
| POST | `/Partner_Integration/GetlstApiDetail` | AJAX: API autocomplete |
| POST | `/Partner_Integration/GetApiDetail` | AJAX: API detail lookup |
| POST | `/Partner_Integration/FeedbackPartial` | AJAX: Load feedback partial view |
| POST | `/Partner_Integration/SaveFeedbackReply` | Save feedback reply |
| GET | `/Partner_Integration/DeletePartneronBoarding?id=` | Delete draft |
| GET | `/Partner_Integration/EditPartnerIntegrationNew?id=&status=` | Newer edit variant |
| POST | `/Partner_Integration/SaveEditPartnerNew` | Newer save variant |

## PartnerOffboardingController

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/PartnerOffboarding/PartnerOffboarding` | Offboarding page |
| POST | `/PartnerOffboarding/PartnerOffboardingDetails` | AJAX: Partner name autocomplete |
| POST | `/PartnerOffboarding/PartnerOffboardingapiname` | AJAX: Load partner's APIs |
| POST | `/PartnerOffboarding/AddAPIList` | Deactivate selected APIs |
| POST | `/PartnerOffboarding/Upload` | Upload offboarding document |
| POST | `/PartnerOffboarding/OtherUpload` | Upload additional document |

## ExceptionManagementController

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/ExceptionManagement/Index` | Exception list (all) |
| GET | `/ExceptionManagement/ListOfExceptions` | Exception list (USER role) |
| GET | `/ExceptionManagement/AddEditException?CaseId=` | Create/edit exception |
| POST | `/ExceptionManagement/SaveaddeditException` | Save exception + redirect to level form |
| GET | `/ExceptionManagement/ExceptionLevel1` | Level 1 approver selection |
| GET | `/ExceptionManagement/ExceptionLevel2` | Level 2 approver selection |
| GET | `/ExceptionManagement/ExceptionLevel3` | Level 3 approver selection |
| POST | `/ExceptionManagement/SaveExceptionLevel1` | Save Level 1 approvers |
| POST | `/ExceptionManagement/SaveExceptionLevel2` | Save Level 2 approvers |
| POST | `/ExceptionManagement/SaveExceptionLevel3` | Save Level 3 approvers |
| POST | `/ExceptionManagement/GetExceptionLevel` | AJAX: Determine level from impact |

## ExceptionApprovalController

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/ExceptionApproval/Index` | Exception approval list (current user) |

## PartnerDashboardController

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/PartnerDashboard/PartnerDashboard` | Dashboard view (legacy) |
| GET | `/PartnerDashboard/PartnerDashboardNew` | Dashboard view (new) |

## CommonController

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/Common/SessionExpiry` | Session expiry partial view |

## Status Filter Quick Reference

### PartnerIntegration & Partner_Integration ListofPartner
| `?Status=` | Shows |
|------------|-------|
| `1` | Created / Submit |
| `2` | Drafted |
| `3` | In Progress |
| `4` | Awaiting For Reply |
| `5` | Approved / Reject |
| (none) | All |

### HomeController NewIntegrationList
| `?Status=` | Shows |
|------------|-------|
| `1` | Review To BTGUser |
| `2` | Rejected |
| `3` | Review To ITUSER |
| `4` | Review To ITARCHITECH |
| `5` | Closed |
| `6` | Feedback to current user |
| (none) | Items assigned to current role |
