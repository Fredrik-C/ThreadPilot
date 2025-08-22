## Phase 05 – Persistence (EF Core + DbUp + SQL Server)

### Scope
- Introduce EF Core for persistence and DbUp for migrations
- Model Products and Insurances (and relationship) in the database
- Configure DbUp migrations targeting containerized SQL Server
- Tests continue to use in-memory provider

### Detailed Steps
1) Domain & Data Modeling
- Records: Product(Id, Name, Price, Terms), Insurance(Id, PersonId, ProductId, VehicleRegNo?)
- EF Core DbContext per API (or shared data project if justified), with migrations owned by DbUp

2) DbUp Migrations
- Create DbUp console or hosted startup runner that applies migrations at boot (idempotent)
- Baseline schema scripts: Products, Insurances tables and FKs

3) EF Core Integration
- Configure DbContext, repositories/services; connection strings via appsettings and env vars

### Deliverables
- DbUp project and migration scripts
- EF Core wired with repositories/services where needed

### Quality Assurance & Tests
- Migration tests: apply baseline on empty DB, re-apply safely (idempotent)
- Repository tests with EF Core InMemory provider: CRUD and queries for Insurance and Product
- Health checks include DB connectivity check

### Entry Criteria
- Phase 04 exit criteria satisfied

### Exit Criteria (Gate to Phase 06)
- Migrations apply cleanly in CI against a test SQL container (optional smoke)
- In-memory repository tests pass in CI

