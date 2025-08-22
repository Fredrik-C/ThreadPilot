## Phase 01 – Solution Foundation & Repo Hygiene

### Scope
- Create two independent C# REST APIs following Clean Architecture:
  - Vehicles API (Endpoint 1)
  - Insurances API (Endpoint 2)
- Create separate test projects for each API (unit + in-memory integration test projects)
- Solution structure, shared tooling, editorconfig, baseline README skeleton
- Initial GitHub Actions workflow (build + test)

### Detailed Steps
1) Solution and Projects
- Create a solution with folders per service: src/Vehicles, src/Insurances, tests/Vehicles, tests/Insurances
- For each API: API (presentation), Application, Domain, Infrastructure projects
- Reference graph: API -> Application -> Domain; Infrastructure referenced by Application through interfaces
- Add common analyzers/style (EditorConfig), nullable enable, warnings as errors where reasonable

2) Test Projects
- xUnit, Shouldly, AutoFixture.AutoBogus packages
- Unit test assemblies for Application and Domain
- Integration test assemblies for API (WebApplicationFactory) using in-memory services

3) CI Skeleton
- GitHub Actions: dotnet restore/build/test on PR and push

4) Repo Hygiene
- .gitignore, minimal CONTRIBUTING notes, solution README skeleton

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

