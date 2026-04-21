# Hunt - Class Diagram

```mermaid
classDiagram

%% ============================================================
%% ASP.NET CORE BASE
%% ============================================================
class Controller {
    <<abstract>>
}

class IActionFilter {
    <<interface>>
    +OnActionExecuting(context)
    +OnActionExecuted(context)
}

%% ============================================================
%% CONTROLLERS
%% ============================================================

class HomeController {
    -MySqlConnection sqlCon
    -SubmitRepository submitRepository
    -int workflowstatus
    -MySqlCommand cmd
    -MySqlDataAdapter sda
    -HttpClient _httpClient
    +Index() IActionResult
    +DiagnoseIssue() IActionResult
    +DiagnoseIssue(IFormCollection) IActionResult
    +DiagnoseIssueLive() IActionResult
    +DiagnoseIssueLive(IFormCollection) IActionResult
    +search(string) IActionResult
    +NewIntegrationList(NewIntegration, int, string) IActionResult
    +newintegration(NewIntegration, int) IActionResult
    +newintegrationDetails(int) IActionResult
    +newintegration(NewIntegration, string) IActionResult
    +search(SearchAPI) IActionResult
    +TestAPI(SearchAPI, string) IActionResult
    +APIDetail(SearchAPI, string) IActionResult
    +TestAPI() IActionResult
    +apiDetails(int, string, SearchAPI, string) IActionResult
    +Appprovalintegration(NewIntegration, int) IActionResult
    +Appprovalintegration(NewIntegration, string) IActionResult
    +DownloadFile(string) IActionResult
    +DownloadIntegrationFiles(int) JsonResult
    +faq() IActionResult
    +AutoFilterExistingServiceName(string) JsonResult
    +GetExistingServiceRecordById(int) JsonResult
    +GetAutoTestApi(string) JsonResult
    +GetRequestRecord(string, string, string) JsonResult
    +AutoExternalServiceNameFilter(string) JsonResult
    +GetExternalServiceNameRecordById(string) JsonResult
    +AutoProducerAppFilter(string) JsonResult
    +GetProducerAppId(int) JsonResult
    +GetProducerAppfullName(string) JsonResult
    +AutoConsumerAppFilter(string) JsonResult
    +GetConsumerAppId(int) JsonResult
    +GetConsumerAppText(string) JsonResult
    +DisplayFile(string) IActionResult
    +GetQuestonDetail(NewIntegration) JsonResult
    +DownloadZip(string) IActionResult
    +GetBTGPorjectMngr(string) JsonResult
    +UploadFile(string, IFormFile) IActionResult
    +CaptureProductivityDetails(MySqlConnection, string, string, string, int, string, string) void
    +Table_Export_IntegratedXL(DataSet, int, string) void
}

class AdminController {
    -AdminRepository Adminrepository
    -AdminMaster adminMaster
    -displayUsermaster disUsermaster
    -displayAppmaster disAppmaster
    -HomeController homeController
    -MySqlConnection sqlCon
    +Index() IActionResult
    +Admins() IActionResult
    +Admins(AdminMaster, string) IActionResult
    +GetUserDetail(string) JsonResult
    +GetAppDetail(string) JsonResult
    +GetAppDetailpoc(string, string) JsonResult
    +AddUpdateAppMaster(displayAppmaster) JsonResult
    +AddUpdateUserMaster(displayUsermaster) JsonResult
    +GetUserName(string) JsonResult
}

class LoginController {
    -MySqlConnection sqlCon
    -DataSet chkstatus
    -ResponseContent message
    +Index(string, string) IActionResult
    +Index(LoginModel) IActionResult
    +LogoutUser() ActionResult
    +RedirectToLogin() ActionResult
    +checkUserMaster(string) DataSet
    +chkmappmaster(int, string) DataTable
    +GetMOfeeUrl() string
    +GetUserRole(string) string
    +ValidateActiveDirectoryLogin(string, string, string) bool
    +CaptureProductivityDetails(MySqlConnection, string, string, string, int, string, string) void
}

class CommonController {
    -MySqlConnection sqlCon
    +Index() IActionResult
    +SessionExpiry() IActionResult
}

class PartnerDashboardController {
    +PartnerDashboard() IActionResult
    +PartnerDashboardNew() IActionResult
}

class PartnerOnboardingController {
    -PartnerRepository submitrepository_Obj
    -HomeController homeController
    -MySqlConnection sqlCon
    -string uploadFolderPath
    +Index() IActionResult
    +ListofPartner(string) IActionResult
    +AddPartnerOnBoarding() IActionResult
    +GetPartnerTypeList() JsonResult
    +SaveAddPartnerOnBoarding(string, string) IActionResult
    +SavePartnerIntegration(string, string) IActionResult
    +GetPartnerByName(string) IActionResult
}

class PartnerOnboardingNewController {
    -HomeController homeController
    -MySqlConnection sqlCon
    -string uploadFolderPath
    +ListofPartner() IActionResult
    +AddPartnerOnBoarding() IActionResult
    +GetMstPartnerType(string, string, string, string) JsonResult
    +SaveAddPartneronBoarding(string, string) IActionResult
    +SaveIntegration(string, string) IActionResult
}

class PartnerApprovalController {
    -string uploadFolderPath
    -HomeController homeController
    -MySqlConnection sqlCon
    -PartnerApprovalRepository partnerApprovalRepository
    +Index() IActionResult
    +ApprovedPartner(string) IActionResult
    +SaveAddPartnerApproval(PartnerApprovalModel) IActionResult
    +ShowImage(string) IActionResult
    +DownloadFile(string) IActionResult
    +DisplayFile(string) IActionResult
    +GetContentType(string) string
}

class PartnerApprovalNewController {
    -HomeController homeController
    -MySqlConnection sqlCon
    -string uploadFolderPath
    -PartnerOnboardingNewRepository PartnerRepository
    +ListofPartner() IActionResult
    +ApprovedPartner(string, string) IActionResult
    +SaveAddPartnerApproval(PartnerApprovalNewModel) IActionResult
}

class PartnerIntegrationController {
    -PartnerRepository submitrepository_Obj
    -HomeController homeController
    -MySqlConnection sqlCon
    -string uploadFolderPath
    +Index() IActionResult
    +AddEditPartnerIntegration() IActionResult
    +ListofPartnerIntegration(string) IActionResult
}

class PartnerIntegrationNewController {
    -HomeController homeController
    -MySqlConnection sqlCon
    -string uploadFolderPath
    -PartnerOnboardingNewRepository partnerOnboardingNewRepository
    +ListofPartner(string) IActionResult
    +AddEditIntegration() IActionResult
    +SaveIntegration(string, string) IActionResult
}

class Partner_IntegrationController {
    -PartnerRepository submitrepository_Obj
    -HomeController homeController
    -MySqlConnection sqlCon
    -string uploadFolderPath
    +Index() IActionResult
    +ListofPartnerIntegration(string) IActionResult
}

class PartnerOffboardingController {
    -string uploadFolderPath
    -PartnerOffboardingRepository OffboardingRepository
    +PartnerOffboarding() IActionResult
    +PartnerOffboardingDetails(string) JsonResult
    +PartnerOffboardingapiname(string) JsonResult
    +AddAPIList(string) ActionResult
    +Upload(PartnerOffboarding) IActionResult
    +OtherUpload(PartnerOffboarding) IActionResult
    +clearsubmit() IActionResult
    +clearsubmit(PartnerOffboarding) IActionResult
    +StoreFileLocally(IFormFile, string, string) void
    +GetUniqueFileName(string) string
}

class ExceptionManagementController {
    -ExceptionRepository exceptionRepository
    +Index() IActionResult
    +ListOfExceptions() IActionResult
    +ListOfExceptions(string) IActionResult
    +AddEditException(string) IActionResult
    +SaveaddeditException(AddEditExceptionModel) IActionResult
    +ExceptionLevel1(AddEditExceptionModel) IActionResult
    +ExceptionLevel2(AddEditExceptionModel) IActionResult
    +ExceptionLevel3(AddEditExceptionModel) IActionResult
    +GetExceptionLevel(string) JsonResult
    +SaveExceptionLevel1(ExceptionLevels) IActionResult
    +SaveExceptionLevel2(ExceptionLevels) IActionResult
    +SaveExceptionLevel3(ExceptionLevels) IActionResult
}

class ExceptionApprovalController {
    +Index() IActionResult
}

class JIRACreaterController {
    -MySqlConnection sqlCon
    -SubmitRepository submitRepository
    -JIRARepository jIRARepository
    -HomeController homeController
    -HttpClient _httpClient
    -MySqlCommand cmd
    -MySqlDataAdapter sda
    +CreateJIRA(NewIntegration) Task~NewIntegration~
    +JIRADocUpload(NewIntegration, string) Task~string~
}

%% ============================================================
%% REPOSITORIES
%% ============================================================

class AdminRepository {
    +GetUsers() List~AdminMaster~
    +GetApps() List~displayAppmaster~
    +AddUpdateUser(displayUsermaster) bool
    +AddUpdateApp(displayAppmaster) bool
}

class ExceptionRepository {
    +GetExceptions() List~ExceptionManager~
    +GetExceptionById(string) AddEditExceptionModel
    +SaveException(AddEditExceptionModel) bool
    +SaveLevel1(ExceptionLevels) bool
    +SaveLevel2(ExceptionLevels) bool
    +SaveLevel3(ExceptionLevels) bool
}

class PartnerRepository {
    +GetPartners() List~PartnerOnboarding~
    +GetPartnerById(int) PartnerOnboarding
    +SavePartner(PartnerOnboarding) bool
}

class PartnerApprovalRepository {
    +GetPendingApprovals() List~PartnerApprovalModel~
    +SaveApproval(PartnerApprovalModel) bool
}

class PartnerOffboardingRepository {
    +GetOffboardingDetails(string) PartnerOffboardingModel
    +SaveOffboarding(PartnerOffboardingModel) bool
}

class PartnerOnboardingNewRepository {
    +GetPartners() List~PartnerOnboardingNew~
    +GetPartnerById(int) PartnerOnboardingNew
    +SavePartner(PartnerOnboardingNew) bool
    +SaveIntegration(string) bool
    +GetApprovals() List~PartnerApprovalNewModel~
    +SaveApproval(PartnerApprovalNewModel) bool
}

class JIRARepository {
    +CreateIssue(JiraIssueModel) Task~JIRAResponseModel~
    +UploadDocument(string, string) Task~string~
}

class SubmitRepository {
    +GetIntegrations() List~NewIntegration~
    +GetIntegrationById(int) NewIntegration
    +SaveIntegration(NewIntegration) bool
}

%% ============================================================
%% DOMAIN MODELS
%% ============================================================

class AdminMaster {
    +string UserId
    +string UserName
    +string Role
    +string Email
    +string Department
}

class displayUsermaster {
    +string UserID
    +string UserName
    +string Email
    +string Role
    +string Department
}

class displayAppmaster {
    +string AppId
    +string AppName
    +string AppPOC
    +string AppPOCEmail
}

class LoginModel {
    +string UserName
    +string Password
    +string Domain
}

class NewIntegration {
    +int IntegrationId
    +string ServiceName
    +string ExternalServiceName
    +string ProducerApp
    +string ConsumerApp
    +string Status
    +string JIRAId
    +string BTGProjectManager
    +string ZipFolderName
    +List~string~ Documents
}

class SearchAPI {
    +string ServiceName
    +string Path
    +string ServiceUrl
    +string ApiCategory
    +int Id
}

class PartnerOnboarding {
    +int PartnerId
    +string PartnerName
    +string PartnerType
    +string Status
    +string CreatedBy
    +DateTime CreatedDate
    +List~string~ Documents
}

class PartnerOnboardingNew {
    +int PartnerId
    +string PartnerName
    +string PartnerType
    +string PartnerSubType
    +string PartnerEntityType
    +string IdentFlag
    +string Status
    +string CreatedBy
    +DateTime CreatedDate
}

class PartnerApproval {
    +int ApprovalId
    +int PartnerId
    +string ApprovedBy
    +string ApprovalStatus
    +DateTime ApprovalDate
    +string Comments
}

class PartnerApprovalModel {
    +int ApprovalId
    +int PartnerId
    +string PartnerName
    +string ApprovedBy
    +string ApprovalStatus
    +string Comments
    +List~string~ Documents
}

class PartnerApprovalNewModel {
    +int ApprovalId
    +int PartnerId
    +string PartnerName
    +string ApprovedBy
    +string ApprovalStatus
    +string Comments
    +string Status
}

class PartnerOffboarding {
    +string PartnerName
    +string CaseId
    +string Reason
    +string Status
    +List~string~ APIList
    +IFormFile Document
    +IFormFile OtherDocument
}

class PartnerOffboardingModel {
    +string PartnerName
    +string CaseId
    +string Reason
    +string Status
    +List~string~ APIList
}

class AddEditExceptionModel {
    +string CaseId
    +string ExceptionTitle
    +string Description
    +string ImpactOnBank
    +string RiskLevel
    +string Status
    +string RequestedBy
    +DateTime RequestedDate
    +int ApprovalLevel
    +ExceptionLevels Level1Details
    +ExceptionLevels Level2Details
    +ExceptionLevels Level3Details
}

class ExceptionLevels {
    +string CaseId
    +int Level
    +string ApprovedBy
    +string ApprovalStatus
    +string Comments
    +DateTime ApprovalDate
    +List~AddExceptionlevelDetails~ LevelDetails
}

class AddExceptionlevelDetails {
    +string FieldName
    +string FieldValue
}

class ExceptionManager {
    +string CaseId
    +string ExceptionTitle
    +string Status
    +string RiskLevel
    +DateTime CreatedDate
    +string CreatedBy
}

class JiraIssueModel {
    +string ProjectKey
    +string Summary
    +string Description
    +string IssueType
    +string Priority
    +string AssigneeName
    +int IntegrationId
    +List~string~ Attachments
}

class JIRAResponseModel {
    +string Id
    +string Key
    +string Self
    +bool Success
    +string ErrorMessage
}

class JsonModel {
    +string Key
    +string Value
    +string Type
}

class JsonSoapModel {
    +string RequestBody
    +string Endpoint
    +string SoapAction
    +string ResponseBody
}

class ResponseContent {
    +bool IsSuccess
    +string Message
    +string Data
    +int StatusCode
}

class TestAPI {
    +string Endpoint
    +string Method
    +string Headers
    +string RequestBody
    +string ResponseBody
    +int StatusCode
}

class SendEmail {
    +string To
    +string CC
    +string Subject
    +string Body
    +bool IsHTML
    +List~string~ Attachments
    +Send() bool
}

class AESEncrytDecry {
    +Encrypt(string) string
    +Decrypt(string) string
}

class GetConnectionString {
    +GetConnection() string
    +GetConnectionByName(string) string
}

class DBClass {
    +ExecuteQuery(string) DataSet
    +ExecuteNonQuery(string) int
}

class CustomFilter {
    <<attribute>>
    +OnActionExecuting(ActionExecutingContext) void
    +OnActionExecuted(ActionExecutedContext) void
}

class ErrorViewModel {
    +string RequestId
    +bool ShowRequestId
}

%% ============================================================
%% INHERITANCE RELATIONSHIPS
%% ============================================================

Controller <|-- HomeController
Controller <|-- AdminController
Controller <|-- LoginController
Controller <|-- CommonController
Controller <|-- PartnerDashboardController
Controller <|-- PartnerOnboardingController
Controller <|-- PartnerOnboardingNewController
Controller <|-- PartnerApprovalController
Controller <|-- PartnerApprovalNewController
Controller <|-- PartnerIntegrationController
Controller <|-- PartnerIntegrationNewController
Controller <|-- Partner_IntegrationController
Controller <|-- PartnerOffboardingController
Controller <|-- ExceptionManagementController
Controller <|-- ExceptionApprovalController
Controller <|-- JIRACreaterController

IActionFilter <|.. CustomFilter

%% ============================================================
%% CONTROLLER → REPOSITORY DEPENDENCIES
%% ============================================================

HomeController --> SubmitRepository : uses
AdminController --> AdminRepository : uses
AdminController --> HomeController : uses
LoginController --> ResponseContent : uses
PartnerOnboardingController --> PartnerRepository : uses
PartnerOnboardingController --> HomeController : uses
PartnerApprovalController --> PartnerApprovalRepository : uses
PartnerApprovalController --> HomeController : uses
PartnerApprovalNewController --> PartnerOnboardingNewRepository : uses
PartnerApprovalNewController --> HomeController : uses
PartnerIntegrationController --> PartnerRepository : uses
PartnerIntegrationController --> HomeController : uses
PartnerIntegrationNewController --> PartnerOnboardingNewRepository : uses
PartnerIntegrationNewController --> HomeController : uses
Partner_IntegrationController --> PartnerRepository : uses
Partner_IntegrationController --> HomeController : uses
PartnerOffboardingController --> PartnerOffboardingRepository : uses
ExceptionManagementController --> ExceptionRepository : uses
JIRACreaterController --> SubmitRepository : uses
JIRACreaterController --> JIRARepository : uses
JIRACreaterController --> HomeController : uses

%% ============================================================
%% CONTROLLER → MODEL DEPENDENCIES
%% ============================================================

HomeController --> NewIntegration : handles
HomeController --> SearchAPI : handles
AdminController --> AdminMaster : handles
AdminController --> displayUsermaster : handles
AdminController --> displayAppmaster : handles
LoginController --> LoginModel : handles
PartnerOnboardingController --> PartnerOnboarding : handles
PartnerOnboardingNewController --> PartnerOnboardingNew : handles
PartnerApprovalController --> PartnerApprovalModel : handles
PartnerApprovalNewController --> PartnerApprovalNewModel : handles
PartnerOffboardingController --> PartnerOffboarding : handles
ExceptionManagementController --> AddEditExceptionModel : handles
ExceptionManagementController --> ExceptionLevels : handles
JIRACreaterController --> NewIntegration : handles
JIRACreaterController --> JiraIssueModel : handles

%% ============================================================
%% REPOSITORY → MODEL DEPENDENCIES
%% ============================================================

AdminRepository --> AdminMaster : returns
AdminRepository --> displayUsermaster : returns
AdminRepository --> displayAppmaster : returns
ExceptionRepository --> ExceptionManager : returns
ExceptionRepository --> AddEditExceptionModel : returns
ExceptionRepository --> ExceptionLevels : returns
PartnerRepository --> PartnerOnboarding : returns
PartnerApprovalRepository --> PartnerApprovalModel : returns
PartnerOffboardingRepository --> PartnerOffboardingModel : returns
PartnerOnboardingNewRepository --> PartnerOnboardingNew : returns
PartnerOnboardingNewRepository --> PartnerApprovalNewModel : returns
JIRARepository --> JIRAResponseModel : returns
JIRARepository --> JiraIssueModel : uses
SubmitRepository --> NewIntegration : returns

%% ============================================================
%% MODEL COMPOSITION RELATIONSHIPS
%% ============================================================

AddEditExceptionModel *-- ExceptionLevels : contains
ExceptionLevels *-- AddExceptionlevelDetails : contains
NewIntegration --> SearchAPI : relates
PartnerOffboarding --> PartnerOffboardingModel : maps to
```
