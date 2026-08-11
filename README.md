# School ERP RD

School ERP RD is the technical foundation for a multi-tenant SaaS ERP serving private schools in the Dominican Republic. Phase 1.2 contains infrastructure only—no school business features.

## Architecture

- .NET 10 / ASP.NET Core 10 modular monolith
- Angular 22 with strict TypeScript
- Entity Framework Core 10
- Azure SQL Database in production
- Azure App Service, Key Vault, and Managed Identity direction
- OpenTelemetry with optional Azure Monitor/Application Insights export
- GitHub Actions CI

The dependency direction is `Api → Application → Domain`, with `Api → Infrastructure` and `Infrastructure → Application + Domain`. Domain has no framework or infrastructure dependency.

## Prerequisites

- .NET SDK 10.0.302 or a compatible 10.0 patch
- Node.js 24.15+ and npm 11+
- SQL Server Developer/Express, or another SQL Server-compatible local instance
- HTTPS development certificate: `dotnet dev-certs https --trust`

Docker is not required for the application. CI uses one ephemeral SQL Server container for isolated relational tests.

## Setup

```powershell
dotnet tool restore
dotnet restore SchoolERP.sln
cd src\SchoolERP.Web
npm ci
cd ..\..
```

No real secret belongs in a committed file. Override local settings with environment variables or .NET user secrets. For example:

```powershell
dotnet user-secrets set "ConnectionStrings:SchoolERP" "<local connection string>" --project src\SchoolERP.Api
```

Production supplies `ConnectionStrings__SchoolERP` through App Service/Managed Identity-compatible configuration. Set `APPLICATIONINSIGHTS_CONNECTION_STRING` only in the target environment to enable Azure Monitor export.

## Database

The committed development default uses Windows authentication with `localhost\SQLEXPRESS` and database `SchoolERP_Development`. It contains no password. Override `ConnectionStrings__SchoolERP` when using another local SQL instance.

The DbContext intentionally has no business `DbSet`. The initial migration is empty; applying it creates only EF Core's technical migration-history table.

## Migrations

List and apply migrations:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet tool run dotnet-ef migrations list --project src\SchoolERP.Infrastructure --startup-project src\SchoolERP.Api
dotnet tool run dotnet-ef database update --project src\SchoolERP.Infrastructure --startup-project src\SchoolERP.Api
```

Create a later approved migration:

```powershell
dotnet tool run dotnet-ef migrations add <MeaningfulName> --project src\SchoolERP.Infrastructure --startup-project src\SchoolERP.Api --output-dir Persistence\Migrations
```

Production migrations are an explicit deployment operation; they do not run automatically on application startup.

## Backend

```powershell
dotnet run --project src\SchoolERP.Api --launch-profile https
```

Local endpoints:

- `https://localhost:7080/health/live` — process liveness, independent of SQL
- `https://localhost:7080/health/ready` — readiness including SQL connectivity
- `https://localhost:7080/openapi/v1.json` — development-only OpenAPI document

## Frontend

In another terminal:

```powershell
cd src\SchoolERP.Web
npm start
```

Open `http://localhost:4200`. The development proxy forwards `/health` to the HTTPS API at port 7080.

## Tests

Backend tests create and remove a uniquely named `SchoolERP_IntegrationTests_<guid>` database on local SQL Express. To use another instance, set `SCHOOLERP_TEST_SQL_CONNECTION_STRING` outside source control; the database name in that base connection is replaced for the migration test.

```powershell
dotnet restore SchoolERP.sln
dotnet build SchoolERP.sln
dotnet test SchoolERP.sln
```

Frontend validation:

```powershell
cd src\SchoolERP.Web
npm ci
npm run lint
npm test
npm run build
```

## Dependency security pins

- `Microsoft.OpenApi` is referenced directly by the API to keep the ASP.NET OpenAPI
  dependency graph on the patched 2.11.0 release.
- npm overrides `@hono/node-server` to 2.1.0 because Angular CLI's development tooling
  reaches it transitively through the Model Context Protocol SDK.

Keep these pins until their parent packages resolve to equally patched or newer versions.
Validate any change with `dotnet list package --vulnerable --include-transitive` and
`npm audit` in addition to the normal build and test commands.

## Documentation

- [Agent instructions](AGENTS.md)
- [Project memory](PROJECT_MEMORY.md)
- [System Design Document](docs/SDD.md)
- [Technical Architecture Gate 1.1](docs/architecture/technical-architecture-gate-1.1.md)
- [Phase 1.2 execution contract](docs/requirements/phase-1.2-execution-prompt.md)
- [Phase 1.3 review gate](docs/architecture/bootstrap-architecture-review-gate-1.3.md)
- [Architecture decisions](docs/architecture/adr/)
