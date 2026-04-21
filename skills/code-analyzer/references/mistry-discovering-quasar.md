# Legacy Codebase Transformation Plan

## Context

The Hunt (API_HUNT) ASP.NET Core 8.0 MVC codebase has a critical god-controller problem: `HomeController.cs` is 4,887 LOC (49% of the codebase) with 48 methods spanning 5+ domains. `JIRACreatorController` injects HomeController directly as a dependency. There are 12 dead files, duplicate partner integration controllers, and stub implementations. The goal is to modernize the architecture systematically while preserving all existing URLs and business logic.

**Key constraint:** `_Layout.cshtml` has hardcoded nav links (`/home/index/`, `/home/search/`, etc.) and 25+ views use `Html.BeginForm("...", "Home")`. We **cannot** rename or split the Home controller routing — we extract logic into services instead.

---

## Phase 0: Dead Code Removal

**Delete these files (zero runtime references):**
- `startupclass.cs` — dead legacy Startup class
- `Controllers/CommonController.cs` — wrong namespace, references dead Startup
- `Models/AESEncrytDecry.cs` — empty class
- `Models/GetConnectionString.cs` — empty class
- `Models/ExceptionManager.cs` — empty class
- `Models/ResponseContent.cs` — unreferenced model
- `Models/JiraIssueModel.cs` — unreferenced model
- `Models/JsonSoapModel.cs` — 5 unreferenced classes
- `Models/ApprovalListModels.cs` — zero references
- `Views/Home/newintegration1.cshtml` — orphaned view
- `Views/Partner_Integration/ViewPartnerIntegration_colors.cshtml` — orphaned view
- `Views/Partner_Integration/sentToApproval.cshtml` — orphaned view

