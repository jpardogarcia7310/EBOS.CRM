# AGENTS

## Scope
This repository contains the EBOS.CRM solution (API, Application, Domain, Infrastructure) plus unit and integration tests.
Use the instructions below when making changes in this solution.

## Architecture Quick Map
- API: `EBOS.CRM.Api`
- Application (CQRS/Handlers, contracts): `EBOS.CRM.Application`
- Domain (entities, interfaces): `EBOS.CRM.Domain`
- Infrastructure (EF Core, repositories): `EBOS.CRM.Infrastructure`
- Tests:
  - Unit tests: `tests/EBOS.CRM.ApiTests`
  - Integration tests: `tests/EBOS.CRM.Api.IntegrationTests`

## Testing Conventions
- List endpoints return a **raw list**:
  - DTO: `IReadOnlyCollection<T>`
  - Tests should deserialize list endpoints as `IReadOnlyCollection<T>`.
  - Helpers:
    - Unit/integration tests: `ReadItemsAsync<T>()` in `tests/**/TestUtils/HttpContentExtensions.cs`
    - Controller helper: `GetFirstIdAsync<T>()` in `tests/EBOS.CRM.ApiTests/TestUtils/ControllerTestHelper.cs`

## Running Tests
- All tests:
  - `dotnet test`
- Unit tests only:
  - `dotnet test tests/EBOS.CRM.ApiTests/EBOS.CRM.ApiTests.csproj`
- Integration tests only:
  - `dotnet test tests/EBOS.CRM.Api.IntegrationTests/EBOS.CRM.Api.IntegrationTests.csproj`

## Repository Patterns
- Repositories expose list reads via `GetAllAsync` and return `ICollection<T>`.

## CQRS/Handlers
- Handlers live in `EBOS.CRM.Application/Features/**`.
- Query handlers returning lists should return `IReadOnlyCollection<T>`.

## API Controllers
- Controllers reside in `EBOS.CRM.Api/Controllers/**`.
- List endpoints should return `IReadOnlyCollection<T>` with HTTP 200.

## Coding Guidelines
- Prefer small, focused changes.
- Keep additions consistent with existing naming and folder structure.
- Use ASCII for file contents unless the file already contains non-ASCII characters.

