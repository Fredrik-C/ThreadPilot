## Phase 08 – Documentation (README and Architecture Notes)

### Scope
- Write README with architecture overview, design decisions, local run/test instructions, error handling, extensibility, security
- Add architecture diagrams if helpful

### Detailed Steps
1) README
- Overview of services, endpoints, dependencies
- How to run locally with docker-compose and without
- How to run tests; how to view OpenAPI and health endpoints
- Error handling strategy and problem details schema
- Security approach: OAuth toggle, feature flags, threat model (basic)

2) Architecture Notes
- Clean Architecture layering and boundaries
- Data model for Products/Insurances
- Observability stack and configuration

### Deliverables
- README.md at repo root (or docs/) and architecture notes

### Quality Assurance & Tests
- New developer dry-run: follow README to run and test locally; capture gaps and fix
- Lint docs (spelling/links); ensure diagrams render

### Entry Criteria
- Phase 07 exit criteria satisfied

### Exit Criteria (Gate to Phase 09)
- README complete and validated by dry-run

