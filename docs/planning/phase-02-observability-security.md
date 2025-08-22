## Phase 02 – Observability & Security Foundations

### Scope
- OpenAPI (Swagger) for both APIs
- Health checks endpoints
- Structured logging (console by default; configuration-driven Elastic sink readiness)
- OpenTelemetry tracing/metrics/logs wiring
- OAuth protection with appsettings/IOptions toggle; prepare for feature flags

### Detailed Steps

1) **OpenAPI & Health Checks**
   - Add Swashbuckle.AspNetCore with OpenAPI 3.0+ specification
   - Configure Swagger UI for development environment with API versioning support
   - Implement ASP.NET Core Health Checks with separate liveness (`/health/live`) and readiness (`/health/ready`) endpoints
   - Add health check for database connectivity and external service dependencies

2) **Structured Logging**
   - Configure Serilog with structured JSON logging for console output
   - Add correlation ID middleware for request tracing across services
   - Prepare configuration for Elasticsearch/Splunk sinks without hard dependencies
   - Implement log enrichment with user context, request details, and performance metrics

3) **OpenTelemetry Integration**
   - Add OpenTelemetry SDK with ASP.NET Core, HttpClient, and EF Core instrumentation
   - Configure exporters (console for dev, OTLP for production) via configuration
   - Ensure trace ID correlation between logs and distributed traces
   - Add custom metrics for business operations (API calls, validation failures, etc.)

4) **Security Framework**
   - Implement OAuth 2.0/JWT authentication with configurable identity providers
   - Add authorization policies for different API operations and user roles
   - Create IOptions-based toggle to disable authentication for local development
   - Register feature flag abstraction for runtime configuration management

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

