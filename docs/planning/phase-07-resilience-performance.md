## Phase 07 – Resilience & Performance Hardening

### Scope
- Add policies for timeouts, retries, and circuit breakers for downstream calls
- Audit Task.WhenAll usage for concurrency and cancellation propagation
- Introduce cancellation tokens across layers

### Detailed Steps
1) Resilience Policies
- Use Polly or ASP.NET built-in resilience pipeline: timeouts, retries with backoff, circuit breaker
- Configure via IOptions and feature flags

2) Concurrency Audit
- Ensure Task.WhenAll used for vehicle enrichment; add max-degree controls if needed
- Propagate CancellationToken from HTTP to downstream calls; time-box operations

3) Metrics
- Add custom metrics/timers for downstream latency; log slow calls with correlation ids

### Deliverables
- Downstream clients wrapped with resilience policies
- Concurrency paths verified and safe

### Quality Assurance & Tests
- Resilience tests using fakes: trigger retries, breaker open/half-open transitions
- Cancellation tests: request aborted cancels downstream work promptly
- Performance baseline: mocked downstream delays; compare sequential vs parallel durations

### Entry Criteria
- Phase 06 exit criteria satisfied

### Exit Criteria (Gate to Phase 08)
- Resilience and cancellation tests pass; performance targets/regressions tracked

