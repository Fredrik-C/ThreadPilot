# Contributing to ThreadPilot

Thank you for your interest in contributing! We are in Phase 08 (Documentation). Focus on improving developer experience: docs completeness, samples, and accuracy matching the code.

## Scope for Phase 08

- Update README with run/test instructions, endpoints, samples, error handling, security, observability, and architecture notes
- Keep CONTRIBUTING current (this file)
- Ensure docker-compose and local run are reflected accurately
- Add missing samples or clarifications discovered during a dry-run

## Branching / PRs

- Branch from `main` or the current phase branch as instructed in issues
- Keep PRs small and focused
- Ensure `dotnet build` and `dotnet test` succeed locally
- If updating run instructions, validate with a local dry-run before requesting review

## Coding standards

- .NET 9, nullable enabled, treat warnings as errors (see Directory.Build.props)
- Clean Architecture: Api ↔ Application ↔ Domain; Infrastructure integrates via abstractions
- Records for DTOs where appropriate; prefer async, cancellation tokens, and Task.WhenAll for parallelism
- Structured logging via Serilog; OpenTelemetry for traces/metrics

## Commit messages

- Clear, descriptive; reference phase/task when applicable

## Running locally

- dotnet build && dotnet test
- docker compose up --build -d (see README for ports and samples)

## Adding documentation

- Keep README sections concise, accurate, and verified against the current codebase
- Provide minimal reproducible commands (curl samples, docker compose)
- When showing code, prefer short excerpts with context

## Reporting issues

- Use GitHub Issues; include repro steps, observed vs expected, logs (redact secrets)
