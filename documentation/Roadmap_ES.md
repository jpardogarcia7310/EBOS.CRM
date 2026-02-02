# Hoja de ruta EBOS.CRM

Este roadmap se deriva de `documentation/Features.md` y esta pensado para GitHub. Agrupa el trabajo por modulo y luego por hitos con estimacion de esfuerzo.

Leyenda:
- Niveles: MVP, Enterprise, Best-in-Class
- Esfuerzo: S (≤1 semana), M (1–2 semanas), L (3–5 semanas), XL (6–10 semanas)

## Alcance general

- Ventas: Lead → Oportunidad → Cotizacion → Pedido/Contrato → Renovacion, forecast, pipeline, aprobaciones, descuentos.
- Servicio: Casos/tickets, SLA, colas/escalado, base de conocimiento, omnicanal, metricas.
- Marketing: Segmentacion, campanas multicanal, journeys, scoring, consentimientos/opt-out, atribucion.

## Checklist por modulo (priorizado)

1) Seguridad e identidad (bloqueante)
- [ ] MVP: Autenticacion JWT/OIDC + UseAuthentication (Alto Impacto / Esfuerzo Medio)
- [ ] MVP: RBAC basico (roles por modulo) (Alto / Medio)
- [ ] Enterprise: ABAC por entidad/registro (Alto / Alto)
- [ ] Enterprise: MFA + SSO empresarial (Alto / Alto)
- [ ] Enterprise: Auditoria de accesos (Medio / Medio)

2) Multi-tenant y gobernanza
- [ ] MVP: TenantId en entidades + filtros globales (Alto / Alto)
- [ ] Enterprise: Aislamiento por esquema/BD (Alto / Alto)
- [ ] Enterprise: Configuracion por tenant (campos, layouts, reglas) (Alto / Alto)
- [ ] Best: Metricas/cuotas/facturacion por tenant (Medio / Alto)

3) Customer 360 (Core CRM)
- [ ] MVP: Contactos y roles dentro de cuentas (Alto / Medio)
- [ ] MVP: Relacion cuenta‑cuenta (holding/sucursal) (Medio / Medio)
- [ ] Enterprise: Dedupe + merge + golden record (Alto / Alto)
- [ ] Enterprise: Preferencias y consentimientos por canal (Alto / Medio)

4) Pipeline de ventas
- [ ] MVP: Leads (CRUD + conversion) (Alto / Medio)
- [ ] MVP: Oportunidades por etapas (Alto / Medio)
- [ ] MVP: Forecast basico (Medio / Medio)
- [ ] Enterprise: Cotizaciones y descuentos (Alto / Alto)
- [ ] Enterprise: Aprobaciones de precio (Medio / Alto)
- [ ] Best: CPQ completo (Medio / Alto)

5) Servicio (Casos)
- [ ] MVP: Casos/tickets + estados (Alto / Medio)
- [ ] MVP: SLA basico (Alto / Medio)
- [ ] Enterprise: Colas, enrutamiento, escalado (Alto / Alto)
- [ ] Enterprise: Base de conocimiento (Medio / Medio)
- [ ] Best: Omnicanal (email/chat/voz) (Alto / Alto)

6) Marketing
- [ ] MVP: Segmentacion simple (Medio / Medio)
- [ ] MVP: Campanas y mailings basicos (Medio / Medio)
- [ ] Enterprise: Journeys + Triggers (Alto / Alto)
- [ ] Enterprise: Scoring y atribucion (Medio / Alto)
- [ ] Best: CDP basico (Medio / Alto)

7) Integraciones
- [ ] MVP: Webhooks + Domain Events (Medio / Medio)
- [ ] Enterprise: Integracion Email/Calendario (Medio / Alto)
- [ ] Enterprise: ERP/Facturacion (Alto / Alto)

8) Observabilidad y resiliencia
- [ ] MVP: Health checks BD/servicios (Medio / Bajo)
- [ ] MVP: Logs estructurados + correlacion (Medio / Medio)
- [ ] Enterprise: Trazas (OpenTelemetry) (Medio / Medio)
- [ ] Enterprise: Rate limiting, retries, circuit breakers (Alto / Alto)

9) Cumplimiento y auditoria
- [ ] MVP: Auditoria de cambios por entidad (Alto / Alto)
- [ ] Enterprise: GDPR/LPDP (borrado/portabilidad) (Alto / Alto)
- [ ] Enterprise: Retencion de datos (Medio / Medio)

## Hitos (GitHub-ready)

Hito 1 — Fundacion (Seguridad + base multi-tenant)
- [ ] JWT/OIDC Auth + UseAuthentication — M
- [ ] RBAC basico por modulo — M
- [ ] TenantId + filtros globales en EF — L
- [ ] Auditoria de accesos — M
- [ ] Health checks — S

Hito 2 — Customer 360
- [ ] Contactos + roles — M
- [ ] Jerarquias de cuentas — M
- [ ] Preferencias/consentimientos — M
- [ ] Dedupe + merge (golden record) — XL

Hito 3 — Ventas (MVP)
- [ ] Leads + conversion — M
- [ ] Oportunidades + etapas — M
- [ ] Forecast basico — M
- [ ] Cotizaciones y descuentos — L

Hito 4 — Servicio (MVP)
- [ ] Casos + estados — M
- [ ] SLA basico — M
- [ ] Colas y enrutamiento — L
- [ ] Base de conocimiento — M

Hito 5 — Marketing (MVP)
- [ ] Segmentacion simple — M
- [ ] Campanas basicas — M
- [ ] Journeys + Triggers — L
- [ ] Scoring + Atribucion — L

Hito 6 — Operaciones Enterprise
- [ ] Observabilidad (logs estructurados + trazas) — M
- [ ] Rate limiting + retries + circuit breakers — M
- [ ] GDPR/LPDP + retencion — L
- [ ] Webhooks + eventos — M
- [ ] Integracion Email/Calendario — L
- [ ] Integracion ERP — XL
