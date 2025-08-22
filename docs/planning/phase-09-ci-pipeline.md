## Phase 09 – CI Pipeline Enhancements

### Scope
- Enhance GitHub Actions beyond skeleton: caching, matrix builds, Docker builds, PR gates

### Detailed Steps
1) Build Matrix
- dotnet target frameworks matrix (if multiple) and OS matrix if useful
- Cache NuGet packages

2) Test and Coverage
- Run unit and integration tests; collect coverage (Coverlet + ReportGenerator)
- Upload coverage artifact; optional status check threshold

3) Docker Builds
- Build multi-stage Docker images for both APIs on PR; push on main/tag to registry (opt-in)

4) PR Quality Gates
- Require green build/test; optional linting and formatting checks

### Deliverables
- Robust CI with clear artifacts (test results, coverage, images)

### Quality Assurance & Tests
- CI runs green across branches and PRs; artifacts present
- Intentional failure tests validate gates (e.g., failing a test turns pipeline red)

### Entry Criteria
- Phase 08 exit criteria satisfied

### Exit Criteria (Project v1 Ready)
- CI reliably builds, tests, packages; PR gates enforced

