## Phase 08 – Documentation (README and Architecture Notes)

### Scope
- Write README with architecture overview, design decisions, local run/test instructions, error handling, extensibility, security

### Detailed Steps
1) README
- Overview of services, endpoints, dependencies
- How to run locally with docker-compose and without
- How to run tests; how to view OpenAPI and health endpoints
- Sample requests and expected responses

2) Eplaination of approach on:
- Error handling strategy and problem details schema
- Security approach: OAuth toggle, feature flags, token forwarding
- Extensibility of code

2) Architecture Notes
- Clean Architecture layering and boundaries
- Data model for Products/Insurances
- Observability stack and configuration

Also make sure CONTRIBUTING.md is updated and relevant

### Deliverables
- README.md at repo root (or docs/) and architecture notes

### Quality Assurance & Tests
- New developer dry-run: follow README to run and test locally; capture gaps and fix
- Lint docs (spelling/links); ensure diagrams render

### Exit Criteria (Gate to Phase 09)
- README complete and validated by dry-run

