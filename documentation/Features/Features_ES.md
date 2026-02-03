Definición de alcance Enterprise (Ventas, Servicio, Marketing), con un roadmap listo para GitHub con estimaciones por tarea.

•Ventas: Lead → Oportunidad → Cotización → Pedido/Contrato → Renovación. Forecast, pipeline, equipo/territorios, 
    reglas de aprobación, descuentos.
•Servicio: Casos/tickets, SLA, colas y escalamiento, base de conocimiento, omnicanal (email/chat/voz), métricas.
•Marketing: Segmentación dinámica, campañas multicanal, journeys, scoring, consentimientos/opt‑out, atribución.

Checklist priorizado por módulo (Niveles + Impacto/Esfuerzo)
Leyenda niveles: MVP, Enterprise, Best‑in‑Class.
Impacto/Esfuerzo: Bajo/Medio/Alto.

1) Seguridad e Identidad (bloqueante)
•[ ] MVP: Autenticación JWT/OIDC + UseAuthentication (Impacto Alto / Esfuerzo Medio)
•[ ] MVP: RBAC básico (roles por módulo) (Alto / Medio)
•[ ] Enterprise: ABAC por entidad/registro (Alto / Alto)
•[ ] Enterprise: MFA + SSO corporativo (Alto / Alto)
•[ ] Enterprise: Auditoría de accesos (Medio / Medio)

2) Multi‑tenant & Gobierno
•[ ] MVP: TenantId en entidades + filtros globales (Alto / Alto)
•[ ] Enterprise: Aislamiento por schema/DB (Alto / Alto)
•[ ] Enterprise: Config por tenant (campos, layouts, reglas) (Alto / Alto)
•[ ] Best: Metricas/quota/billing por tenant (Medio / Alto)

3) Customer 360 (Core CRM)
•[ ] MVP: Contactos y roles dentro de cuentas (Alto / Medio)
•[ ] MVP: Relación cuenta‑cuenta (holding/sucursal) (Medio / Medio)
•[ ] Enterprise: Dedupe + merge + golden record (Alto / Alto)
•[ ] Enterprise: Preferencias y consentimientos por canal (Alto / Medio)

4) Ventas (Pipeline)
•[ ] MVP: Leads (CRUD + conversión) (Alto / Medio)
•[ ] MVP: Oportunidades con etapas (Alto / Medio)
•[ ] MVP: Forecast básico (Medio / Medio)
•[ ] Enterprise: Cotizaciones y descuentos (Alto / Alto)
•[ ] Enterprise: Aprobaciones de precio (Medio / Alto)
•[ ] Best: CPQ completo (Medio / Alto)

5) Servicio (Casos)
•[ ] MVP: Casos/tickets + estados (Alto / Medio)
•[ ] MVP: SLA básico (Alto / Medio)
•[ ] Enterprise: Colas, routing, escalamiento (Alto / Alto)
•[ ] Enterprise: Base de conocimiento (Medio / Medio)
•[ ] Best: Omnicanal (email/chat/voz) (Alto / Alto)

6) Marketing
•[ ] MVP: Segmentación simple (Medio / Medio)
•[ ] MVP: Campañas y envíos básicos (Medio / Medio)
•[ ] Enterprise: Journeys + triggers (Alto / Alto)
•[ ] Enterprise: Scoring y atribución (Medio / Alto)
•[ ] Best: CDP básico (Medio / Alto)

7) Integraciones
•[ ] MVP: Webhooks + eventos de dominio (Medio / Medio)
•[ ] Enterprise: Integración email/calendario (Medio / Alto)
•[ ] Enterprise: ERP/Facturación (Alto / Alto)

8) Observabilidad & Resiliencia
•[ ] MVP: Health checks DB/servicios (Medio / Bajo)
•[ ] MVP: Logging estructurado + correlación (Medio / Medio)
•[ ] Enterprise: Tracing (OpenTelemetry) (Medio / Medio)
•[ ] Enterprise: Rate limiting, retries, circuit breakers (Alto / Alto)

9) Compliance & Auditoría
•[ ] MVP: Auditoría de cambios por entidad (Alto / Alto)
•[ ] Enterprise: GDPR/LPDP (borrado/portabilidad) (Alto / Alto)
•[ ] Enterprise: Retención de datos (Medio / Medio)

Roadmap para GitHub (listas con esfuerzo estimado)
Formato: M (1–2 sem), L (3–5 sem), XL (6–10 sem).

Milestone 1 — Foundation (Seguridad + Multi‑tenant base)
•[ ] Auth JWT/OIDC + UseAuthentication — M
•[ ] RBAC básico por módulo — M
•[ ] TenantId + filtros globales en EF — L
•[ ] Auditoría de accesos — M
•[ ] Health checks — S (≤1 sem)

Milestone 2 — Customer 360
•[ ] Contactos + roles — M
•[ ] Jerarquías de cuentas — M
•[ ] Preferencias/consentimientos — M
•[ ] Dedupe + merge (golden record) — XL

Milestone 3 — Ventas (MVP)
•[ ] Leads + conversión — M
•[ ] Oportunidades + etapas — M
•[ ] Forecast básico — M
•[ ] Cotizaciones y descuentos — L

Milestone 4 — Servicio (MVP)
•[ ] Casos + estados — M
•[ ] SLA básico — M
•[ ] Colas y routing — L
•[ ] Base de conocimiento — M

Milestone 5 — Marketing (MVP)
•[ ] Segmentación simple — M
•[ ] Campañas básicas — M
•[ ] Journeys + triggers — L
•[ ] Scoring + atribución — L

Milestone 6 — Enterprise Ops
•[ ] Observabilidad (logs estructurados + tracing) — M
•[ ] Rate limiting + retries + circuit breakers — M
•[ ] GDPR/LPDP + retención — L
•[ ] Webhooks + eventos — M
•[ ] Integración email/calendario — L
•[ ] Integración ERP — XL