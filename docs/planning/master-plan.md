## ThreadPilot Integration Layer – Master Plan

> Note on vehicle data source: Vehicle information originates from an external legacy system. In v1 we will not connect to it directly; instead, we define an Infrastructure abstraction and provide a stubbed implementation for local/dev and tests. The Vehicles API owns this integration boundary. The Insurances API must obtain vehicle data only via the Vehicles API (not by calling the legacy stub directly). Tests will use the stubbed provider to simulate found/missing/timeout/error scenarios.


### 1) Objective
Build v1 of an integration layer between the new ThreadPilot core and multiple legacy systems via two independent C# REST APIs that follow Clean Architecture, are observable, secure, tested, containerized, and CI-enabled.

### 2) Guiding Principles
- Clean Architecture: API, Application, Domain, Infrastructure per service
- Separation of concerns: two independent APIs (Vehicles, Insurances) with their own tests and infrastructure
- Contracts as records; rigorous validators; graceful error handling and correct HTTP codes
- Observability first (OpenAPI, Health Checks, Structured Logging, OpenTelemetry)
- Security by default (OAuth), with runtime toggle via appsettings/IOptions
- Persistence via EF Core with DbUp migrations to containerized SQL Server (tests use in-memory)
- Testing culture: xUnit + Shouldly + AutoFixture.AutoBogus (unit and in-memory integration)
- Performance: favor Task.WhenAll for parallel downstream calls
- DevEx: Docker, docker-compose, GitHub Actions (build/test)

### 3) Deliverable Phases (with QA gates between phases)
1. Phase 01 – Solution Foundation & Repo Hygiene
   - Scaffolding: two API projects + test projects; solution layout (Clean Architecture per API)
   - CI skeleton (build/test) and baseline README skeleton
   - QA Gate: solution builds, tests pass (placeholder), lint/format check
2. Phase 02 – Observability & Security Foundations
   - OpenAPI, Health Checks, Structured Logging (console + Elastic-ready), OpenTelemetry
   - OAuth protection w/ appsettings/IOptions toggle, feature-flag hook points
   - QA Gate: /health, swagger available; logs structured; OAuth on/off verified via tests
3. Phase 03 – Vehicle API (Endpoint 1)
   - Swedish registration validator; stubbed infrastructure for external legacy vehicle system
   - Endpoint returns vehicle info; error handling & correct HTTP codes
   - QA Gate: unit + in-memory integration tests incl. edge cases (invalid, not found, timeout)
4. Phase 04 – Insurance API (Endpoint 2)
   - Swedish personal ID validator; stubbed legacy insurance system; integrates with Vehicle API
   - Includes car info for car insurances; concurrency via Task.WhenAll
   - QA Gate: unit + integration tests incl. multiple/no insurances, timeouts, invalid input
5. Phase 05 – Persistence (EF Core + DbUp + SQL Server container)
   - Domain modeling for Products and Insurances; DbUp migrations; EF Core wiring
   - QA Gate: migrations apply idempotently; repository tests with in-memory provider
6. Phase 06 – Containerization & Local Orchestration
   - Multi-stage Dockerfiles; health checks; docker-compose incl. SQL Server
   - QA Gate: local compose up; health and swagger reachable; minimal smoke tests
7. Phase 07 – Resilience & Performance Hardening
   - Timeouts/retries/circuit breakers around downstreams; Task.WhenAll audit; cancellations
   - QA Gate: resilience tests using fakes; perf baseline for concurrent requests
8. Phase 08 – Documentation (README and Architecture Notes)
   - Architecture and design decisions; runbooks for local dev/test; error handling & security approach
   - QA Gate: docs completeness checklist; quickstart validated by a new dev
9. Phase 09 – CI Pipeline Enhancements
   - GitHub Actions build/test matrices; Docker build; artifacts; PR quality gates
   - QA Gate: green pipeline on main; PR template and checks enforced

### 4) Cross-Cutting Requirements Mapping
- OpenAPI: Phases 02, 06 verification
- Health checks: Phases 02, 06
- Structured logging + Elastic-ready: Phase 02
- OpenTelemetry: Phase 02
- OAuth toggle: Phase 02 (+ tested in Phases 03–04)
- Validations (vehicle reg, personal id): Phases 03–04
- EF Core + DbUp + SQL Server: Phase 05
- Docker multi-stage + compose: Phase 06
- Tests (xUnit, Shouldly, AutoFixture.AutoBogus): Across Phases 01–07
- Performance (Task.WhenAll): Phase 04 primary, Phase 07 audit
- README/doc: Phase 08
- CI: Phase 01 skeleton, Phase 09 enhancements

### 5) Test Strategy (incremental, gated)
- Unit tests: validators, mappers, service logic; Shouldly assertions; AutoFixture.AutoBogus for data
- In-memory integration tests: API endpoints via WebApplicationFactory; Infrastructure stubs
- Non-functional checks: health endpoints, swagger availability, structured logging presence
- Security tests: OAuth enabled/disabled paths; 401/403 behavior; feature toggle semantics
- Resilience tests: injected fakes for timeouts/failures to validate retries/circuit breaking
- Performance checks: concurrency correctness and basic throughput using Task.WhenAll in logic

### 6) Risk & Mitigation
- Over-coupling between APIs: maintain separate bounded contexts; integrate via HTTP or shared contracts only where needed
- Legacy system variability: stubs and fakes with timeouts/errors to design for resilience
- Log/store vendor lock-in: abstract logging sinks; config-driven
- CI flakiness: deterministic tests, seeded data, retry minimal where appropriate

### 7) Acceptance of v1
- All phase exit criteria met; pipelines green; docker-compose brings system up locally; endpoints functional with validators and error handling; documentation complete; resilience/performance baselines collected.

