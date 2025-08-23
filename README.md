# ThreadPilot

Integration layer scaffolding for Vehicles and Insurances APIs following Clean Architecture.

## Structure (Phase 01)

- src/
  - Vehicles/
    - ThreadPilot.Vehicles.Api
    - ThreadPilot.Vehicles.Application
    - ThreadPilot.Vehicles.Domain
    - ThreadPilot.Vehicles.Infrastructure
  - Insurances/
    - ThreadPilot.Insurances.Api
    - ThreadPilot.Insurances.Application
    - ThreadPilot.Insurances.Domain
    - ThreadPilot.Insurances.Infrastructure
- tests/
  - Vehicles/
    - ThreadPilot.Vehicles.UnitTests
    - ThreadPilot.Vehicles.IntegrationTests
  - Insurances/
    - ThreadPilot.Insurances.UnitTests
    - ThreadPilot.Insurances.IntegrationTests

## Quick start

- dotnet build
- dotnet test
