# Service MVP Technical Backlog

Concrete work items aligned with the current local structure (Clean Architecture, Service module under `EBOS.CRM.*`).
Derived from issues #60, #81, #82, #83, #84 scopes.

## Scope for MVP

- Case management (create, update, close, reopen).
- SLA tracking (targets, breach checks).
- Queue assignment (routing rules, manual reassign).
- Case activity tracking for workflows (CaseActivity).

## Why this milestone helps the CRM (layer by layer)

### Domain value

- Adds the minimum business vocabulary to deliver support: Case, Sla, Queue, CaseActivity.
- Enables consistent lifecycle rules (open/close/reopen) and SLA due dates.

**Pros**
- Clear ownership and states for service operations.
- Reusable invariants for future automation.

**Cons**
- Requires disciplined data entry to stay reliable.

### Application value

- Encodes workflows (case lifecycle, SLA checks, queue assignment rules, activities).
- Centralizes validation and tenant isolation.

**Pros**
- Predictable behavior across API, tests, and UI.
- Easier to evolve rules without touching controllers.

**Cons**
- More handler/validator surface to maintain.

### API value

- Exposes endpoints to create/manage cases, SLAs, queues, and case activities.
- Enables integration with UI or external systems.

**Pros**
- Fast enablement for support dashboards.
- Consistent patterns with existing CRM controllers.

**Cons**
- Additional endpoints to version and document.

### Infrastructure value

- Persists service data with EF mappings + migrations.
- Provides repositories for Service entities.

**Pros**
- Reliable storage and query performance.
- Consistent DI and data access patterns.

**Cons**
- Migration overhead and schema evolution to manage.

### Testing value

- Verifies lifecycle rules, SLA behavior, and assignment workflows.

**Pros**
- Reduces regressions when business rules grow.
- Confidence for future automation and reporting.

**Cons**
- More test coverage to keep up to date.

## Why this milestone helps the CRM (layer by layer)

### Domain value

- Adds the minimum business vocabulary to deliver support: Case, Sla, Queue.
- Enables consistent lifecycle rules (open/close/reopen) and SLA due dates.

**Pros**
- Clear ownership and states for service operations.
- Reusable invariants for future automation.

**Cons**
- Requires disciplined data entry to stay reliable.

### Application value

- Encodes workflows (case lifecycle, SLA checks, queue assignment rules).
- Centralizes validation and tenant isolation.

**Pros**
- Predictable behavior across API, tests, and UI.
- Easier to evolve rules without touching controllers.

**Cons**
- More handler/validator surface to maintain.

### API value

- Exposes endpoints to create/manage cases, SLAs, and queues.
- Enables integration with UI or external systems.

**Pros**
- Fast enablement for support dashboards.
- Consistent patterns with existing CRM controllers.

**Cons**
- Additional endpoints to version and document.

### Infrastructure value

- Persists service data with EF mappings + migrations.
- Provides repositories for Service entities.

**Pros**
- Reliable storage and query performance.
- Consistent DI and data access patterns.

**Cons**
- Migration overhead and schema evolution to manage.

### Testing value

- Verifies lifecycle rules, SLA behavior, and assignment workflows.

**Pros**
- Reduces regressions when business rules grow.
- Confidence for future automation and reporting.

**Cons**
- More test coverage to keep up to date.

## Domain (EBOS.CRM.Domain)

### Aggregates and entities

- Case
  - Fields: Id, TenantId, Title, Description, Status, Priority, OwnerUserId, QueueId, SlaId, DueAt, ClosedAt, CreatedAt, UpdatedAt.
  - Behavior: Open, UpdateDetails, AssignQueue, AssignOwner, Close, Reopen.
- Sla
  - Fields: Id, TenantId, Name, TargetMinutes, WarningMinutes, ActiveFrom, ActiveTo, IsActive.
  - Behavior: IsActiveAt(date), CalculateDueAt(start), IsBreached(now, dueAt).
- Queue
  - Fields: Id, TenantId, Name, Code, IsActive, DefaultOwnerUserId.
  - Behavior: Activate, Deactivate.
- CaseActivity
  - Fields: Id, TenantId, CaseId, Title, Description, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy.
  - Behavior: Allowed statuses Open/InProgress/Completed/Cancelled.

### Interfaces (Repositories)

- `ICaseRepository`
- `ICaseActivityRepository`
- `ISlaRepository`
- `IQueueRepository`

### Invariants

- TenantId required on all aggregates.
- Case must reference an existing Queue and Sla (tenant-scoped).
- Only open cases can be closed; only closed cases can be reopened.
- DueAt computed from Sla target; breach check uses current time.

## Application (EBOS.CRM.Application)

### Contracts (Requests/Responses)

- Requests:
  - Case: AddCaseRequest, UpdateCaseRequest, CloseCaseRequest, ReopenCaseRequest, AssignCaseQueueRequest, AssignCaseOwnerRequest.
  - CaseActivity: AddCaseActivityRequest, UpdateCaseActivityRequest.
  - Sla: AddSlaRequest, UpdateSlaRequest, ToggleSlaRequest.
  - Queue: AddQueueRequest, UpdateQueueRequest, ToggleQueueRequest, AssignQueueDefaultOwnerRequest.
  - SLA checks: CheckCaseSlaRequest (caseId, now).
