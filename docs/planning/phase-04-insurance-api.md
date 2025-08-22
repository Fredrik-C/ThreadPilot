## Phase 04 – Insurance API (Endpoint 2)

> Alignment: Vehicle data enrichment for car insurances is obtained by calling the Vehicles API, which in turn uses an Infrastructure stub for the external legacy vehicle system in v1. The Insurances API does not call the legacy stub directly.

### Scope
- Implement Swedish personal identification validator (Luhn-like steps described)
- Infrastructure stub for legacy insurance provider (returns a list of insurances with product and monthly cost)
- REST endpoint: accepts personal id and returns all insurances; for car insurances, include vehicle info by integrating with Vehicle API
- Use Task.WhenAll to parallelize car info fetches
- Graceful error handling and correct HTTP codes

### Detailed Steps
1) Domain & Contracts
- Define Product and Insurance records; Product contains Name, Price, Terms; Insurance references Product and optional VehicleRegNo
- Define response model aggregating Insurances and embedded Vehicle info when applicable

2) **Swedish Personal ID Validation**
   - Implement Swedish personal identification number validator using the Luhn-like algorithm:
     * **Dubblera varannan siffra**: från höger, men inte den första (double every second digit from right, but not the first)
     * **Summera alla siffror**: och dra bort 9 om summan är över 9 (sum all digits, subtract 9 if sum > 9)
     * **Kontrollsiffran**: är den siffra som behövs för att den totala summan ska bli en multipel av 10 (check digit makes total sum divisible by 10)
   - Support both 10-digit (YYMMDD-XXXX) and 12-digit (YYYYMMDD-XXXX) formats
   - Map validation failures to HTTP 400 with RFC 7807 Problem Details format
   - Include detailed validation error messages for different failure scenarios

3) Infrastructure Stubs
- IInsuranceProvider: returns list of insurances by personal id (pet, personal health, car)
- For car items: retain VehicleRegNo for follow-up enrichment

4) **Vehicle API Integration & Orchestration**
   - Application orchestrator identifies car insurances and fires parallel requests to Vehicle API using Task.WhenAll
   - Implement resilient integration patterns:
     * **Missing vehicle data**: include insurance without vehicle details, log warning
     * **Timeout scenarios**: return partial data with HTTP 206 (Partial Content) or 200 with error indicators
     * **Complete failure**: return HTTP 503 (Service Unavailable) with retry-after header
   - Use HttpClient with Polly for retry policies and circuit breaker patterns
   - Implement proper cancellation token propagation for request timeouts

> Stub Simulation: Follow docs/planning/stub-simulation-spec.md for consistent Vehicles and Insurances stub scenarios. Control via DI-seeded maps in tests; optional magic inputs for local/manual.

5) API Layer
- Endpoint with proper status codes: 200 on success, 400 invalid id, 504/503 on legacy outages beyond policy

### Deliverables
- Insurances API endpoint implemented with validator and enrichment via Vehicles API

### Quality Assurance & Tests
- Unit tests:
  - Personal id validator including edge cases
  - Orchestrator logic: multiple insurances, none, multiple cars, downstream timeout/partial
- In-memory integration tests:
  - Endpoint happy path, no insurances, multiple insurances, multiple cars
  - OAuth on/off paths
- Performance tests (basic): ensure parallelization reduces latency vs. sequential mock
- Error handling tests: downstream not responding/timeouts -> policy-conformant HTTP codes

### Entry Criteria
- Phase 03 exit criteria satisfied

### Exit Criteria (Gate to Phase 05)
- All insurance endpoint tests pass in CI (unit + integration)
- Performance assertion for parallelization holds (mocked timings demonstrate benefit)

