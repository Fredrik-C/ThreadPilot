## Phase 05 – Persistence (EF Core + DbUp + SQL Server)

### Scope
- Introduce EF Core for persistence and DbUp for migrations
- Model Products and Insurances (and relationship) in the database
- Configure DbUp migrations targeting containerized SQL Server
- Tests continue to use in-memory provider

### Detailed Steps

1) **Domain & Data Modeling**
   - Define domain records: `Product(Id, Name, Price, Terms)`, `Insurance(Id, PersonId, ProductId, VehicleRegNo?)`
   - Create separate DbContext per API to maintain bounded context isolation
   - Implement repository pattern with generic base repository and specific implementations
   - Use EF Core value converters for domain-specific types (e.g., PersonId, VehicleRegNo)

2) **DbUp Migration Strategy**
   - Create DbUp console application or hosted service for migration execution
   - Implement idempotent migration scripts with proper rollback strategies
   - Baseline schema scripts: Products, Insurances tables with proper indexes and foreign keys
   - Add audit columns (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy) for data lineage

3) **EF Core Configuration**
   - Configure DbContext with connection string from appsettings and environment variables
   - Implement connection resilience with retry policies for transient failures
   - Configure query splitting and performance optimizations for complex queries
   - Add database health checks for monitoring and readiness probes

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

