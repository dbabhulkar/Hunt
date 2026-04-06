# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Hunt** is an ASP.NET Core 8.0 MVC web application that manages partner lifecycle workflows. It handles partner onboarding, offboarding, integration approvals, exception management with multi-level approvals, and JIRA integration for issue tracking.

## Build & Run Commands

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run

# Development with HTTPS on port 7044
dotnet run --launch-profile https

# Development with HTTP on port 5089
dotnet run --launch-profile http
```

The application runs at:
- HTTP: `http://localhost:5089`
- HTTPS: `https://localhost:7044`

## Architecture

### High-Level Structure

- **Controllers** (15+ controllers): Handle different workflows and user roles
  - Partner workflows: Onboarding, Offboarding, Approval, Integration, Dashboard
  - Exception Management: Multiple approval levels (Level 1-3)
  - Admin & Login: User and system management
  - JIRA integration: Create and track issues

- **Models**: Business logic and data access
  - Repository pattern: `*Repository.cs` classes handle database operations
  - Domain models: `Partner*`, `Exception*`, `*Model.cs` classes represent domain entities
  - Utilities: `AESEncrytDecry`, `SendEmail`, `GetConnectionString` for cross-cutting concerns

- **Views**: Razor templates organized by controller
  - Multiple versions of views (e.g., `PartnerOnboarding` and `PartnerOnboardingNew`) suggest active refactoring
  - `_Layout.cshtml` provides main layout structure
  - Partial views like `_FeedbackPartial.cshtml` reused across workflows

### Key Technical Patterns

1. **Database Access**: Direct SqlConnection/SqlCommand usage (not EF Core)
   - Connection strings managed via `Startup.connectionstring` configuration
   - Models use repository pattern for data access
   - SQL Server required

2. **Authentication & Authorization**: Session-based
   - User role checked via `HttpContext.Session.GetString("Role")`
   - Roles include: USER, APPROVER, ADMIN
   - Custom `[CustomFilter]` attribute used for role-based filtering

3. **File Uploads**: Partners can upload documents
   - Upload folder: `wwwroot/UploadPO/` (created at runtime if missing)
   - Handled in constructors of controllers managing uploads

4. **Data Export**: EPPlus used for Excel generation
   - Supports exporting partner lists and approval data

5. **External Integrations**:
   - JIRA API: `JIRARepository` handles issue creation and sync
   - Email: `SendEmail` utility for workflow notifications
   - HttpClient for API calls

### Code Organization Observations

- Some legacy code remnants from an older `API_HUNT` project (visible in HomeController and some controller usings)
- Multiple controller variants suggest incremental refactoring (e.g., `PartnerIntegration` + `PartnerIntegrationNew`)
- Empty placeholder classes (`DBClass`, `GetConnectionString`, `PartnerRepository`) suggest ongoing development

## Configuration

- **appsettings.json**: Minimal config (logging, allowed hosts)
- **appsettings.Development.json**: Development-specific overrides (not included in repo)
- **launchSettings.json**: Three profiles - HTTP, HTTPS, and IIS Express

## Common Development Tasks

### Adding a New Workflow

1. Create a new Controller in `Controllers/` directory
2. Add corresponding Model/Repository in `Models/` for data access
3. Create views under `Views/{ControllerName}/`
4. Implement role-based access checks using Session.GetString("Role")
5. Use existing Repository classes as template for database patterns

### Modifying Database Operations

- Repository classes handle all data access using SqlConnection
- Avoid mixing data access logic in controllers
- Follow existing pattern: Repository methods return lists or single objects
- Consider using parameterized queries to prevent SQL injection (check existing patterns)

### Working with Views

- Use `_Layout.cshtml` as base for shared UI elements
- Leverage existing partial views (`_FeedbackPartial.cshtml`, etc.)
- Check both versioned and new view folders for latest patterns

## Database

- **Type**: SQL Server
- **Connection**: Configured via `Startup.connectionstring` (implementation not visible in current codebase)
- **Scripts**: DBScripts folder (currently empty; likely migrated elsewhere)

## Notes for Future Development

1. **Code Cleanup**: Several empty model classes and redundant view versions indicate ongoing refactoring—consolidate when possible
2. **Legacy Code**: Mix of Hunt and API_HUNT namespaces; align namespaces during refactoring
3. **Direct SQL**: Consider evaluating migration to Entity Framework for new features
4. **Error Handling**: HomeController references a `Startup` class not visible in project structure—verify configuration is properly set up
5. **Testing**: No test projects detected; consider adding unit/integration tests for critical workflows
