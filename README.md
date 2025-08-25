# ThreadPilot

An integration layer between a new core system (named ThreadPilot) and multiple legacy systems.

## Overview

- Services
  - ThreadPilot.Vehicles.Api: exposes vehicle info by Swedish registration number; currently uses a stubbed provider for a legacy system.
  - ThreadPilot.Insurances.Api: returns insurances for a Swedish personal ID and enriches vehicle products via Vehicles API.
- Ports (default)
  - Vehicles API: 5123 (docker), 5193 (dotnet run)
  - Insurances API: 5261 (docker and dotnet run)
- Health endpoints
  - GET /health/live
  - GET /health/ready
- OpenAPI (Development only)
  - /swagger (UI) and /swagger/v1/swagger.json

## Run locally

Prerequisites: .NET 9 SDK, Docker Desktop (for compose).

Option A: docker-compose (recommended)

1) docker compose up --build -d
   - Vehicles: <http://localhost:5123>
   - Insurances: <http://localhost:5261>
2) Open <http://localhost:5123/swagger> and <http://localhost:5261/swagger>
3) Health checks: <http://localhost:5123/health/ready> and <http://localhost:5261/health/ready>
4) Smoke test: PowerShell scripts/compose.ps1 -cmd smoke
5) Tear down: docker compose down -v --remove-orphans

Option B: dotnet run (both services)

- Terminal 1
  - cd src/Vehicles/ThreadPilot.Vehicles.Api
  - dotnet run
  - Serves on <http://localhost:5193>
- Terminal 2
  - cd src/Insurances/ThreadPilot.Insurances.Api
  - dotnet run
  - Serves on <http://localhost:5261>
- Ensure Insurances points to Vehicles:
  - In src/Insurances/.../appsettings.json set "VehiclesApi": { "BaseAddress": "<http://localhost:5193>" }

Run tests

- dotnet build
- dotnet test

## New-developer dry-run checklist

- Clone, dotnet build, dotnet test
- docker compose up --build -d; verify /health/ready both services
- Hit sample endpoints listed above and confirm responses
- Open /swagger for both services

## Endpoints and samples

### Vehicles

- GET /api/vehicles/{registrationNumber}
  - Sample OK:

    ```bash
    curl http://localhost:5123/api/vehicles/ABC123
    ```

    ```json
    { "regNo": "ABC123", "make": "Volvo", "model": "V60", "year": 2019, "fuelType": "Petrol" }
    ```
  - Not found:

    ```bash
    curl http://localhost:5123/api/vehicles/XYZ789
    ```

    => 404 ProblemDetails with code "NotFound"

### Insurances

- GET /api/insurances/{personalId}
  - Sample OK (enriched):

    ```bash
    curl http://localhost:5261/api/insurances/640823-3234
    ```

    ```json
    [
      { "product": {"name":"Home Insurance","price":30.0,"terms":"Standard terms"}, "vehicleRegNo": null, "vehicle": null },
      { "product": {"name":"Car Insurance","price":20.0,"terms":"Comprehensive coverage"}, "vehicleRegNo": "ABC123", "vehicle": {"regNo":"ABC123","make":"Volvo","model":"V60","year":2019,"fuelType":"Petrol"} },
      { "product": {"name":"Pet Insurance","price":10.0,"terms":"For dogs and cats"}, "vehicleRegNo": null, "vehicle": null }
    ]
    ```
  - Empty:

    ```bash
    curl http://localhost:5261/api/insurances/19850515-5678
    ```

    => 404 ProblemDetails with code "NotFound"

### Configured stub triggers (whitelisted values)

- Vehicles (appsettings): ABC123=Success, XYZ789=NotFound, ERR123=Error, TMO123=Timeout, SLO123=Slow
- Insurances (appsettings): 640823-3234=Success, 19850515-5678=Empty, 19701231-9999=Timeout

## Configuration

Security (OAuth) toggle

- appsettings.json: "Security": { "Enabled": true|false, "Authority": "...", "Audience": "..." }
- In Development, Security.Enabled is false by default. Docker compose sets Security__Enabled=false for both services.
- When enabled, policies are registered: "ReadAccess" (role Reader) and "WriteAccess" (role Writer).

Vehicles API base address for Insurances

- appsettings in Insurances: "VehiclesApi": { "BaseAddress": "<http://localhost:5123>" } for docker, "<http://localhost:5193>" for dotnet run.

Feature flags (ready-to-wire)

- Options class: Features section => FeatureFlagsOptions (Flags: { name: bool })
- Providers: AppSettingsFeatureToggleProvider, RemoteFeatureToggleProvider; aggregator: CompositeFeatureToggle
- DI example (add to Program.cs):

  ```csharp
  builder.Services.Configure<FeatureFlagsOptions>(builder.Configuration.GetSection(FeatureFlagsOptions.SectionName));
  builder.Services.AddSingleton<IFeatureToggleProvider, AppSettingsFeatureToggleProvider>();
  builder.Services.AddSingleton<IFeatureToggle>(sp => new CompositeFeatureToggle(sp.GetServices<IFeatureToggleProvider>()));
  ```

- Usage example (service):

  ```csharp
  public class ExampleService(IFeatureToggle flags)
  {
      public bool DoX() => flags.IsEnabled("X");
  }
  ```

## Error handling (ProblemDetails)

- Both APIs use a global exception middleware and map service results to RFC7807 ProblemDetails.
- Stable extensions added:
  - code: a machine-readable error code (e.g., InvalidPersonalId, NotFound, Timeout)
  - traceId: current trace identifier (OpenTelemetry)
  - correlationId: value of X-Correlation-ID if provided
