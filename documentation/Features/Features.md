# Features and Roadmap

Enterprise scope definition (Sales, Service, Marketing), with a GitHub-ready roadmap and estimates per task.

## Scope

- Sales: Lead -> Opportunity -> Quote -> Order/Contract -> Renewal. Forecast, pipeline, team/territories, approval rules, discounts.
- Service: Cases/tickets, SLA, queues and escalation, knowledge base, omnichannel (email/chat/voice), metrics.
- Marketing: Dynamic segmentation, multichannel campaigns, journeys, scoring, consents/opt-out, attribution.

## Checklist by module (levels + impact/effort)

Legend:
- Levels: MVP, Enterprise, Best-in-Class.
- Impact/Effort: Low/Medium/High.

### 1) Security and Identity (blocking)
- [ ] MVP: JWT/OIDC Authentication + UseAuthentication (High Impact / Medium Effort)
- [ ] MVP: Basic RBAC (roles per module) (High / Medium)
- [ ] Enterprise: ABAC per entity/record (High / High)
- [ ] Enterprise: MFA + Enterprise SSO (High / High)
- [ ] Enterprise: Access Auditing (Medium / Medium)

### 2) Multi-tenant and Governance
- [ ] MVP: TenantId in entities + global filters (High / High)
- [ ] Enterprise: Schema/DB isolation (High / High)
- [ ] Enterprise: Config per tenant (fields, layouts, rules) (High / High)
- [ ] Best: Metrics/quota/billing per tenant (Medium / High)

### 3) Customer 360 (Core CRM)
- [ ] MVP: Contacts and roles inside accounts (High / Medium)
- [ ] MVP: Account-to-account relation (holding/subsidiary) (Medium / Medium)
- [ ] Enterprise: Dedupe + merge + golden record (High / High)
- [ ] Enterprise: Channel preferences and consents (High / Medium)

### 4) Sales Pipeline
- [ ] MVP: Leads (CRUD + conversion) (High / Medium)
- [ ] MVP: Staged Opportunities (High / Medium)
- [ ] MVP: Basic Forecast (Medium / Medium)
- [ ] Enterprise: Quotes and Discounts (High / High)
- [ ] Enterprise: Price Approvals (Medium / High)
- [ ] Best: Full CPQ (Medium / High)

### 5) Service (Cases)
- [ ] MVP: Cases/tickets + statuses (High / Medium)
- [ ] MVP: Basic SLA (High / Medium)
- [ ] Enterprise: Queues, routing, escalation (High / High)
- [ ] Enterprise: Knowledge base (Medium / Medium)
- [ ] Best: Omnichannel (email/chat/voice) (High / High)

### 6) Marketing
- [ ] MVP: Simple Segmentation (Medium / Medium)
- [ ] MVP: Basic Campaigns and Mailings (Medium / Medium)
- [ ] Enterprise: Journeys + Triggers (High / High)
- [ ] Enterprise: Scoring and Attribution (Medium / High)
- [ ] Best: Basic CDP (Medium / High)

### 7) Integrations
- [ ] MVP: Webhooks + Domain Events (Medium / Medium)
- [ ] Enterprise: Email/Calendar Integration (Medium / High)
- [ ] Enterprise: ERP/Billing (High / High)

### 8) Observability and Resilience
- [ ] MVP: Health checks DB/services (Medium / Low)
- [ ] MVP: Structured logging + correlation (Medium / Medium)
- [ ] Enterprise: Tracing (OpenTelemetry) (Medium / Medium)
- [ ] Enterprise: Rate limiting, retries, circuit breakers (High / High)

### 9) Compliance and Audit
- [ ] MVP: Audit of changes by entity (High / High)
- [ ] Enterprise: GDPR/LPDP (deletion/portability) (High / High)
- [ ] Enterprise: Data retention (Medium / Medium)

## Roadmap for GitHub (estimated effort)

Format: M (1-2 weeks), L (3-5 weeks), XL (6-10 weeks), S (<=1 week).

### Milestone 1 - Foundation (Security + Multi-tenant base)
- [ ] JWT/OIDC Auth + UseAuthentication - M
- [ ] Basic RBAC per module - M
- [ ] TenantId + global filters in EF - L
- [ ] Access Auditing - M
- [ ] Health checks - S

### Milestone 2 - Customer 360
- [ ] Contacts + roles - M
- [ ] Account hierarchies - M
- [ ] Preferences/consents - M
- [ ] Dedupe + merge (golden record) - XL

### Milestone 3 - Sales (MVP)
- [ ] Leads + conversion - M
- [ ] Opportunities + stages - M
- [ ] Basic forecast - M
- [ ] Quotes and discounts - L

### Milestone 4 - Service (MVP)
- [ ] Cases + States - M
- [ ] Basic SLA - M
- [ ] Queues and Routing - L
- [ ] Knowledge Base - M

### Milestone 5 - Marketing (MVP)
- [ ] Simple Segmentation - M
- [ ] Basic Campaigns - M
- [ ] Journeys + Triggers - L
- [ ] Scoring + Attribution - L

### Milestone 6 - Enterprise Ops
- [ ] Observability (structured logs + tracing) - M
- [ ] Rate limiting + retries + circuit breakers - M
- [ ] GDPR/LPDP + retention - L
- [ ] Webhooks + events - M
- [ ] Email/calendar integration - L
- [ ] ERP integration - XL
