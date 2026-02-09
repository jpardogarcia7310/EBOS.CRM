# EBOS.CRM Roadmap

This roadmap is derived from `documentation/Features.md` and is intended to be GitHub-ready. It groups work by module and then by milestones with estimated effort.

Legend:
- Levels: MVP, Enterprise, Best-in-Class
- Effort: S (≤1 week), M (1–2 weeks), L (3–5 weeks), XL (6–10 weeks)

## Scope overview

- Sales: Lead → Opportunity → Quote → Order/Contract → Renewal, forecast, pipeline, approvals, discounts.
- Service: Cases/tickets, SLA, queues/escalation, knowledge base, omnichannel, metrics.
- Marketing: Segmentation, multichannel campaigns, journeys, scoring, consents/opt-out, attribution.

## Module checklist (prioritized)

1) Security and Identity (blocking)
- [ ] MVP: JWT/OIDC Authentication + UseAuthentication (High Impact / Medium Effort)
- [ ] MVP: Basic RBAC (roles per module) (High / Medium)
- [ ] Enterprise: ABAC per entity/record (High / High)
- [ ] Enterprise: MFA + Enterprise SSO (High / High)
- [ ] Enterprise: Access Auditing (Medium / Medium)

2) Multi-tenant & Governance
- [x] MVP: TenantId in entities + global filters (High / High)
- [x] Enterprise: Schema/DB isolation (High / High)
- [x] Enterprise: Config per tenant (fields, layouts, rules) (High / High)
- [x] Best: Metrics/quota/billing per tenant (Medium / High)

3) Customer 360 (Core CRM)
- [ ] MVP: Contactos y roles dentro de cuentas (Alto / Medio)
- [ ] MVP: Relación cuenta‑cuenta (holding/sucursal) (Medio / Medio)
- [ ] Enterprise: Dedupe + merge + golden record (Alto / Alto)
- [ ] Enterprise: Preferencias y consentimientos por canal (Alto / Medio)

4) Sales Pipeline
- [ ] MVP: Leads (CRUD + conversion) (High / Medium)
- [ ] MVP: Staged Opportunities (High / Medium)
- [ ] MVP: Basic Forecast (Medium / Medium)
- [ ] Enterprise: Quotes and Discounts (High / High)
- [ ] Enterprise: Price Approvals (Medium / High)
- [ ] Best: Full CPQ (Medium / High)

5) Service (Cases)
- [ ] MVP: Cases/tickets + statuses (High / Medium)
- [ ] MVP: Basic SLA (High / Medium)
- [ ] Enterprise: Queues, routing, escalation (High / High)
- [ ] Enterprise: Knowledge base (Medium / Medium)
- [ ] Best: Omnichannel (email/chat/voice) (High / High)

6) Marketing
- [ ] MVP: Simple Segmentation (Medium / Medium)
- [ ] MVP: Basic Campaigns and Mailings (Medium / Medium)
- [ ] Enterprise: Journeys + Triggers (High / High)
- [ ] Enterprise: Scoring and Attribution (Medium / High)
- [ ] Best: Basic CDP (Medium / High)

7) Integrations
- [ ] MVP: Webhooks + Domain Events (Medium / Medium)
- [ ] Enterprise: Email/Calendar Integration (Medium / High)
- [ ] Enterprise: ERP/Billing (High / High)

8) Observability & Resilience
- [ ] MVP: Health checks DB/services (Medium / Low)
- [ ] MVP: Structured logging + correlation (Medium / Medium)
- [ ] Enterprise: Tracing (OpenTelemetry) (Medium / Medium)
- [ ] Enterprise: Rate limiting, retries, circuit breakers (High / High)

9) Compliance & Audit
- [ ] MVP: Audit of changes by entity (High / High)
- [ ] Enterprise: GDPR/LPDP (deletion/portability) (High / High)
- [ ] Enterprise: Data retention (Medium / Medium)

## Milestones (GitHub-ready)

Milestone 1 — Foundation (Security + Multi-tenant base)
- [ ] JWT/OIDC Auth + UseAuthentication — M
- [ ] Basic RBAC per module — M
- [x] TenantId + global filters in EF — L
- [ ] Access Auditing — M
- [ ] Health checks — S

Milestone 2 — Customer 360
- [ ] Contacts + roles — M
- [ ] Account hierarchies — M
- [ ] Preferences/consents — M
- [ ] Dedupe + merge (golden record) — XL

Milestone 3 — Sales (MVP)
- [ ] Leads + conversion — M
- [ ] Opportunities + stages — M
- [ ] Basic forecast — M
- [ ] Quotes and discounts — L

Milestone 4 — Service (MVP)
- [ ] Cases + States — M
- [ ] Basic SLA — M
- [ ] Queues and Routing — L
- [ ] Knowledge Base — M

Milestone 5 — Marketing (MVP)
- [ ] Simple Segmentation — M
- [ ] Basic Campaigns — M
- [ ] Journeys + Triggers — L
- [ ] Scoring + Attribution — L

Milestone 6 — Enterprise Ops
- [ ] Observability (structured logs + tracing) — M
- [ ] Rate limiting + retries + circuit breakers — M
- [ ] GDPR/LPDP + retention — L
- [ ] Webhooks + events — M
- [ ] Email/calendar integration — L
- [ ] ERP integration — XL