- Responses:
  - CaseResponse, CaseActivityResponse, SlaResponse, QueueResponse, SlaCheckResponse.

### Features (Commands/Queries)

Structure mirrors current CRM features, e.g. `Features/CRM/Service/...`:

- `Features/CRM/Service/Case/Commands/AddCase`
- `Features/CRM/Service/Case/Commands/UpdateCase`
- `Features/CRM/Service/Case/Commands/CloseCase`
- `Features/CRM/Service/Case/Commands/ReopenCase`
- `Features/CRM/Service/Case/Commands/AssignCaseQueue`
- `Features/CRM/Service/Case/Commands/AssignCaseOwner`
- `Features/CRM/Service/Case/Queries/GetCaseById`
- `Features/CRM/Service/Case/Queries/GetAllCases`

- `Features/CRM/Service/CaseActivity/Commands/AddCaseActivity`
- `Features/CRM/Service/CaseActivity/Commands/UpdateCaseActivity`
- `Features/CRM/Service/CaseActivity/Commands/DeleteCaseActivity`
- `Features/CRM/Service/CaseActivity/Queries/GetCaseActivityById`
- `Features/CRM/Service/CaseActivity/Queries/GetAllCaseActivities`

- `Features/CRM/Service/Sla/Commands/AddSla`
- `Features/CRM/Service/Sla/Commands/UpdateSla`
- `Features/CRM/Service/Sla/Commands/ToggleSla`
- `Features/CRM/Service/Sla/Queries/GetSlaById`
- `Features/CRM/Service/Sla/Queries/GetAllSlas`
- `Features/CRM/Service/Sla/Queries/CheckCaseSla`

- `Features/CRM/Service/Queue/Commands/AddQueue`
- `Features/CRM/Service/Queue/Commands/UpdateQueue`
- `Features/CRM/Service/Queue/Commands/ToggleQueue`
- `Features/CRM/Service/Queue/Commands/AssignQueueDefaultOwner`
- `Features/CRM/Service/Queue/Queries/GetQueueById`
- `Features/CRM/Service/Queue/Queries/GetAllQueues`

### Mapping

- `Mappings/CRM/MappingCase`
- `Mappings/CRM/MappingCaseActivity`
- `Mappings/CRM/MappingSla`
- `Mappings/CRM/MappingQueue`

### Validation

- FluentValidation validators per request.
- Enforce tenant isolation and required references.

## API (EBOS.CRM.Api)

### Controllers

Follow existing CRM controllers layout:

- `Controllers/CRM/Service/Case/CaseController`
- `Controllers/CRM/Service/CaseActivity/CaseActivityController`
- `Controllers/CRM/Service/Sla/SlaController`
- `Controllers/CRM/Service/Queue/QueueController`

### Endpoints (v2)

- Cases
  - `GET /api/v2/Case`
  - `GET /api/v2/Case/{id}`
  - `POST /api/v2/Case`
  - `PUT /api/v2/Case/{id}`
  - `PATCH /api/v2/Case/{id}/close`
  - `PATCH /api/v2/Case/{id}/reopen`
  - `PATCH /api/v2/Case/{id}/queue`
  - `PATCH /api/v2/Case/{id}/owner`
- CaseActivities
  - `GET /api/v2/CaseActivity`
  - `GET /api/v2/CaseActivity/{id}`
  - `GET /api/v2/CaseActivity/by-case/{caseId}?status=...&from=...&to=...`
  - `POST /api/v2/CaseActivity`
  - `PUT /api/v2/CaseActivity/{id}`
  - `DELETE /api/v2/CaseActivity/{id}`
- SLAs
  - `GET /api/v2/Sla`
  - `GET /api/v2/Sla/{id}`
  - `POST /api/v2/Sla`
  - `PUT /api/v2/Sla/{id}`
  - `PATCH /api/v2/Sla/{id}/toggle`
  - `GET /api/v2/Sla/{id}/check?now=...&caseId=...`
- Queues
  - `GET /api/v2/Queue`
  - `GET /api/v2/Queue/{id}`
  - `POST /api/v2/Queue`
  - `PUT /api/v2/Queue/{id}`
  - `PATCH /api/v2/Queue/{id}/toggle`
  - `PATCH /api/v2/Queue/{id}/default-owner`

## Infrastructure (EBOS.CRM.Infrastructure)

- EF Core configurations for Case, Sla, Queue.
- Add repository implementations and DI registrations.
- Update `CrmDbContext` and migrations.

## Tests (tests/EBOS.CRM.ApiTests)

### Domain tests

- Case lifecycle transitions (open, close, reopen) and invariants.
- SLA due date calculation and breach checks.
- Queue activation rules.

### Application tests

- Command handlers success and error paths.
- Validation for required references and tenant scope.
- SLA check query returns expected status.

### Controller tests

- CRUD happy paths and invalid payloads.
- Close/reopen/assign endpoints behavior.

### Integration tests

- End-to-end case creation with queue + SLA.
- Tenant isolation across cases, SLAs, queues.

