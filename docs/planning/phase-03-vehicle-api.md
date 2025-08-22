## Phase 03 – Vehicle API (Endpoint 1)

> Alignment: Vehicle information originates from an external legacy system. In v1 we will integrate via an Infrastructure abstraction and provide a stubbed implementation (used by tests and local/dev). The Vehicles API owns this boundary. The Insurances API must not call the legacy stub directly—only the Vehicles API.

### Scope
- Implement Vehicle Registration Number validator (Swedish rules; regex given) with Shouldly-tested unit tests
- Infrastructure stub for external legacy vehicle info provider (latency/error injection ready)
- REST endpoint: accepts vehicle registration number and returns vehicle info
- Graceful error handling and correct HTTP codes

### Detailed Steps
1) Contracts & Domain
- Define Vehicle record (e.g., RegNo, Make, Model, Year, FuelType)
- Define DTOs and mappers between domain and API contracts

2) Validation

> Stub Simulation: Follow docs/planning/stub-simulation-spec.md for consistent scenarios (Success, NotFound, Timeout, Error, Slow), control via DI-seeded map in tests and magic inputs for local.

- Implement Swedish registration validator using provided regex
- Add guard middleware/filters to convert validation failures to 400 with problem+details

`var regex = new Regex(
    @"^(?:
        # Ordinarie (gamla och nya)
        [A-Z-[IQVÅÄÖ]]{3}\s?(?:\d{3}|\d{2}[A-Z-[IQVÅÄÖO]])
        |
        # Personlig skylt (2–7 tecken, minst två icke-mellanslag)
        (?=.*\S.*\S)[A-Za-zÅÄÖåäö0-9 ]{2,7}
    )$",
    RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase
);
var regex = new Regex(
    @"^(?:
        # Ordinarie (gamla och nya)
        [A-Z-[IQVÅÄÖ]]{3}\s?(?:\d{3}|\d{2}[A-Z-[IQVÅÄÖO]])
        |
        # Personlig skylt (2–7 tecken, minst två icke-mellanslag)
        (?=.*\S.*\S)[A-Za-zÅÄÖåäö0-9 ]{2,7}
    )$",
    RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase
);
`

3) Infrastructure Stub
- Define IVehicleInfoProvider; create StubVehicleInfoProvider with configurable responses (found/not found/timeout)

4) Application & API
- Application service retrieves vehicle info via provider
- API controller/Minimal API endpoint with proper status codes: 200 when found, 404 when not found, 400 invalid reg, 503 on downstream timeout

### Deliverables
- Vehicles API endpoint implemented with validator and stubbed infrastructure
- Mapping and records in place

### Quality Assurance & Tests
- Unit tests:
  - Validator: valid/invalid edge cases per regex, casing and spacing, personal plates
  - Service: maps provider results to responses; handles not found; propagates cancellations/timeouts
- In-memory integration tests:
  - GET/POST endpoint behavior with WebApplicationFactory
  - OAuth enabled/disabled paths
  - Structured error responses (problem details) and correct HTTP codes
- Resilience tests using stub provider:
  - Timeout -> 503; error -> 502/500 as appropriate

### Entry Criteria
- Phase 02 exit criteria satisfied

### Exit Criteria (Gate to Phase 04)
- All vehicle endpoint tests pass in CI (unit + integration)
- Error handling verified with edge cases (invalid, not found, timeout)