**Mark as obsolete (implement interfaces, can't delete yet):**
- `Repositories/Implementations/AdminRepository.cs` — add `[Obsolete("Stub")]`
- `Models/SendEmail.cs` — add `[Obsolete("Stub")]`
- `Models/AppEDCrypto.cs` — add `[Obsolete("Crypto no-op")]`

**Verify:** `dotnet build`

---

## Phase 1: Extract ExcelExportService (Break JIRACreator → HomeController anti-pattern)

**Create:** `Services/IExcelExportService.cs` + `Services/ExcelExportService.cs`
- Move body of `HomeController.Table_Export_IntegratedXL` (lines 3484-3960, ~480 LOC) into `ExcelExportService.ExportIntegrationDocument(DataSet ds, int id, string docName)`
- This method is stateless — no HttpContext/Session/TempData usage, only DataSet + EPPlus + file I/O

**Modify `HomeController.cs`:**
- Add `IExcelExportService` to constructor
- Keep `Table_Export_IntegratedXL` as 1-line pass-through → `_excelExportService.ExportIntegrationDocument(ds, Id, DocName)`

**Modify `JIRACreaterController.cs`:**
- Replace `HomeController _homeController` with `IExcelExportService _excelExportService`
- Line 313: `_homeController.Table_Export_IntegratedXL(...)` → `_excelExportService.ExportIntegrationDocument(...)`

**Modify `Program.cs`:**
- Add: `builder.Services.AddScoped<IExcelExportService, ExcelExportService>();`
- Remove: `builder.Services.AddTransient<API_HUNT.Controllers.HomeController>();`

**Verify:** `dotnet build`

---

## Phase 2: Extract IntegrationWorkflowService (Shared Helpers)

**Create:** `Services/IIntegrationWorkflowService.cs` + `Services/IntegrationWorkflowService.cs`

Move these 5 helper method bodies from HomeController (shared by `newintegration POST` and `Appprovalintegration POST`):
- `RemoveListData` (lines 3995-4008) — pure model manipulation
- `UpdateWorkflowIntegrationservice` (lines 4010-4043) — needs `ISubmitRepository`
- `DeletePriviousdocinupdate` (lines 4419-4441) — pure file I/O
- `CaptureProductivityDetails` (line 4414) — needs `IActivityLogRepository`; drop dead `MySqlConnection` first param (always `null!`)
- `SetFilePath` (lines 4835-4839) — needs folder name from caller (pass as param instead of TempData)

**Modify `HomeController.cs`:**
- Add `IIntegrationWorkflowService` to constructor
- Replace method bodies with delegation calls
- Update all `CaptureProductivityDetails(null!, ...)` calls to drop the first arg

**Modify `Program.cs`:**
- Add: `builder.Services.AddScoped<IIntegrationWorkflowService, IntegrationWorkflowService>();`

**Verify:** `dotnet build`

---

## Phase 3: Extract FileOperationService

**Create:** `Services/IFileOperationService.cs` + `Services/FileOperationService.cs`

Move method bodies:
- `DisplayFile` (lines 4635-4697) — pure file read + content type detection
- `DownloadZip` (lines 4781-4826) — zip creation from folder path
- `UploadFile` (lines 4842-4887) — FTP upload via `ISubmitRepository.GetFTPCredential()`

HomeController actions become thin wrappers that read session/TempData and delegate.

**Modify `Program.cs`:**
- Add: `builder.Services.AddScoped<IFileOperationService, FileOperationService>();`

**Verify:** `dotnet build`

---

## Phase 4: Extract AutocompleteLookupService

**Create:** `Services/IAutocompleteLookupService.cs` + `Services/AutocompleteLookupService.cs`

Move logic from 13 JSON autocomplete endpoints (all stateless LINQ filters over `ISubmitRepository`):
- `AutoFilterExistingServiceName`, `GetExistingServiceRecordById`, `GetAutoTestApi`, `GetRequestRecord`, `AutoExternalServiceNameFilter`, `GetExternalServiceNameRecordById`, `AutoProducerAppFilter`, `GetProducerAppId`, `GetProducerAppfullName`, `AutoConsumerAppFilter`, `GetConsumerAppId`, `GetConsumerAppText`, `GetBTGPorjectMngr`

HomeController actions become: `return Json(_lookupService.Xxx(prefix));`

**Modify `Program.cs`:**
- Add: `builder.Services.AddScoped<IAutocompleteLookupService, AutocompleteLookupService>();`

**Verify:** `dotnet build`

---

## Phase 5: Consolidate Partner Integration Controllers

Merge `Partner_IntegrationController` (legacy, 661 LOC) into `PartnerIntegrationController` (new, 451 LOC).

1. Move unique legacy actions into `PartnerIntegrationController`
2. Add `[Route("Partner_Integration/[action]")]` aliases on migrated actions to preserve old URLs
3. Bulk replace `"Partner_Integration"` → `"PartnerIntegration"` in `Views/Partner_Integration/*.cshtml` (28 occurrences)
4. Move view files from `Views/Partner_Integration/` → `Views/PartnerIntegration/`
5. Add `[CustomFilter]` to `PartnerIntegrationController`
6. Delete `Partner_IntegrationController.cs` and `Views/Partner_Integration/`

**Verify:** `dotnet build` + test all partner integration URLs

---

## Phase 6: Final Cleanup

1. Remove all commented-out code blocks in HomeController
2. Remove dead field `int workflowstatus = 0;` (line 38)
3. Organize HomeController into `#region` blocks
4. Remove unused `IPartnerApprovalRepository` DI registration (verify first)
5. Rename `JIRACreaterController.cs` → `JIRACreatorController.cs` (match class name)

**Expected result:** HomeController drops from ~4,887 → ~1,800 LOC. Remaining code is orchestration logic (session, view data, branching) that belongs on the controller.

---

## New Files Summary

| File | Phase |
|------|-------|
| `Services/IExcelExportService.cs` | 1 |
| `Services/ExcelExportService.cs` | 1 |
| `Services/IIntegrationWorkflowService.cs` | 2 |
| `Services/IntegrationWorkflowService.cs` | 2 |
| `Services/IFileOperationService.cs` | 3 |
| `Services/FileOperationService.cs` | 3 |
| `Services/IAutocompleteLookupService.cs` | 4 |
| `Services/AutocompleteLookupService.cs` | 4 |

## Verification

After each phase: `dotnet build` must succeed. After all phases: navigate every route in `_Layout.cshtml`, test integration creation, approval, JIRA creation, file download, autocomplete fields.
