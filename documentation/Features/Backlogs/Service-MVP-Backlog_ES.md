# Backlog técnico Servicio MVP

Trabajo concreto alineado con la estructura local (Clean Architecture, módulo Servicio en `EBOS.CRM.*`).
Derivado de los issues #60, #81, #82, #83, #84.

Mini TOC:
1. [Alcance del MVP](#alcance-del-mvp)
2. [Para qué sirve este hito en el CRM](#para-que-sirve-este-hito-en-el-crm-capa-por-capa)
3. [Valor en Domain](#valor-en-domain)
4. [Valor en Application](#valor-en-application)
5. [Valor en API](#valor-en-api)
6. [Valor en Infraestructura](#valor-en-infraestructura)
7. [Valor en Pruebas](#valor-en-pruebas)
8. [Dominio](#dominio-eboscrmdomain)
9. [Agregados y entidades](#agregados-y-entidades)
10. [Interfaces](#interfaces-repositorios)
11. [Invariantes](#invariantes)
12. [Aplicación](#aplicacion-eboscrmapplication)
13. [Contratos](#contratos-solicitudesrespuestas)
14. [Funcionalidades](#funcionalidades-comandosconsultas)
15. [Mapeo](#mapeo)
16. [Validación](#validacion)
17. [API](#api-eboscrmapi)
18. [Controladores](#controladores)
19. [Puntos finales](#puntos-finales-v2)
20. [Infraestructura](#infraestructura-eboscrminfrastructure)
21. [Pruebas](#pruebas-testseboscrmapitests)
22. [Pruebas de dominio](#pruebas-de-dominio)
23. [Pruebas de aplicación](#pruebas-de-aplicacion)
24. [Pruebas de controladores](#pruebas-de-controladores)
25. [Pruebas de integración](#pruebas-de-integracion)
26. [Pruebas de mapeo](#pruebas-de-mapeo)
27. [Referencia de suites de pruebas existentes](#referencia-de-suites-de-pruebas-existentes)

## Alcance del MVP

- Gestión de casos (crear, actualizar, cerrar, reabrir).
- Seguimiento de SLA (objetivos, chequeo de incumplimiento).
- Asignación a colas (reglas de enrutamiento, reasignación manual).
- Registro y seguimiento de actividades del caso (CaseActivity) para flujos de trabajo.

## Para qué sirve este hito en el CRM (capa por capa)

### Valor en Domain

- Agrega el vocabulario mínimo de servicio: Case, Sla, Queue, CaseActivity.
- Define reglas de ciclo de vida y cálculo de vencimientos por SLA.

**Pros**
- Estados y ownership claros para operación.
- Invariants reutilizables para automatizaciones futuras.

**Contras**
- Requiere disciplina en la carga de datos.

### Valor en Application

- Implementa workflows (ciclo de vida, chequeos SLA, asignación a cola, actividades).
- Centraliza validación y aislamiento por tenant.

**Pros**
- Comportamiento consistente entre API, pruebas y la interfaz de usuario.
- Fácil de evolucionar sin tocar controladores.

**Contras**
- Más manejadores/validadores que mantener.

### Valor en API

- Expone endpoints para casos, SLAs, colas y actividades del caso.
- Habilita integraciones con la interfaz de usuario o sistemas externos.

**Pros**
- Permite habilitar rápido tableros de soporte.
- Patrones consistentes con el resto de CRM.

**Contras**
- Más endpoints para versionar y documentar.

### Valor en Infraestructura

- Persistencia con mapeos EF + migraciones.
- Repositorios para entidades de Service.

**Pros**
- Almacenamiento confiable y consultas eficientes.
- Patrones de DI y acceso a datos consistentes.

**Contras**
- Sobrecarga de migraciones y evolución de esquema.

### Valor en Pruebas

- Verifica reglas de ciclo de vida, SLA y asignaciones.

**Pros**
- Reduce regresiones al crecer reglas de negocio.
- Confianza para automatización y reporting.

**Contras**
- Más cobertura a mantener.

## Dominio (EBOS.CRM.Domain)

### Agregados y entidades

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
  - Behavior: Estados permitidos Open/InProgress/Completed/Cancelled.

### Interfaces (Repositorios)

- `ICaseRepository`
- `ICaseActivityRepository`
- `ISlaRepository`
- `IQueueRepository`

### Invariantes

- TenantId requerido en todos los aggregates.
- Case debe referenciar Queue y Sla existentes (tenant-scoped).
- Solo casos abiertos pueden cerrarse; solo casos cerrados pueden reabrirse.
- DueAt se calcula por SLA; el check usa la hora actual.

## Aplicación (EBOS.CRM.Application)

### Contratos (Solicitudes/Respuestas)

- Solicitudes:
  - Case: AddCaseRequest, UpdateCaseRequest, CloseCaseRequest, ReopenCaseRequest, AssignCaseQueueRequest, AssignCaseOwnerRequest.
  - CaseActivity: AddCaseActivityRequest, UpdateCaseActivityRequest.
  - Sla: AddSlaRequest, UpdateSlaRequest, ToggleSlaRequest.
  - Queue: AddQueueRequest, UpdateQueueRequest, ToggleQueueRequest, AssignQueueDefaultOwnerRequest.
- SLA checks: CheckCaseSlaRequest (caseId, now).
  - Multi-tenant: todos los requests deben incluir TenantId cuando aplique el aggregate.
- Respuestas:
  - CaseResponse, CaseActivityResponse, SlaResponse, QueueResponse, SlaCheckResponse.

### Funcionalidades (Comandos/Consultas)

Estructura igual a CRM, por ejemplo `Features/CRM/Service/...`:

- `Features/CRM/Service/Case/Commands/AddCase`
- `Features/CRM/Service/Case/Commands/UpdateCase`
- `Features/CRM/Service/Case/Commands/CloseCase`
- `Features/CRM/Service/Case/Commands/ReopenCase`
- `Features/CRM/Service/Case/Commands/AssignCaseQueue`
- `Features/CRM/Service/Case/Commands/AssignCaseOwner`
- `Features/CRM/Service/Case/Commands/AssignCaseSla`
- `Features/CRM/Service/Case/Queries/GetCaseById`
- `Features/CRM/Service/Case/Queries/GetAllCases`
- `Features/CRM/Service/Case/Commands/DeleteCase`

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

### Mapeo

- `Mappings/CRM/MappingCase`
- `Mappings/CRM/MappingCaseActivity`
- `Mappings/CRM/MappingSla`
- `Mappings/CRM/MappingQueue`

### Validación

- FluentValidation por solicitud.
- Aislamiento por tenant y referencias requeridas.

## API (EBOS.CRM.Api)

### Controladores

Seguir el layout de CRM:

- `Controllers/CRM/Service/Case/CaseController`
- `Controllers/CRM/Service/CaseActivity/CaseActivityController`
- `Controllers/CRM/Service/Sla/SlaController`
- `Controllers/CRM/Service/Queue/QueueController`

### Puntos finales (v2)

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
  - `GET /api/v2/Sla/{id}/check?tenantId=...&caseId=...&now=...`
- Queues
  - `GET /api/v2/Queue`
  - `GET /api/v2/Queue/{id}`
  - `POST /api/v2/Queue`
  - `PUT /api/v2/Queue/{id}`
  - `PATCH /api/v2/Queue/{id}/toggle`
  - `PATCH /api/v2/Queue/{id}/default-owner`

## Infraestructura (EBOS.CRM.Infrastructure)

- Configuraciones EF Core para Case, Sla, Queue.
- Repositorios e inyección en DI.
- Update de `CrmDbContext` y migrations.

## Pruebas (tests/EBOS.CRM.ApiTests)

### Pruebas de dominio

- Ciclo de vida de Case e invariants.
- Cálculo de SLA y comprobación de incumplimiento.
- Activación de Queue.

### Pruebas de aplicación

- Manejadores de comandos: rutas exitosas y errores.
- Validaciones por referencias y tenant.
- Query de SLA devuelve el estado esperado.

### Pruebas de controladores

- CRUD y payloads inválidos.
- Endpoints de close/reopen/assign.

### Pruebas de integración

- Caso extremo a extremo con cola + SLA.
- Aislamiento por tenant en las entidades cases, SLAs, queues.

### Pruebas de mapeo

- Perfiles de AutoMapper para case/case activity/sla/queue.

### Referencia de suites de pruebas existentes

- `tests/EBOS.CRM.ApiTests`: pruebas unitarias y de componentes para manejadores de servicio, validadores, mapeos y controladores (Case, CaseActivity, Sla, Queue), incluyendo ciclo de vida y chequeos SLA.
- `tests/EBOS.CRM.ConcurrencyTests`: escenarios concurrentes en endpoints de servicio para validar cierres/reaperturas/asignaciones seguras y consistencia ante operaciones simultáneas.
- `tests/EBOS.CRM.IntegrationTests`: validación extremo a extremo de flujos de casos con asignación de cola/SLA y persistencia con alcance por tenant entre capas.
- `tests/EBOS.CRM.StressTests`: cobertura de estrés sostenido en controladores de servicio para verificar rendimiento, estabilidad de respuesta y confiabilidad bajo carga intensa.

