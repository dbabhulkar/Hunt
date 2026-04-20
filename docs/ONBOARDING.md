# Developer Onboarding Guide

## Quick Start

```bash
# Prerequisites: .NET 8 SDK, MySQL 8.0+
dotnet build Hunt.csproj
dotnet run                # http://localhost:5089
dotnet watch run          # with hot reload
```

## Local Database Setup

1. Install MySQL 8.0+ and create a database called `hunt`
2. The connection string is in `Models/DataBaseConnection.cs` — update credentials to match your local MySQL
3. The current local config points to `localhost:3306`, database `hunt`, user `root`

**Important**: The production connection string in `Program.cs` passes an encrypted password (`COLD3whn89PtD+SNyCdvwQ==`) to `ConnectionDB.getConString()`. In the current code, `getConString()` ignores these parameters and returns the hardcoded local MySQL string. Production deployment would need the original `getConString()` logic restored.

## Authentication in Local Dev

LDAP/Active Directory authentication is **bypassed** in the local environment. In `LoginController.Index(POST)`, the line `bool isValid = true;` skips AD validation entirely. To log in locally:

1. You need a record in `UserMaster` with a matching `EmpCode`
2. You need a record in `tbl_API_HUNT_USER` with that `EmpCode` and an `IsActive = 1` flag
3. Enter the `EmpCode` as username with any password

## Project Structure Gotchas

### Repository locations are inconsistent
- `AdminRepository` is in `Repositories/Implementations/` with its interface in `Repositories/Interfaces/`
- **All other repositories** are in `Models/` — both interfaces (`I*.cs`) and implementations

### Two partner management systems
The app has a **legacy** and **new** partner system running in parallel:
- Legacy: `Partner_IntegrationController` → `IPartnerRepository` → `PartnerOnboarding` model
- New: `PartnerOnboardingController` + `PartnerIntegrationController` → `IPartnerOnboardingNewRepository` → `PartnerOnboardingNew`/`PartnerIntegrationNew` models

### HomeController is massive
`HomeController.cs` is ~1500+ lines and handles the original integration workflow, diagnostics, API search, Excel export, and more. It is the most complex file in the codebase.

### Naming inconsistencies
- `JIRACreaterController` (typo in filename) vs `JIRACreatorController` (class name)
- `PartnerIntegrationNew` has both `CaseId` and `CaseID` properties
- `status` (lowercase) and `Status` (uppercase) coexist as separate properties on the same model

## Key Database Tables

Create these tables to get the app running locally:

- `UserMaster` — Employee master with EmpCode, EmpName, Active, Locked, Dormant, Enabled, LastLoginDate, LastLogoutDate
- `tbl_API_HUNT_USER` — App users with EmpCode, Role, IsActive
- `TBL_OVI_RM_Hierarchy_Mapping` — Maps EmpCode to EmpRole
- `tbl_API_HUNT_Activity_Log_Tracker` — Audit log
- `tbl_Mofee_Url` — Single-row table with the external portal URL

## Testing

Test projects live **one level up** from the main project:
- `../Hunt.Tests/` — Unit tests
- `../Hunt.UITests/` — UI tests

All three are referenced in `Hunt.sln`.

## Common Tasks

### Adding a new user role
1. Insert into `tbl_API_HUNT_USER` with the desired `Role` value
2. Add role-based routing logic in the relevant controller (check `HttpContext.Session.GetString("Role")`)
3. The `CustomFilter` only checks that a role exists — it does not check *which* role

### Adding a new approval department
1. Add the department code to the approval matrix creation in `PartnerIntegrationController.SavePartnerIntegration()` and `Partner_IntegrationController.SaveAddPartneronBoarding()`
2. Add corresponding properties to the `PartnerIntegrationNew` model (e.g., `NEWDEPT_FH`, `NEWDEPT_VH`, `NEWDEPT_GH` plus status and name fields)
3. Update the view to display the new approval columns

### Understanding the upload directories
| Directory | Controller | What goes here |
|-----------|-----------|----------------|
| `wwwroot/UploadPO/` | Partner_IntegrationController (legacy) | Journey docs, risk sheets, other docs |
| `wwwroot/UploadPONew/` | PartnerOnboardingController | Client profile sheets, risk assessment sheets |
| `wwwroot/UploadPINew/` | PartnerIntegrationController + PartnerApprovalController | Integration journey docs, other docs |
| `wwwroot/APIHuntDoc/NewIntegrations/` | HomeController | Integration workflow documents |
