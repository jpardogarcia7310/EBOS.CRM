# Backlog tecnico Sales MVP

Items concretos alineados con la estructura local (Clean Architecture, modulo CRM bajo `EBOS.CRM.*`).

## Alcance MVP

- Leads (captura, calificacion, conversion a Opportunity).
- Opportunities con etapas y montos.
- Forecast basico de pipeline (por etapa y owner).

## Domain (EBOS.CRM.Domain)

### Aggregates y entidades

- Lead
  - Campos: Id, TenantId, Source, Status, OwnerUserId, CompanyName, ContactName, Email, Phone, EstimatedValue, Notes, CreatedAt, UpdatedAt.
  - Comportamiento: Qualify, Disqualify (con motivo), Convert (crea Opportunity).
- Opportunity
  - Campos: Id, TenantId, Name, StageId, OwnerUserId, AccountId (Corporate/Individual), ExpectedCloseDate, Amount, Probability, SourceLeadId, CreatedAt, UpdatedAt.
  - Comportamiento: MoveStage, UpdateForecast, CloseWon, CloseLost (con motivo).
- OpportunityStage (lookup)
  - Campos: Id, TenantId, Name, Order, DefaultProbability, IsClosed, IsWon.
- ForecastSnapshot (opcional MVP si no hay background jobs)
  - Campos: Id, TenantId, SnapshotDate, OwnerUserId, StageId, TotalAmount, WeightedAmount.

### Interfaces (Repositories)

- `ILeadRepository`
- `IOpportunityRepository`
- `IOpportunityStageRepository`
- `IForecastSnapshotRepository` (opcional)

### Invariants

- TenantId requerido en todos los aggregates.
- Stage debe existir y ser tenant-scoped.
- Lead conversion solo una vez y a una unica Opportunity.
- Opportunity amount >= 0, probability 0..1.
- CloseWon/CloseLost solo desde etapas no cerradas.

## Application (EBOS.CRM.Application)

### Contracts (Requests/Responses)

- Requests:
  - Lead: AddLeadRequest, UpdateLeadRequest, QualifyLeadRequest, DisqualifyLeadRequest, ConvertLeadRequest.
  - Opportunity: AddOpportunityRequest, UpdateOpportunityRequest, PatchOpportunityStageRequest, CloseOpportunityRequest.
  - Stage: AddOpportunityStageRequest, UpdateOpportunityStageRequest.
  - Forecast: GetForecastRequest (rango de fechas, owner, stage).
- Responses:
  - LeadResponse, OpportunityResponse, OpportunityStageResponse, ForecastSummaryResponse.

### Features (Commands/Queries)

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

### Mapping

- `Mappings/CRM/MappingLead`
- `Mappings/CRM/MappingOpportunity`
- `Mappings/CRM/MappingOpportunityStage`
- `Mappings/CRM/MappingForecast`

### Validation

- Validators de FluentValidation por request.
- Reusar tenant isolation behavior (ya existe).

## API (EBOS.CRM.Api)

### Controllers

Seguir el layout actual de controllers CRM:

- `Controllers/CRM/Lead/LeadController`
- `Controllers/CRM/Opportunity/OpportunityController`
- `Controllers/CRM/OpportunityStage/OpportunityStageController`
- `Controllers/CRM/Forecast/ForecastController`

### Endpoints (v1)

- Leads
  - `GET /api/v1/Lead`
  - `GET /api/v1/Lead/{id}`
  - `POST /api/v1/Lead`
  - `PUT /api/v1/Lead/{id}`
  - `PATCH /api/v1/Lead/{id}/qualify`
  - `PATCH /api/v1/Lead/{id}/disqualify`
  - `POST /api/v1/Lead/{id}/convert`
- Opportunities
  - `GET /api/v1/Opportunity`
  - `GET /api/v1/Opportunity/{id}`
  - `POST /api/v1/Opportunity`
  - `PUT /api/v1/Opportunity/{id}`
  - `PATCH /api/v1/Opportunity/{id}/stage`
  - `PATCH /api/v1/Opportunity/{id}/close`
- Opportunity Stages
  - `GET /api/v1/OpportunityStage`
  - `POST /api/v1/OpportunityStage`
  - `PUT /api/v1/OpportunityStage/{id}`
- Forecast
  - `GET /api/v1/Forecast?from=...&to=...&ownerUserId=...&stageId=...`

## Infrastructure (EBOS.CRM.Infrastructure)

- EF Core entity configurations para Lead, Opportunity, OpportunityStage.
- Seed de OpportunityStage por defecto (Prospecting, Qualified, Proposal, Negotiation, Closed Won, Closed Lost).
- Implementaciones de repositorios y registro en DI.
- Actualizar `CrmDbContext` y migrations.

## Tests (tests/EBOS.CRM.ApiTests)

### Domain tests

- Reglas de conversion de Lead e idempotencia.
- Transiciones de etapa y reglas de cierre de Opportunity.
- Invariants de TenantId.

### Application tests

- Command handlers para success y error paths.
- Validaciones por request.
- Forecast query devuelve agregados esperados.

### Controller tests

- CRUD happy paths y payloads invalidos.
- Endpoints de stage/close.

### Integration tests

- Conversion end-to-end de Lead crea Opportunity.
- Tenant isolation con multiples tenants.

### Mapping tests

- AutoMapper profiles para lead/opportunity/stage/forecast.
