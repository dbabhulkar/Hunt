# Codebase Analysis Report

**Date:** 2026-04-20
**Scope:** Dependency graphs, dead code identification, complexity metrics, refactoring priorities

---

## Table of Contents

1. [Dependency Graph](#1-dependency-graph)
2. [Dead Code Inventory](#2-dead-code-inventory)
3. [Complexity Metrics](#3-complexity-metrics)
4. [Anti-Patterns](#4-anti-patterns)
5. [Refactoring Priority Matrix](#5-refactoring-priority-matrix)
6. [Refactoring Roadmap](#6-refactoring-roadmap)

---

## 1. Dependency Graph

### Controller to Repository Dependencies

```
LoginController
  └── ILoginRepository
  └── IActivityLogRepository

HomeController (4,887 LOC)
  └── ISubmitRepository
  └── IActivityLogRepository
  └── IDbConnectionFactory
  └── IJiraRepository
  └── IHttpClientFactory

AdminController
  └── IAdminRepository
  └── IActivityLogRepository

Partner_IntegrationController (LEGACY, 661 LOC)
  └── IPartnerRepository
  └── IActivityLogRepository

PartnerIntegrationController (NEW, 451 LOC)
  └── IPartnerOnboardingNewRepository
  └── IPartnerRepository
  └── IActivityLogRepository

PartnerOnboardingController
  └── IPartnerOnboardingNewRepository
  └── IActivityLogRepository

PartnerApprovalController
  └── IPartnerOnboardingNewRepository
  └── IActivityLogRepository

PartnerOffboardingController
  └── IPartnerOffboardingRepository

ExceptionManagementController
  └── IExceptionRepository

ExceptionApprovalController
  └── IExceptionRepository

JIRACreatorController
  └── HomeController  (ANTI-PATTERN: controller-to-controller DI)
  └── IJiraRepository
  └── ISubmitRepository
  └── IHttpClientFactory

PartnerDashboardController
  └── (no dependencies — empty shell)

CommonController
  └── (no DI — uses static SqlConnection from dead Startup class)
```

### DI Registration Summary (Program.cs)

| Interface | Implementation | Lifetime |
|---|---|---|
| IDbConnectionFactory | DbConnectionFactory | Singleton |
| IActivityLogRepository | ActivityLogRepository | Scoped |
| ILoginRepository | LoginRepository | Scoped |
| IAdminRepository | AdminRepository | Scoped |
| IExceptionRepository | ExceptionRepository | Scoped |
| IPartnerRepository | PartnerRepository | Scoped |
| IPartnerOnboardingNewRepository | PartnerOnboardingNewRepository | Scoped |
| IPartnerApprovalRepository | PartnerApprovalRepository | Scoped |
| IPartnerOffboardingRepository | PartnerOffboardingRepository | Scoped |
| ISubmitRepository | SubmitRepository | Scoped |
| IJiraRepository | JIRARepository | Scoped |
| HomeController | (self) | Transient |

### Cross-Layer Anti-Patterns

| Issue | Location | Severity |
|---|---|---|
| Controller injected into controller | `JIRACreatorController` depends on `HomeController` for `Table_Export_IntegratedXL` | HIGH |
| HomeController registered as Transient to support above | `Program.cs:56` | HIGH |
| Duplicate controllers for same domain | `Partner_IntegrationController` + `PartnerIntegrationController` | HIGH |
| Unused DI registration | `IPartnerApprovalRepository` registered but never injected by any controller | MEDIUM |

---

## 2. Dead Code Inventory

### Tier 1: Confirmed Dead Files (safe to delete, zero runtime references)

| File | LOC | Evidence |
|---|---|---|
| `startupclass.cs` | 111 | Legacy ASP.NET Core 2.1 Startup class; `Program.cs` fully supersedes it. Not referenced anywhere. |
| `Models/GetConnectionString.cs` | 6 | Empty class body, zero references in `.cs` or `.cshtml` files |
| `Models/AESEncrytDecry.cs` | 6 | Empty class body, zero references. Real crypto is in `AppEDCrypto.cs` |
| `Models/ExceptionManager.cs` | 6 | Empty class body, zero references |
| `Models/PartnerApproval.cs` | ~6 | Empty stub. `PartnerApprovalNew.cs` and `PartnerApprovalModel.cs` are the real classes |
| `Models/ResponseContent.cs` | 9 | Model with 3 properties, never referenced outside its own file |
| `Models/JiraIssueModel.cs` | 13 | Model with 7 properties, never referenced outside its own file |
| `Models/JsonSoapModel.cs` | 37 | 5 classes (`JsonSoapModel`, `SoapRequestDetail`, `SoapSubItem`, `SoapApplicationItem`, `SoapApplication`), zero references |
| `Models/ApprovalListModels.cs` | ~10 | Zero references anywhere in codebase |
| `Views/Home/newintegration1.cshtml` | — | Orphaned view; no controller action returns this view name |
| `Views/Partner_Integration/ViewPartnerIntegration_colors.cshtml` | — | Orphaned view; no action references it |
| `Views/Partner_Integration/sentToApproval.cshtml` | — | Orphaned view; `sentApproval` exists but not `sentToApproval` |

**Estimated dead LOC (code files only): ~200+**

### Tier 2: Stub Implementations (functional dead code, investigate before removing)

| File | Issue | Evidence |
|---|---|---|
| `Repositories/Implementations/AdminRepository.cs` | Every method returns `null` or empty string | Lines 12-24: `AllAdminMaster()`, `UserMasterEdit()`, `AppMasterEdit()`, `AppMasterEditpoc()`, `AddUpdateAPPsMaster()`, `AddUpdateUsersMaster()`, `GetUserNames()` all return null/empty |
| `Models/SendEmail.cs` | All email methods return `false` or do nothing | Lines 11-17: stub implementations, never called from any controller |
| `Models/DataBaseConnection.cs` (`LoginUpdate`) | Method returns `string.Empty` | `DBClass` is passed in but ignored; called from `LoginController.cs:99,118,122` |
| `Models/AppEDCrypto.cs` | Encrypt/Decrypt are identity functions (return input unchanged) | `Encrypt(string plainText) => plainText;` and `Decrypt(string cipherText) => cipherText;` |

### Tier 3: Near-Dead / Legacy Controllers

| Controller | LOC | Issue |
|---|---|---|
| `CommonController.cs` | 28 | Wrong namespace (`OneViewIndicator.Controllers`), references dead `Startup.connectionstring`, no DI. `SessionExpiry()` action and `_SessionExpiry.cshtml` partial are never called. |
| `PartnerDashboardController.cs` | 20 | 2 methods returning empty views with no data binding |

### Unused DI Registration

`IPartnerApprovalRepository` / `PartnerApprovalRepository` is registered in `Program.cs:43` but no controller constructor injects it.

---

## 3. Complexity Metrics

### File Size Distribution

| Rank | File | LOC | % of Total (9,892) | Public Methods |
|---|---|---|---|---|
| 1 | `Controllers/HomeController.cs` | 4,887 | 49.4% | 48 |
| 2 | `Controllers/Partner_IntegrationController.cs` | 661 | 6.7% | 18 |
| 3 | `Controllers/PartnerIntegrationController.cs` | 451 | 4.6% | 16 |
| 4 | `Controllers/JIRACreaterController.cs` | 358 | 3.6% | ~8 |
| 5 | `Models/NewIntegration.cs` | 268 | 2.7% | 0 (95+ properties) |
| 6 | `Controllers/PartnerOnboardingController.cs` | 203 | 2.1% | ~6 |
| 7 | `Controllers/LoginController.cs` | 190 | 1.9% | 4 |
| 8 | `Controllers/ExceptionManagementController.cs` | 176 | 1.8% | 11 |
| 9 | `Controllers/PartnerApprovalController.cs` | 164 | 1.7% | 6 |
| 10 | `Models/PartnerOnboarding.cs` | 161 | 1.6% | 0 (model) |
| — | All other files (59) | 3,267 | 33.0% | — |

### Top 10 Most Complex Methods

| Rank | Method | File : Lines | LOC | Est. Control Flow Stmts | Severity |
|---|---|---|---|---|---|
| 1 | `newintegration` (POST) | `HomeController.cs:576-1891` | 1,315 | 85+ | CRITICAL |
| 2 | `Appprovalintegration` (POST) | `HomeController.cs:2615-3400+` | 800+ | 60+ | CRITICAL |
| 3 | `SaveAddPartneronBoarding` | `Partner_IntegrationController.cs:177-662` | 485 | 50+ | HIGH |
| 4 | `apiDetails` | `HomeController.cs:2136-2516` | 381 | 35+ | HIGH |
| 5 | `NewIntegrationList` | `HomeController.cs:199-468` | 270 | 45+ | HIGH |
| 6 | `Table_Export_IntegratedXL` | `HomeController.cs:3484-3962` | ~480 | 30+ | HIGH |
| 7 | `search` (POST) | `HomeController.cs:1895-1960+` | 100+ | 20+ | MEDIUM |
| 8 | `TestAPI` | `HomeController.cs:1996-2070` | 75 | 18+ | MEDIUM |
| 9 | `Appprovalintegration` (GET) | `HomeController.cs:2537-2614` | 78 | 15+ | MEDIUM |
| 10 | `DiagnoseIssueLive` (POST) | `HomeController.cs:126-164` | 39 | 10+ | LOW |

### God Objects

| Model | Properties | Concerns Mixed | Issue |
|---|---|---|---|
| `Models/NewIntegration.cs` | 95+ | 5: metadata, file handling, user/role info, dropdown data (18 `List<SelectItem>`), question/survey data | Should be split into focused DTOs |
| `Models/PartnerApprovalModel.cs` | ~30 | 2: approval fields, helper classes | Near-duplicate of `PartnerApprovalNew.cs` (86 LOC) |

### Classes Exceeding 15 Public Methods

| Class | Method Count | Status |
|---|---|---|
| `HomeController` | 48 | CRITICAL — 3x threshold |
| `Partner_IntegrationController` | 18 | HIGH — exceeds threshold |
| `PartnerIntegrationController` | 16 | HIGH — at threshold |

---

## 4. Anti-Patterns

### Structural Anti-Patterns

| Pattern | Occurrences | Locations | Impact |
|---|---|---|---|
| Controller-to-controller DI | 1 | `JIRACreatorController` injects `HomeController` to call `Table_Export_IntegratedXL` | Tight coupling, untestable |
| Duplicate controllers for same domain | 1 pair | `Partner_IntegrationController` (legacy) + `PartnerIntegrationController` (new) with separate view folders | Confusing routing, maintenance burden |
| God controller | 1 | `HomeController` (4,887 LOC, 48 methods, 5+ domains) | Single file = 49% of codebase |
| God object model | 1 | `NewIntegration.cs` (95+ properties mixing 5 concerns) | Impossible to reason about |
| Inconsistent repository layout | 2 patterns | Admin in `Repositories/Interfaces/` + `Repositories/Implementations/`; all others in `Models/` | Discoverability |
| Wrong namespace | 1 | `CommonController` uses `OneViewIndicator.Controllers` instead of `API_HUNT.Controllers` | Legacy artifact |
| File name != class name | 1 | File: `JIRACreaterController.cs`, Class: `JIRACreatorController` | Confusing |

### Code-Level Anti-Patterns

| Pattern | Occurrences | Locations | Impact |
|---|---|---|---|
| Copy-paste file path construction | 15+ | `HomeController.cs` throughout (e.g., `Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIHuntDoc\\NewIntegrations\\" + "API" + date + id + "\\")`) | Bug-prone, DRY violation |
| Role-based filter logic duplicated | 4x | `HomeController.cs:269-458` — same Where clause repeated for each role | Maintenance burden |
| Manual property mapping | 30+ lines | `HomeController.cs:2755-2783` | Should use AutoMapper or similar |
| Magic strings for department codes | 5+ | `Partner_IntegrationController.cs:186-200` (`"HOPP"`, `"HOB"`, `"HODB"`, `"HOISG"`, `"HOITDRM"`) | Error-prone, no compile-time safety |
| Crypto stubs pretending to encrypt | 2 methods | `Models/AppEDCrypto.cs` — `Encrypt`/`Decrypt` return input unchanged | Security smell, misleads auditors |
| Commented-out code blocks | Dozens | `HomeController.cs:24,31,642-643,772-775,884-887,944-947,1183-1186,2533,2614,3969`; `Partner_IntegrationController.cs:82-83,87,574,582-586`; `startupclass.cs:37-42,66-68,72-75,85,89-97` | Noise, confuses readers |
| Overloaded action methods | 3 pairs | `HomeController.cs` — `newintegration` (GET+POST), `search` (GET+POST), `Appprovalintegration` (GET+POST) with ambiguous routing | Potential routing conflicts |

### Security Concerns

| Issue | Location | Severity |
|---|---|---|
| Crypto no-ops | `Models/AppEDCrypto.cs` — Encrypt/Decrypt are identity functions | HIGH |
| Hardcoded connection string parameters | `Program.cs:37` and `startupclass.cs:65` contain encrypted DB credential keys | MEDIUM |
| Static SQL connection | `CommonController.cs` uses static `SqlConnection` outside DI | MEDIUM |
| SQL queries | Parameterized queries appear to be used consistently (no SQL injection found) | OK |

---

## 5. Refactoring Priority Matrix

Priorities are ordered by **(Business Impact x Complexity Risk) / Effort**.

### P0: CRITICAL (Do First)

#### 5.1 Decompose HomeController.cs

**Current state:** 4,887 LOC, 48 methods, 5+ unrelated domains in one file.
**Target state:** ~800 LOC dashboard controller + extracted services and controllers.

| Extract To | Methods to Move | Est. LOC |
|---|---|---|
| `IntegrationService` (shared service) | `newintegration` (GET+POST), `Appprovalintegration` (GET+POST), `NewIntegrationList`, `newintegrationDetails`, `RemoveListData`, `UpdateWorkflowIntegrationservice`, `DeletePriviousdocinupdate`, `SetFilePath`, `GetQuestonDetail` | ~2,800 |
| `ApiTestController` | `search` (GET+POST), `TestAPI` (x2), `apiDetails`, `SearchFilter`, `GetAutoTestApi`, `GetRequestRecord` | ~600 |
| `FileController` | `DownloadFile`, `DownloadFileOBP`, `DownloadIntegrationFiles`, `DownloadZip`, `DisplayFile`, `UploadFile` | ~250 |
| `AutocompleteController` | `AutoFilterExistingServiceName`, `GetExistingServiceRecordById`, `AutoExternalServiceNameFilter`, `GetExternalServiceNameRecordById`, `AutoProducerAppFilter`, `GetProducerAppId`, `GetProducerAppfullName`, `AutoConsumerAppFilter`, `GetConsumerAppId`, `GetConsumerAppText`, `GetBTGPorjectMngr` | ~300 |
| `ExcelExportService` (shared service) | `Table_Export_IntegratedXL`, `CaptureProductivityDetails` | ~500 |

Extracting `Table_Export_IntegratedXL` into a shared service also resolves the `JIRACreatorController -> HomeController` anti-pattern.

#### 5.2 Delete Confirmed Dead Code

Remove the 12 dead artifacts listed in Tier 1 of Section 2. Zero-risk cleanup.
Remove `IPartnerApprovalRepository` registration from `Program.cs`.

### P1: HIGH

#### 5.3 Consolidate Partner Integration Controllers

`Partner_IntegrationController` (legacy, 661 LOC, 28 methods) and `PartnerIntegrationController` (new, 451 LOC, 16 methods) both serve active views.

**Steps:**
1. Migrate remaining legacy views from `Views/Partner_Integration/` (10 files) to `Views/PartnerIntegration/`
2. Update 30+ `Url.Action("...", "Partner_Integration")` references across views
3. Move unique legacy methods into `PartnerIntegrationController`
4. Delete `Partner_IntegrationController.cs` and `Views/Partner_Integration/`

#### 5.4 Split the NewIntegration God Object

Break `NewIntegration.cs` (268 LOC, 95+ properties) into focused DTOs:

| New Class | Properties | Purpose |
|---|---|---|
| `IntegrationMetadata` | IDs, status, dates, workflow fields | Core integration record |
| `IntegrationFiles` | File names, paths, upload fields | Document management |
| `IntegrationDropdowns` | 18 `List<SelectItem>` properties | UI binding only |
| `IntegrationSurvey` | QuestionList, QDropValuesList, answer fields | Q&A/assessment data |

### P2: MEDIUM

#### 5.5 Standardize Repository Layout

Currently two patterns coexist:
- `Repositories/Interfaces/` + `Repositories/Implementations/` (only `IAdminRepository`/`AdminRepository`)
- `Models/` (all 10 other repository interfaces + implementations mixed with 30+ model classes)

Consolidate to one location. The `Models/` folder has 40+ files mixing domain models, view models, repositories, interfaces, crypto, and utilities.

#### 5.6 Fix Crypto Stubs

`Models/AppEDCrypto.cs` has `Encrypt`/`Decrypt` methods that return input unchanged. Either implement real encryption or remove the facade to avoid misleading auditors and code reviewers.

#### 5.7 Merge Duplicate Approval Models

`PartnerApprovalNew.cs` (86 LOC) and `PartnerApprovalModel.cs` (105 LOC) have nearly identical structures with overlapping properties (HOPP_FH, HOPP_VH, etc.). Merge into one canonical model and delete the empty `PartnerApproval.cs` stub.

#### 5.8 Resolve Stub Implementations

Decide on `AdminRepository` (all methods return null) and `SendEmail` (all methods return false): implement them or remove them along with their interfaces and calling code.

### P3: LOW

#### 5.9 Extract Constants for Department Codes

Replace magic strings `"HOPP"`, `"HOB"`, `"HODB"`, `"HOISG"`, `"HOITDRM"` in `Partner_IntegrationController.cs` with an enum or constants class.

#### 5.10 Rename JIRACreaterController.cs

File name `JIRACreaterController.cs` does not match class name `JIRACreatorController`. Rename file to match.

#### 5.11 Fix or Remove CommonController

- Fix namespace from `OneViewIndicator.Controllers` to `API_HUNT.Controllers`
- Replace dead `Startup.connectionstring` reference with injected `IDbConnectionFactory`
- Or remove entirely if `SessionExpiry` is unused

#### 5.12 Extract Duplicate File Path Logic

The file path construction pattern in `HomeController.cs`:
```csharp
Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\APIHuntDoc\\NewIntegrations\\" + "API" + date + id + "\\")
```
Appears 15+ times with minor variations. Extract to a single helper method.

#### 5.13 Remove Commented-Out Code

Dozens of commented-out blocks across `HomeController.cs`, `Partner_IntegrationController.cs`, and `startupclass.cs`. These add noise and confuse readers. Git history preserves the original code if needed.

---

## 6. Refactoring Roadmap

```
Phase 1: Quick Wins (Hours)
  |-- Delete 12 dead files (Tier 1, Section 2)
  |-- Remove IPartnerApprovalRepository from Program.cs
  |-- Rename JIRACreaterController.cs to JIRACreatorController.cs
  |-- Remove commented-out code blocks across codebase
  |-- Fix CommonController namespace

Phase 2: HomeController Decomposition (Days)
  |-- Extract ExcelExportService (resolves JIRACreator anti-pattern)
  |-- Extract IntegrationService (~2,800 LOC)
  |-- Create ApiTestController (~600 LOC)
  |-- Create FileController (~250 LOC)
  |-- Create AutocompleteController (~300 LOC)
  |-- HomeController reduced to ~800 LOC

Phase 3: Consolidation (Days)
  |-- Merge Partner_IntegrationController into PartnerIntegrationController
  |-- Migrate 10 legacy views, update 30+ Url.Action references
  |-- Split NewIntegration god object into 4 focused DTOs
  |-- Merge PartnerApprovalNew + PartnerApprovalModel

Phase 4: Structural Cleanup (Week)
  |-- Standardize repository location (pick one pattern)
  |-- Implement or remove AdminRepository stubs
  |-- Implement or remove SendEmail stubs
  |-- Fix or remove crypto no-ops in AppEDCrypto.cs
  |-- Extract department code constants to enum
  |-- Extract file path construction helper
```

---

## Appendix: Codebase Statistics

| Metric | Value |
|---|---|
| Total C# source files | 70 |
| Total C# LOC | 9,892 |
| Controllers | 13 |
| Repository interfaces | 11 |
| Repository implementations | 11 |
| Model/DTO classes | ~25 |
| View folders | 14 |
| Dead files (confirmed) | 12 |
| Stub implementations | 4 |
| Largest file | HomeController.cs (4,887 LOC, 49.4%) |
| Largest method | newintegration POST (1,315 LOC) |
| Classes >15 public methods | 3 |
