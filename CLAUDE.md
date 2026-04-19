# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Hunt** (namespace: `API_HUNT`) is an internal ASP.NET Core 8.0 MVC web application for API partner lifecycle management — onboarding, integration, approval, offboarding, exception handling, and health diagnostics. It integrates with Active Directory for authentication and JIRA for ticket management.

## Build and Run

```bash
dotnet build Hunt.csproj        # Build
dotnet run                      # Run (http://localhost:5089)
dotnet watch run                # Run with hot reload
```

The solution (`Hunt.sln`) contains three projects: `Hunt` (main app), `Hunt.Tests`, and `Hunt.UITests` (test projects live one level up at `../Hunt.Tests/` and `../Hunt.UITests/`).

## Architecture

### MVC + Repository Pattern

- **Controllers/** — Handle HTTP requests, delegate to repositories. `CustomFilter` attribute enforces session-based auth.
- **Models/** — Contains domain models, view models, AND repository implementations (most repositories live here, not in `Repositories/`).
- **Repositories/** — Only `IAdminRepository`/`AdminRepository` use the formal Interfaces/Implementations folder structure. All other repository interfaces and implementations are in `Models/`.
- **Views/** — Razor views organized by controller. Shared layout in `Views/Shared/_Layout.cshtml`.

### Data Access

Uses **raw ADO.NET** (not EF Core). Database is **MySQL** via MySqlConnector, but code uses `SqlCommand`/`SqlDataAdapter` patterns.

- Connection string built in `Program.cs` via `ConnectionDB.getConString()` (defined in `Models/DataBaseConnection.cs`) with encrypted credentials.
- Connection factory: `IDbConnectionFactory` / `DbConnectionFactory` — injected into repositories.
- Data pattern: `SqlDataAdapter` → `DataSet` → iterate `DataTable.Rows`. Parameters use `@p_` prefix convention.

### Authentication Flow

1. Active Directory/LDAP validation (`LDAP://ldap.hunt.com`)
2. `UserMaster` table status check (Active/Locked/Dormant/Disabled)
3. Session variables set: `EmpId`, `UserName`, `UserRole`, `Role`, `LoginTime`, `Logid`
4. `CustomFilter` action filter (in `Models/CustomFilter.cs`) checks session on protected controllers
5. Session timeout: 30 minutes idle

### Dependency Injection

All repositories registered as `AddScoped` in `Program.cs`. `HomeController` is registered as `AddTransient` because `JIRACreatorController` depends on it directly.

### Key Modules

| Controller | Purpose |
|---|---|
| `LoginController` | Auth, session management |
| `HomeController` | Main dashboard, health diagnostics (Diagnose), Excel export |
| `AdminController` | User/app master CRUD |
| `PartnerOnboardingController` | Partner onboarding workflow |
| `PartnerIntegrationController` | API integration management |
| `PartnerApprovalController` | Approval workflows |
| `PartnerOffboardingController` | Partner offboarding |
| `ExceptionManagementController` | Exception CRUD |
| `ExceptionApprovalController` | Exception approval workflow |
| `JIRACreaterController` | JIRA ticket creation/export |
| `PartnerDashboardController` | Partner analytics dashboard |

### Key Libraries

- **EPPlus** — Excel file generation/export
- **Newtonsoft.Json** — JSON serialization
- **System.DirectoryServices** — Active Directory/LDAP auth
- **MySqlConnector** — MySQL database driver

### Default Route

`{controller=Login}/{action=Index}/{id?}` — app starts at the login page.
