## Phase 06 – Containerization & Local Orchestration

### Scope
- Multi-stage Dockerfiles for both APIs
- Health checks in container definitions
- docker-compose.yml orchestrating APIs

### Detailed Steps
1) Dockerfiles
- Multi-stage (build/publish/runtime), respect configuration via env vars
- Expose ports and healthcheck commands

2) docker-compose
- Define services: vehicles-api, insurances-api, sqlserver
- Network, volumes for DB persistence, environment configuration injection

3) Local Runbook
- Makefile/PowerShell scripts for compose up/down, logs, seed data if any

### Deliverables
- Dockerfiles for both APIs
- docker-compose.yml running all services locally

### Quality Assurance & Tests
- Compose smoke test script: wait for health, curl swagger/health endpoints, sample requests
- Validate logs and OpenTelemetry exporters in container environment

### Exit Criteria (Gate to Phase 07)
- Local compose up works; health endpoints healthy; sample API flows succeed

