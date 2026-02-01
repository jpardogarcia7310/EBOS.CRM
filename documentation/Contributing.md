# Contributing

Thank you for your interest in contributing to EBOS.CRM. This guide explains the workflow and expectations for contributions.

## 1. Getting Started
- Ensure you have .NET SDK 8.x installed.
- Clone the repository and restore dependencies:
  - `dotnet restore`

## 2. Project Structure
- API: `EBOS.CRM.Api`
- Application: `EBOS.CRM.Application`
- Domain: `EBOS.CRM.Domain`
- Infrastructure: `EBOS.CRM.Infrastructure`
- Tests:
  - Unit tests: `tests/EBOS.CRM.ApiTests`
  - Integration tests: `tests/EBOS.CRM.Api.IntegrationTests`

## 3. Coding Guidelines
- Follow existing naming conventions and folder structure.
- Keep changes small and focused.
- Prefer ASCII for new content unless the file already contains non-ASCII.
- Update or add tests for any functional change.

## 4. Testing
Run tests before opening a PR:
- All tests:
  - `dotnet test`
- Unit tests only:
  - `dotnet test tests/EBOS.CRM.ApiTests/EBOS.CRM.ApiTests.csproj`
- Integration tests only:
  - `dotnet test tests/EBOS.CRM.Api.IntegrationTests/EBOS.CRM.Api.IntegrationTests.csproj`

## 5. API Response Conventions
- List endpoints return a paged response:
  - DTO: `EBOS.CRM.Application.Contracts.Responses.Common.PagedResponse<T>`
  - Tests should read list responses via `PagedResponse<T>.Items`.
  - Helpers:
    - `ReadPagedItemsAsync<T>()` in `tests/**/TestUtils/HttpContentExtensions.cs`
    - `GetFirstIdAsync<T>()` in `tests/EBOS.CRM.ApiTests/TestUtils/ControllerTestHelper.cs`

## 6. Repository Interfaces
- Repositories that support pagination implement `IPagedRepository<T>` and must expose:
  - `Task<PagedResult<T>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)`

## 7. Pull Request Checklist
- Code builds locally.
- Tests are updated and passing.
- Public API changes are documented if applicable.
- License headers and notices are preserved.

## 8. License
By contributing, you agree that your contributions will be licensed under the project license (LGPL-3.0), unless stated otherwise.

