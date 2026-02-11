# Backlog tecnico Service MVP

Trabajo concreto alineado con la estructura local (Clean Architecture, módulo Service en `EBOS.CRM.*`).
Derivado de los issues #60, #81, #82, #83, #84.

## Alcance del MVP

- Gestion de casos (crear, actualizar, cerrar, reabrir).
- Seguimiento de SLA (objetivos, chequeo de incumplimiento).
- Asignación a colas (reglas de ruteo, reasignación manual).

## Para que sirve este milestone en el CRM (capa por capa)

### Valor en Domain

- Agrega el vocabulario minimo de servicio: Case, Sla, Queue.
- Define reglas de ciclo de vida y calculo de vencimientos por SLA.

**Pros**
- Estados y ownership claros para operacion.
- Invariants reutilizables para automatizaciones futuras.

**Contras**
- Requiere disciplina en la carga de datos.

### Valor en Application

- Implementa workflows (ciclo de vida, chequeos SLA, asignacion a cola).
- Centraliza validacion y aislamiento por tenant.

**Pros**
- Comportamiento consistente entre API, tests y UI.
- Facil de evolucionar sin tocar controladores.

**Contras**
- Mas handlers/validators que mantener.

### Valor en API

- Expone endpoints para casos, SLAs y colas.
- Habilita integraciones con UI o sistemas externos.

**Pros**
- Permite habilitar rapido dashboards de soporte.
- Patrones consistentes con el resto de CRM.

**Contras**
- Mas endpoints para versionar y documentar.

### Valor en Infrastructure

- Persistencia con EF mappings + migrations.
- Repositorios para entidades de Service.

**Pros**
- Almacenamiento confiable y consultas eficientes.
- Patrones de DI y data access consistentes.

**Contras**
- Overhead de migrations y evolucion de esquema.

### Valor en Tests

- Verifica reglas de ciclo de vida, SLA y asignaciones.

**Pros**
- Reduce regresiones al crecer reglas de negocio.
- Confianza para automatizacion y reporting.

**Contras**
- Mas cobertura a mantener.

## Domain (EBOS.CRM.Domain)

### Aggregates y entities

- Case
  - Fields: Id, TenantId, Title, Description, Status, Priority, OwnerUserId, QueueId, SlaId, DueAt, ClosedAt, CreatedAt, UpdatedAt.
  - Behavior: Open, UpdateDetails, AssignQueue, AssignOwner, Close, Reopen.
- Sla
  - Fields: Id, TenantId, Name, TargetMinutes, WarningMinutes, ActiveFrom, ActiveTo, IsActive.
  - Behavior: IsActiveAt(date), CalculateDueAt(start), IsBreached(now, dueAt).
- Queue
  - Fields: Id, TenantId, Name, Code, IsActive, DefaultOwnerUserId.
  - Behavior: Activate, Deactivate.

### Interfaces (Repositories)

- `ICaseRepository`
- `ISlaRepository`
- `IQueueRepository`

### Invariants

- TenantId requerido en todos los aggregates.
- Case debe referenciar Queue y Sla existentes (tenant-scoped).
- Solo casos abiertos pueden cerrarse; solo casos cerrados pueden reabrirse.
- DueAt se calcula por SLA; el check usa la hora actual.

## Application (EBOS.CRM.Application)

### Contracts (Requests/Responses)

- Requests:
  - Case: AddCaseRequest, UpdateCaseRequest, CloseCaseRequest, ReopenCaseRequest, AssignCaseQueueRequest, AssignCaseOwnerRequest.
  - Sla: AddSlaRequest, UpdateSlaRequest, ToggleSlaRequest.
  - Queue: AddQueueRequest, UpdateQueueRequest, ToggleQueueRequest, AssignQueueDefaultOwnerRequest.
  - SLA checks: CheckCaseSlaRequest (caseId, now).
- Responses:
  - CaseResponse, SlaResponse, QueueResponse, SlaCheckResponse.

### Features (Commands/Queries)

Estructura igual a CRM, por ejemplo `Features/CRM/Service/...`:

- `Features/CRM/Service/Case/Commands/AddCase`
- `Features/CRM/Service/Case/Commands/UpdateCase`
- `Features/CRM/Service/Case/Commands/CloseCase`
- `Features/CRM/Service/Case/Commands/ReopenCase`
- `Features/CRM/Service/Case/Commands/AssignCaseQueue`
- `Features/CRM/Service/Case/Commands/AssignCaseOwner`
- `Features/CRM/Service/Case/Queries/GetCaseById`
- `Features/CRM/Service/Case/Queries/GetAllCases`

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
- `Mappings/CRM/MappingSla`
- `Mappings/CRM/MappingQueue`

### Validation

- FluentValidation por request.
- Aislamiento por tenant y referencias requeridas.

## API (EBOS.CRM.Api)

### Controllers

Seguir el layout de CRM:

- `Controllers/CRM/Service/Case/CaseController`
- `Controllers/CRM/Service/Sla/SlaController`
- `Controllers/CRM/Service/Queue/QueueController`

### Endpoints (v1)

- Cases
  - `GET /api/v1/Case`
  - `GET /api/v1/Case/{id}`
  - `POST /api/v1/Case`
  - `PUT /api/v1/Case/{id}`
  - `PATCH /api/v1/Case/{id}/close`
  - `PATCH /api/v1/Case/{id}/reopen`
  - `PATCH /api/v1/Case/{id}/queue`
  - `PATCH /api/v1/Case/{id}/owner`
- SLAs
  - `GET /api/v1/Sla`
  - `GET /api/v1/Sla/{id}`
  - `POST /api/v1/Sla`
  - `PUT /api/v1/Sla/{id}`
  - `PATCH /api/v1/Sla/{id}/toggle`
  - `GET /api/v1/Sla/{id}/check?now=...&caseId=...`
- Queues
  - `GET /api/v1/Queue`
  - `GET /api/v1/Queue/{id}`
  - `POST /api/v1/Queue`
  - `PUT /api/v1/Queue/{id}`
  - `PATCH /api/v1/Queue/{id}/toggle`
  - `PATCH /api/v1/Queue/{id}/default-owner`

## Infrastructure (EBOS.CRM.Infrastructure)

- Configuraciones EF Core para Case, Sla, Queue.
- Repositorios e inyección en DI.
- Update de `CrmDbContext` y migrations.

## Tests (tests/EBOS.CRM.ApiTests)

### Domain tests

- Ciclo de vida de Case e invariants.
- Calculo de SLA y chequeo de breach.
- Activación de Queue.

### Application tests

- Command handlers: success y errores.
- Validaciones por referencias y tenant.
- Query de SLA devuelve el estado esperado.

### Controller tests

- CRUD y payloads inválidos.
- Endpoints de close/reopen/assign.

### Integration tests

- Caso end-to-end con queue + SLA.
- Aislamiento por tenant en cases, SLAs, queues.
