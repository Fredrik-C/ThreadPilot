## Phase 02 – Observability & Security Foundations

### Scope
- OpenAPI (Swagger) for both APIs
- Health checks endpoints
- Structured logging (console by default; configuration-driven Elastic sink readiness)
- OpenTelemetry tracing/metrics/logs wiring
- OAuth protection with appsettings/IOptions toggle; prepare for feature flags

### Detailed Steps
1) OpenAPI & Health
- Add Swashbuckle; expose swagger-ui in dev; OpenAPI doc versioning
- Add ASP.NET Core Health Checks; liveness/readiness endpoints

2) Logging
- Configure structured logging (Serilog or Microsoft.Extensions.Logging with JSON console)
- Provide configuration for Elastic/Splunk sink without hard dependency

3) OpenTelemetry
- Add OTel SDK; exporters (console/OTLP configurable); trace ID correlation with logs

4) Security
- Add OAuth/JWT authentication & authorization policies
- Implement IOptions toggle to disable auth for local/dev; register feature flag abstraction

### Deliverables
- Both APIs expose /swagger and /health endpoints
- Structured JSON logs with correlation IDs
- Distributed tracing enabled
- OAuth enabled by default; can be disabled via configuration

### Quality Assurance & Tests
- Endpoint availability tests (integration): /health returns healthy; /swagger returns 200 in dev
- Logging tests: verify structured format and correlation ID presence (via captured sink)
- Security tests: with OAuth enabled, unauthenticated requests to protected endpoints get 401/403; with toggle disabled, endpoints accessible
- OpenTelemetry smoke tests: ensure spans created for requests

### Entry Criteria
- Phase 01 exit criteria satisfied

### Exit Criteria (Gate to Phase 03)
- Health, OpenAPI, logging, tracing validated by tests in CI
- OAuth toggle behavior validated

