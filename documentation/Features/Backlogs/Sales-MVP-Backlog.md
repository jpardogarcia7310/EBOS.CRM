# Sales MVP Technical Backlog

Concrete work items aligned with the current local structure (Clean Architecture, CRM module under `EBOS.CRM.*`).

## Scope for MVP

- Leads (capture, qualify, convert to Opportunity).
- Opportunities with stages and amounts.
- Basic pipeline forecast (by stage and owner).

## Domain (EBOS.CRM.Domain)

### Aggregates and entities

- Lead
  - Fields: Id, TenantId, Source, Status, OwnerUserId, CompanyName, ContactName, Email, Phone, EstimatedValue, Notes, CreatedAt, UpdatedAt.
  - Behavior: Qualify, Disqualify (with reason), Convert (creates Opportunity).
- Opportunity
  - Fields: Id, TenantId, Name, StageId, OwnerUserId, AccountId (Corporate/Individual), ExpectedCloseDate, Amount, Probability, SourceLeadId, CreatedAt, UpdatedAt.
  - Behavior: MoveStage, UpdateForecast, CloseWon, CloseLost (with reason).
- OpportunityStage (lookup)
  - Fields: Id, TenantId, Name, Order, DefaultProbability, IsClosed, IsWon.
- ForecastSnapshot (optional MVP if no background jobs)
  - Fields: Id, TenantId, SnapshotDate, OwnerUserId, StageId, TotalAmount, WeightedAmount.

### Interfaces (Repositories)

- `ILeadRepository`
- `IOpportunityRepository`
- `IOpportunityStageRepository`
- `IForecastSnapshotRepository` (optional)

### Invariants

- TenantId required on all aggregates.
- Stage must exist and be tenant-scoped.
- Lead conversion only once and to a single Opportunity.
- Opportunity amount >= 0, probability 0..1.
- CloseWon/CloseLost only allowed from non-closed stages.

## Application (EBOS.CRM.Application)

### Contracts (Requests/Responses)

- Requests:
  - Lead: AddLeadRequest, UpdateLeadRequest, QualifyLeadRequest, DisqualifyLeadRequest, ConvertLeadRequest.
  - Opportunity: AddOpportunityRequest, UpdateOpportunityRequest, PatchOpportunityStageRequest, CloseOpportunityRequest.
  - Stage: AddOpportunityStageRequest, UpdateOpportunityStageRequest.
  - Forecast: GetForecastRequest (date range, owner, stage).
- Responses:
  - LeadResponse, OpportunityResponse, OpportunityStageResponse, ForecastSummaryResponse.

### Features (Commands/Queries)

Structure matches current CRM features, e.g.:

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

- FluentValidation validators per request.
- Reuse tenant isolation behavior (already present).

## API (EBOS.CRM.Api)

### Controllers

Follow existing CRM controllers layout:

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

- EF Core entity configurations for Lead, Opportunity, OpportunityStage.
- Seed default OpportunityStage set (e.g., Prospecting, Qualified, Proposal, Negotiation, Closed Won, Closed Lost).
- Add repository implementations and include in DI.
- Update `CrmDbContext` and migrations.

## Tests (tests/EBOS.CRM.ApiTests)

### Domain tests

- Lead conversion rules and idempotency.
- Opportunity stage transitions and close rules.
- TenantId invariants.

### Application tests

- Command handlers success and error paths.
- Validation coverage for requests.
- Forecast query returns expected aggregates.

### Controller tests

- CRUD happy paths and invalid payloads.
- Stage/close endpoints behavior.

### Integration tests

- End-to-end lead conversion creates opportunity.
- Tenant isolation with multiple tenants.

### Mapping tests

- AutoMapper profiles for lead/opportunity/stage/forecast.
