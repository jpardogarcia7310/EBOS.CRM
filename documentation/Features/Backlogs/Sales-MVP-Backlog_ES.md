# Backlog técnico Ventas MVP

Items concretos alineados con la estructura local (Clean Architecture, módulo CRM bajo `EBOS.CRM.*`).

Mini TOC:
1. [Alcance MVP](#alcance-mvp)
2. [Dominio](#dominio-eboscrmdomain)
3. [Agregados y entidades](#agregados-y-entidades)
4. [Interfaces](#interfaces-repositorios)
5. [Invariantes](#invariantes)
6. [Aplicación](#aplicacion-eboscrmapplication)
7. [Contratos](#contratos-solicitudesrespuestas)
8. [Funcionalidades](#funcionalidades-comandosconsultas)
9. [Mapeo](#mapeo)
10. [Validación](#validacion)
11. [API](#api-eboscrmapi)
12. [Controladores](#controladores)
13. [Puntos finales](#puntos-finales-v1)
14. [Infraestructura](#infraestructura-eboscrminfrastructure)
15. [Pruebas](#pruebas-testseboscrmapitests)
16. [Pruebas de dominio](#pruebas-de-dominio)
17. [Pruebas de aplicación](#pruebas-de-aplicacion)
18. [Pruebas de controladores](#pruebas-de-controladores)
19. [Pruebas de integración](#pruebas-de-integracion)
20. [Pruebas de mapeo](#pruebas-de-mapeo)
21. [Referencia de suites de pruebas existentes](#referencia-de-suites-de-pruebas-existentes)

## Alcance MVP

- Prospectos (captura, calificación, conversión a Opportunity).
- Oportunidades con etapas y montos.
- Pronóstico básico de embudo (por etapa y responsable).

## Dominio (EBOS.CRM.Domain)

### Agregados y entidades

- Lead
  - Campos: Id, TenantId, Source, Status, OwnerUserId, CompanyName, ContactName, Email, Phone, EstimatedValue, Notes, CreatedAt, UpdatedAt.
  - Comportamiento: Qualify, Disqualify (con motivo), Convert (crea Opportunity).
- Opportunity
  - Campos: Id, TenantId, Name, StageId, OwnerUserId, AccountId (Corporate/Individual), ExpectedCloseDate, Amount, Probability, SourceLeadId, CreatedAt, UpdatedAt.
  - Comportamiento: MoveStage, UpdateForecast, CloseWon, CloseLost (con motivo).
- OpportunityStage (tabla de referencia)
  - Campos: Id, TenantId, Name, Order, DefaultProbability, IsClosed, IsWon.
- ForecastSnapshot (opcional MVP si no hay tareas en segundo plano)
  - Campos: Id, TenantId, SnapshotDate, OwnerUserId, StageId, TotalAmount, WeightedAmount.

### Interfaces (Repositorios)

- `ILeadRepository`
- `IOpportunityRepository`
- `IOpportunityStageRepository`
- `IForecastSnapshotRepository` (opcional)

### Invariantes

- TenantId requerido en todos los aggregates.
- Stage debe existir y ser tenant-scoped.
- Lead conversión solo una vez y a una única Opportunity.
- Monto de Opportunity >= 0, probabilidad 0..1.
- CloseWon/CloseLost solo desde etapas no cerradas.

## Aplicación (EBOS.CRM.Application)

### Contratos (Solicitudes/Respuestas)

- Solicitudes:
  - Lead: AddLeadRequest, UpdateLeadRequest, QualifyLeadRequest, DisqualifyLeadRequest, ConvertLeadRequest.
  - Opportunity: AddOpportunityRequest, UpdateOpportunityRequest, PatchOpportunityStageRequest, CloseOpportunityRequest.
  - Stage: AddOpportunityStageRequest, UpdateOpportunityStageRequest.
- Pronóstico: GetForecastRequest (rango de fechas, responsable, etapa).
- Respuestas:
  - LeadResponse, OpportunityResponse, OpportunityStageResponse, ForecastSummaryResponse.

### Funcionalidades (Comandos/Consultas)

Estructura igual a los features CRM actuales:

- `Features/CRM/Lead/Commands/AddLead`
- `Features/CRM/Lead/Commands/UpdateLead`
- `Features/CRM/Lead/Commands/QualifyLead`
- `Features/CRM/Lead/Commands/DisqualifyLead`
- `Features/CRM/Lead/Commands/ConvertLead`
- `Features/CRM/Lead/Queries/GetLeadById`
- `Features/CRM/Lead/Queries/GetAllLeads`

- `Features/CRM/Opportunity/Commands/AddOpportunity`
- `Features/CRM/Opportunity/Commands/UpdateOpportunity`
- `Features/CRM/Opportunity/Commands/PatchOpportunityStage`
- `Features/CRM/Opportunity/Commands/CloseOpportunity`
- `Features/CRM/Opportunity/Queries/GetOpportunityById`
- `Features/CRM/Opportunity/Queries/GetAllOpportunities`

- `Features/CRM/OpportunityStage/Commands/AddOpportunityStage`
- `Features/CRM/OpportunityStage/Commands/UpdateOpportunityStage`
- `Features/CRM/OpportunityStage/Queries/GetAllOpportunityStages`

- `Features/CRM/Forecast/Queries/GetForecastSummary`

### Mapeo

- `Mappings/CRM/MappingLead`
- `Mappings/CRM/MappingOpportunity`
- `Mappings/CRM/MappingOpportunityStage`
- `Mappings/CRM/MappingForecast`

### Validación

- Validadores de FluentValidation por solicitud.
- Reusar el comportamiento de aislamiento de tenant (ya existe).

## API (EBOS.CRM.Api)

### Controladores

Seguir el layout actual de controllers CRM:

- `Controllers/CRM/Lead/LeadController`
- `Controllers/CRM/Opportunity/OpportunityController`
- `Controllers/CRM/OpportunityStage/OpportunityStageController`
- `Controllers/CRM/Forecast/ForecastController`

### Puntos finales (v1)

- Prospectos
  - `GET /api/v1/Lead`
  - `GET /api/v1/Lead/{id}`
  - `POST /api/v1/Lead`
  - `PUT /api/v1/Lead/{id}`
  - `PATCH /api/v1/Lead/{id}/qualify`
  - `PATCH /api/v1/Lead/{id}/disqualify`
  - `POST /api/v1/Lead/{id}/convert`
- Oportunidades
  - `GET /api/v1/Opportunity`
  - `GET /api/v1/Opportunity/{id}`
  - `POST /api/v1/Opportunity`
  - `PUT /api/v1/Opportunity/{id}`
  - `PATCH /api/v1/Opportunity/{id}/stage`
  - `PATCH /api/v1/Opportunity/{id}/close`
- Etapas de oportunidad
  - `GET /api/v1/OpportunityStage`
  - `POST /api/v1/OpportunityStage`
  - `PUT /api/v1/OpportunityStage/{id}`
- Pronóstico
  - `GET /api/v1/Forecast?from=...&to=...&ownerUserId=...&stageId=...`

## Infraestructura (EBOS.CRM.Infrastructure)

- Configuraciones de entidades EF Core para Lead, Opportunity, OpportunityStage.
- Carga inicial de OpportunityStage por defecto (Prospecting, Qualified, Proposal, Negotiation, Closed Won, Closed Lost).
- Implementaciones de repositorios y registro en DI.
- Actualizar `CrmDbContext` y migraciones.

## Pruebas (tests/EBOS.CRM.ApiTests)

### Pruebas de dominio

- Reglas de conversión de Lead e idempotencia.
- Transiciones de etapa y reglas de cierre de Opportunity.
- Invariants de TenantId.

### Pruebas de aplicación

- Manejadores de comandos para rutas exitosas y de error.
- Validaciones por solicitud.
- La consulta de pronóstico devuelve agregados esperados.

### Pruebas de controladores

- CRUD con rutas exitosas y cargas útiles inválidas.
- Endpoints de stage/close.

### Pruebas de integración

- Conversión extremo a extremo de Lead crea Opportunity.
- Tenant isolation con múltiples tenants.

### Pruebas de mapeo

- Perfiles de AutoMapper para lead/opportunity/stage/forecast.

### Referencia de suites de pruebas existentes

- `tests/EBOS.CRM.ApiTests`: cobertura unitaria y de componentes para comandos/consultas de ventas, validadores, mapeos y controladores (Lead, Opportunity, OpportunityStage, Forecast).
- `tests/EBOS.CRM.ConcurrencyTests`: escenarios concurrentes sobre endpoints de ventas para validar transiciones de etapa, cierres y consistencia ante actualizaciones simultáneas.
- `tests/EBOS.CRM.IntegrationTests`: flujos extremo a extremo de conversión de lead, ciclo de vida de opportunity y consulta de forecast con persistencia real.
- `tests/EBOS.CRM.StressTests`: ejecución de alto volumen sobre controladores de ventas para validar rendimiento sostenido, estabilidad de latencia y tasa de errores bajo carga.
