## Phase 01 – Solution Foundation & Repo Hygiene

### Scope
- Create two independent C# REST APIs following Clean Architecture:
  - Vehicles API (Endpoint 1)
  - Insurances API (Endpoint 2)
- Create separate test projects for each API (unit + in-memory integration test projects)
- Solution structure, shared tooling, editorconfig, baseline README skeleton
- Initial GitHub Actions workflow (build + test)

### Detailed Steps

1) **Solution and Projects Structure**
   ```
   ThreadPilot.sln
   ├── src/
   │   ├── Vehicles/
   │   │   ├── ThreadPilot.Vehicles.Api/
   │   │   ├── ThreadPilot.Vehicles.Application/
   │   │   ├── ThreadPilot.Vehicles.Domain/
   │   │   └── ThreadPilot.Vehicles.Infrastructure/
   │   └── Insurances/
   │       ├── ThreadPilot.Insurances.Api/
   │       ├── ThreadPilot.Insurances.Application/
   │       ├── ThreadPilot.Insurances.Domain/
   │       └── ThreadPilot.Insurances.Infrastructure/
   └── tests/
       ├── Vehicles/
       │   ├── ThreadPilot.Vehicles.UnitTests/
       │   └── ThreadPilot.Vehicles.IntegrationTests/
       └── Insurances/
           ├── ThreadPilot.Insurances.UnitTests/
           └── ThreadPilot.Insurances.IntegrationTests/
   ```

2) **Project Configuration**
   - Target .Net 9.0, enable nullable reference types, treat warnings as errors
   - Reference graph: API → Application → Domain; Infrastructure referenced by Application through interfaces
   - Add Directory.Build.props for common settings (analyzers, nullable, version, etc.)
   - Configure EditorConfig for consistent formatting and style rules

3) **Test Projects Setup**
   - **Package references**: xUnit, Shouldly, AutoFixture.AutoBogus, Microsoft.AspNetCore.Mvc.Testing
   - **Unit test assemblies**: for Application and Domain layers with isolated testing
   - **Integration test assemblies**: for API layer using WebApplicationFactory with in-memory services
   - **Test utilities**: shared test builders, fixtures, and helper methods for consistent test data

4) **CI Skeleton**
   - GitHub Actions workflow: dotnet restore/build/test on PR and push to main
   - Basic caching for NuGet packages to improve build performance
   - Fail-fast strategy with clear error reporting

5) **Repository Hygiene**
   - .gitignore for .NET projects (bin/, obj/, .vs/, etc.)
   - CONTRIBUTING.md with development guidelines and PR process
   - Solution README skeleton with project overview and quick start guide

### Deliverables
- Compiling solution with four projects per API + test projects
- Initial tests pass (placeholder assertions) to exercise workflow
- CI workflow running and passing

### Quality Assurance & Tests
- Build validation: dotnet build succeeds for all projects
- Test validation: dotnet test runs all placeholder tests and passes
- Static checks: analyzers enabled, stylecop/fxcop optional, nullable enabled
- CI validation: PRs trigger pipeline; red/green feedback observed

### Entry Criteria
- Agreement on Clean Architecture layout and naming conventions

### Exit Criteria (Gate to Phase 02)
- Solution builds successfully in CI
- Baseline tests pass in CI
- README skeleton present with project map

