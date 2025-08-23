# Contributing to ThreadPilot

Thank you for your interest in contributing! This project follows a phased plan. For Phase 01, please focus contributions on:

- Solution and project scaffolding (no endpoints yet)
- Baseline tests that pass
- CI build/test workflow

## Branching / PRs
- Create feature branches from `main`
- Keep PRs small and focused
- Ensure `dotnet build` and `dotnet test` both succeed locally

## Coding Standards
- .NET 9.0, nullable enabled, warnings as errors
- Follow Clean Architecture layout: Api → Application → Domain; Infrastructure is separate, referenced via abstractions by Application (future phases)
- Use records for contracts (future phases)

## Commit Messages
- Use clear, descriptive messages
- Reference the phase/task when applicable

## Running the solution
- Restore/build/test using the .NET SDK 9.x
- No external services required in Phase 01

