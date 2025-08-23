# ThreadPilot Project Context

## Project Overview

ThreadPilot is a .NET solution designed as an integration layer scaffolding for Vehicles and Insurances APIs. It follows Clean Architecture principles, separating concerns into distinct layers (Domain, Application, Infrastructure, Api) for each bounded context (Vehicles, Insurances). This structure promotes modularity, testability, and maintainability.

The solution is built using modern .NET practices, including:
- .NET 9.0 (as per CI configuration)
- Minimal APIs for the web layer
- Serilog for structured logging
- OpenTelemetry for tracing and metrics
- Authentication/Authorization with JWT Bearer tokens (toggleable)
- Health checks
- Swagger/OpenAPI for API documentation (in Development)
- Dependency injection and configuration options

## Building and Running

The project uses standard .NET CLI commands for building and testing, as indicated in the `README.md` and CI workflow.

- **Build:** `dotnet build`
- **Test:** `dotnet test`
- **Restore Dependencies:** `dotnet restore` (usually run automatically by build/test, but useful for initial setup)
- **Run (individual API):**
  - Navigate to the API project directory (e.g., `src\Vehicles\ThreadPilot.Vehicles.Api`).
  - Run `dotnet run`.
  - Access the API (e.g., `http://localhost:5000` or the port specified by the application). Swagger UI will be available at `/swagger` if running in Development environment.

The CI pipeline (`.github/workflows/ci.yml`) uses `dotnet restore`, `dotnet build --configuration Release --no-restore`, and `dotnet test --configuration Release --no-build` on Windows.

## Development Conventions

Based on the files analyzed, the following conventions and practices are used:

- **.NET Project Structure:** Projects follow a standard .NET structure with `Program.cs` as the entry point. Common properties like `TreatWarningsAsErrors`, `Nullable`, `ImplicitUsings`, and `LangVersion` are enforced via `Directory.Build.props`.
- **Clean Architecture:** The solution is organized into Domain, Application, Infrastructure, and Api layers for each feature area (Vehicles, Insurances). This separation helps manage dependencies and responsibilities.
- **Dependency Injection:** Services are registered with the DI container in `Program.cs` using `builder.Services`.
- **Configuration:** Strongly-typed options are used, bound from configuration sections (e.g., `SecurityOptions`).
- **Logging:** Serilog is used for structured, enriched logging, including correlation IDs and service names.
- **Authentication/Authorization:** JWT Bearer authentication is implemented with a toggle (`SecurityOptions.Enabled`) to easily switch it on/off, which is helpful for development or internal services. Policies like "ReadAccess" and "WriteAccess" are defined.
- **Observability:** OpenTelemetry is integrated for application tracing and metrics, aiding in monitoring and debugging.
- **Health Checks:** Basic health check endpoints (`/health/live`, `/health/ready`) are implemented.
- **Testing:** Unit and Integration tests are planned/structured for each feature area, although specific test content wasn't analyzed.
- **API Documentation:** Swagger/OpenAPI is included for API exploration and documentation during development.