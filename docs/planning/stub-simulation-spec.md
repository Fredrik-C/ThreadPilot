## Stub Simulation Specification (Vehicles and Insurances)

### Purpose
Unify how our infrastructure stubs simulate downstream behaviors (success, not found, timeout, error, slow) for deterministic tests and local runs.

### Common Principles
- Control scenarios deterministically per input using a seeded map in tests
- Offer optional "magic inputs" for manual local testing
- Keep timing, exception types, and HTTP mappings consistent across services
- Prefer DI-seeded scenarios in tests over magic inputs

### Scenario Types
- Success: returns a valid payload
- NotFound: returns null/empty (mapped to 404 or empty list at API layer)
- Timeout: delays beyond client timeout and throws TimeoutException or OperationCanceledException
- Error: throws a controlled exception (e.g., InvalidOperationException) to simulate 5xx
- Slow: delays significantly but completes within timeout

### Control Mechanisms

1) **DI-Seeded Scenario Map (preferred for tests)**
   - Seed a dictionary keyed by input with a Scenario configuration:
     - **Vehicles**: key = registration number (e.g., "ABC123")
     - **Insurances**: key = personal id (e.g., "19801201-1234")
   - **Scenario structure**: `{ Type, DelayMs?, Payload?, ErrorMessage? }`
   - Tests configure the stub via IOptions pattern or direct service registration
   - Supports complex scenarios with custom payloads and specific error conditions

2) **Magic Inputs (for manual/local testing)**
   - **Vehicles (registration number patterns)**:
     - `TIMEOUT-*` => Timeout scenario (e.g., "TIMEOUT-ABC123")
     - `ERROR-*` => Error scenario (e.g., "ERROR-XYZ789")
     - `SLOW-*` => Slow response scenario (e.g., "SLOW-DEF456")
     - `NOTFOUND-*` => NotFound scenario (e.g., "NOTFOUND-GHI789")
     - Otherwise => Success with default vehicle data
   - **Insurances (personal id patterns)**:
     - `TIMEOUT*` => Timeout scenario (e.g., "TIMEOUT1234567890")
     - `ERROR*` => Error scenario (e.g., "ERROR1234567890")
     - `SLOW*` => Slow response scenario (e.g., "SLOW1234567890")
     - `NONE*` => Success with empty insurance list (e.g., "NONE1234567890")
     - `MULTI*` => Success with multiple policies including car insurance (e.g., "MULTI1234567890")
     - Otherwise => Success with single default insurance policy

### Timing & Cancellation
- Client timeout (via HttpClient/resilience policy) governs the upper bound
- Stub delays:
  - Timeout: delay = 1.5x configured client timeout, then throw TimeoutException (or propagate OperationCanceledException if cancellation token signaled)
  - Slow: delay = 0.75x configured client timeout, then return
- Always check CancellationToken and throw OperationCanceledException immediately if requested

### Exceptions & API Mappings
- Timeout: TimeoutException or OperationCanceledException
  - API mapping: 503 ServiceUnavailable or 504 GatewayTimeout (choose consistently; plan uses 503)
- Error: InvalidOperationException
  - API mapping: 502 BadGateway or 500 InternalServerError (plan uses 502 when downstream error)
- NotFound: null/empty
  - Vehicles API mapping: 404 NotFound
  - Insurances API mapping: 200 OK with empty list
- Success: 200 OK with payload

### Default Payloads (for Success)
- Vehicles: For a normal reg (e.g., ABC123), return a deterministic vehicle
  - { RegNo: ABC123, Make: "Volvo", Model: "V60", Year: 2019, FuelType: "Petrol" }
- Insurances: Product catalog used by stub
  - Pet insurance: $10
  - Personal health insurance: $20
  - Car insurance: $30
  - Scenario presets:
    - DEFAULT: one health policy
    - NONE: empty list
    - MULTI: health + pet + car (with VehicleRegNo = ABC123)

### Validators
- Vehicle registration: use provided Swedish regex in validation layer; stubs assume validated inputs in Application layer tests, but endpoint tests should hit validators
- Personal id: use described algorithm; same assumption as above

### Test Usage Patterns

- **Unit tests**: inject stub with DI-seeded scenarios for specific input(s) under test
- **Integration tests**: replace real provider with stub; configure scenarios via test setup or magic inputs
- **Performance tests**: compare sequential vs Task.WhenAll using Slow scenarios to demonstrate concurrency benefits
- **Resilience tests**: use Timeout and Error scenarios to validate retry policies and circuit breaker behavior
- **Contract tests**: verify API responses match expected schemas for all scenario types

### Documentation & References
- Vehicles API: see docs/planning/phase-03-vehicle-api.md (Validation and Infrastructure Stub sections)
- Insurances API: see docs/planning/phase-04-insurance-api.md (Infrastructure Stubs and Integration sections)