- Example 400 (Vehicles, invalid reg):

  ```json
  { "title":"Invalid Registration Number", "status":400, "type":"https://tools.ietf.org/html/rfc7231#section-6.5.1", "extensions": { "code":"InvalidRegistrationNumber", "traceId":"...", "correlationId":"..." } }
  ```

## Observability

- Structured logging: Serilog emits compact JSON to console with Service and CorrelationId properties.
- OpenTelemetry: tracing + metrics for ASP.NET Core and HttpClient, console exporters enabled for Development.
- Correlation: X-Correlation-ID request header is propagated to responses and logs.

## Architecture notes

- Clean Architecture layers per service
  - Api: HTTP, controllers, model binding, OpenAPI, exception handling, auth, DI.
  - Application: use cases, contracts, services, feature abstractions.
  - Domain: business models, validators, exceptions/value objects.
  - Infrastructure: external providers/clients (currently stubs), feature toggle providers.
- Data model highlights
  - Insurances: Product(Name, Price, Terms) and Insurance(Product, VehicleRegNo?)
  - Vehicles: Vehicle(RegNo, Make, Model, Year, FuelType)
- Dependency rule: Api depends on Application and Domain; Infrastructure depends on Application and Domain; Application depends only on Domain.

## Security approach

- Toggleable OAuth via Security.Enabled. When enabled, JWT bearer is configured; policies ReadAccess/WriteAccess are registered.
- Token forwarding: An AuthPropagationHandler exists in Insurances API to forward Authorization to downstream calls. To enable, add:

  ```csharp
  builder.Services.AddHttpClient<VehiclesApiClient, VehiclesApiClient>().AddHttpMessageHandler<AuthPropagationHandler>();
  builder.Services.AddHttpContextAccessor();
  ```

## Extensibility and versioning

- Extend via new controllers/handlers in Api and new services/providers behind Application interfaces.
- Prefer backwards-compatible contract evolution (additive changes). Introduce new versions only when breaking changes are unavoidable.

## Test Strategy

To ensure the quality, reliability, and maintainability of the ThreadPilot services, a layered testing approach is essential. Here's how different types of tests should be approached:

### Unit Tests

*   **Focus**: Isolate and test individual units of code, typically methods within classes in the Application and Domain layers.
*   **Boundaries**: Mock or stub external dependencies such as `IVehicleInfoProvider`, `IInsuranceProvider`, HTTP clients, databases, or file systems. The goal is to test the logic *within* the unit, not the interaction with dependencies.
*   **Frameworks**: Use standard .NET testing frameworks like xUnit, NUnit, or MSTest. Leverage mocking libraries like Moq or NSubstitute.

### Integration Tests

*   **Focus**: Test the interaction between integrated components within a single service, particularly how the Application layer interacts with the Infrastructure layer.
*   **Boundaries**: Tests should involve real implementations of dependencies where feasible (e.g., using an in-memory database instead of a real one, or using the stub providers with specific configurations). Avoid mocking internal units tested in isolation by unit tests. For example, test the `VehicleService` using the actual `StubVehicleInfoProvider` configured for specific scenarios.
*   **Scope**: Validate data flow, configuration binding, and the correctness of complex queries or operations that span multiple classes but stay within the service boundary.

### End-to-End (E2E) Tests (Future Implementation)

*   **Focus**: Validate the entire flow of a user scenario across multiple services, from the API endpoint down to the (stubbed) external system calls and back.
*   **Implementation Plan**:
    *   Utilize the existing Docker Compose setup to spin up both services in a known state.
    *   Write dedicated test suites (e.g., using .NET test projects with libraries like `Microsoft.AspNetCore.Mvc.Testing` or external tools like Playwright/Cypress for API interactions) that send real HTTP requests to the service endpoints.
    *   Configure the stubs via `appsettings` or direct API calls (if a configuration endpoint is added) to simulate various scenarios (success, failure, timeouts) for the legacy system interactions.
    *   Assertions should cover the final response, ensuring the system behaves correctly end-to-end, including the integration between Insurances and Vehicles APIs.

### Performance Tests (Future Implementation)

*   **Focus**: Assess the responsiveness, throughput, and stability of the services under various load conditions.
*   **Implementation Plan**:
    *   Use tools like k6, JMeter, or Locust to generate load.
    *   Target the Docker Compose environment or a staging environment.
    *   Define test scenarios based on expected real-world usage patterns (e.g., concurrent users fetching vehicle info, requesting insurance lists).
    *   Include tests for resilience by configuring stubs to simulate slow or error responses and observing how the system handles them (e.g., with timeouts and circuit breakers once implemented).
    *   Monitor key metrics like response time, error rate, and resource utilization (CPU, memory).

## Next steps / future work

- The legacy vehicle system integration protocol (HTTP/AMQP/FTP/other) is not yet defined, therefore resilience policies (timeouts, retries with backoff, circuit breaker) are not implemented.
Resilience policies should be added when protocol is known.
- Consider shared building blocks (e.g., FeatureToggleProvider) across services while preserving service autonomy.

## Previous experience of similar projects

At my current assignment we have migrated from a monolith consising of 3 major services to domain focused microservices.
We used a "strangler pattern" approach where we created new services and gradually migrated the functionality from the monolith to the new services.

## Challanges

Figuring out a good enough approach to test how the api react to downstream failures.