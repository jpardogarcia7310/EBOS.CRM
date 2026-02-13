# Funcionalidades y hoja de ruta

Definición de alcance Enterprise (Ventas, Servicio, Marketing), con un roadmap listo para GitHub y estimaciones por tarea.

## Alcance

- Ventas: Lead -> Oportunidad -> Cotización -> Pedido/Contrato -> Renovación. Forecast, pipeline, equipo/territorios, reglas de aprobación, descuentos.
- Servicio: Casos/tickets, SLA, colas y escalamiento, base de conocimiento, omnicanal (email/chat/voz), métricas.
- Marketing: Segmentación dinámica, campañas multicanal, journeys, scoring, consentimientos/opt-out, atribución.

## Notas de organización del código

- Las features de esquema EBOS viven en `EBOS.CRM.Application/Features/EBOS/`.
- Las features de esquema CRM viven en `EBOS.CRM.Application/Features/CRM/`.

## Checklist por módulo (niveles + impacto/esfuerzo)

Leyenda:
- Niveles: MVP, Enterprise, Best-in-Class.
- Impacto/Esfuerzo: Bajo/Medio/Alto.

### 1) Seguridad e identidad (bloqueante)
- [ ] MVP: Autenticación JWT/OIDC + UseAuthentication (Impacto Alto / Esfuerzo Medio)
- [ ] MVP: RBAC básico (roles por módulo) (Alto / Medio)
- [ ] Enterprise: ABAC por entidad/registro (Alto / Alto)
- [ ] Enterprise: MFA + SSO corporativo (Alto / Alto)
- [ ] Enterprise: Auditoría de accesos (Medio / Medio)

### 2) Multi-tenant y gobierno
- [x] MVP: TenantId en entidades + filtros globales (Alto / Alto)
- [x] Enterprise: Aislamiento por schema/DB (Alto / Alto)
- [x] Enterprise: Config por tenant (campos, layouts, reglas) (Alto / Alto)
- [x] Best: Métricas/cuota/facturación por tenant (Medio / Alto)

### 3) Customer 360 (Core CRM)
- [ ] MVP: Contactos y roles dentro de cuentas (Alto / Medio)
- [ ] MVP: Relación cuenta-cuenta (holding/sucursal) (Medio / Medio)
- [ ] Enterprise: Dedupe + merge + golden record (Alto / Alto)
- [ ] Enterprise: Preferencias y consentimientos por canal (Alto / Medio)

### 4) Ventas (Pipeline)
- [x] MVP: Leads (CRUD + conversión) (Alto / Medio)
- [x] MVP: Oportunidades con etapas (Alto / Medio)
- [x] MVP: Forecast básico (Medio / Medio)
- [x] Enterprise: Cotizaciones y descuentos (Alto / Alto)
- [x] Enterprise: Aprobaciones de precio (Medio / Alto)
- [x] Best: CPQ completo (Medio / Alto)

### 5) Servicio (Casos)
- [x] MVP: Casos/tickets + estados (Alto / Medio)
- [x] MVP: SLA básico (Alto / Medio)
- [x] Enterprise: Colas, routing, escalamiento (Alto / Alto)
- [x] Enterprise: Base de conocimiento (Medio / Medio)
- [x] Best: Omnicanal (email/chat/voz) (Alto / Alto)

### 6) Marketing
- [ ] MVP: Segmentación simple (Medio / Medio)
- [ ] MVP: Campañas y envíos básicos (Medio / Medio)
- [ ] Enterprise: Journeys + triggers (Alto / Alto)
- [ ] Enterprise: Scoring y atribución (Medio / Alto)
- [ ] Best: CDP básico (Medio / Alto)

### 7) Integraciones
- [ ] MVP: Webhooks + eventos de dominio (Medio / Medio)
- [ ] Enterprise: Integración email/calendario (Medio / Alto)
- [ ] Enterprise: ERP/Facturación (Alto / Alto)

### 8) Observabilidad y resiliencia
- [ ] MVP: Health checks DB/servicios (Medio / Bajo)
- [ ] MVP: Logging estructurado + correlación (Medio / Medio)
- [ ] Enterprise: Tracing (OpenTelemetry) (Medio / Medio)
- [ ] Enterprise: Rate limiting, retries, circuit breakers (Alto / Alto)

### 9) Cumplimiento y auditoría
- [ ] MVP: Auditoría de cambios por entidad (Alto / Alto)
- [ ] Enterprise: GDPR/LPDP (borrado/portabilidad) (Alto / Alto)
- [ ] Enterprise: Retención de datos (Medio / Medio)

## Roadmap para GitHub (esfuerzo estimado)

Formato: M (1-2 sem), L (3-5 sem), XL (6-10 sem), S (<=1 sem).

### Milestone 1 - Foundation (Seguridad + base multi-tenant)
- [x] Auth JWT/OIDC + UseAuthentication - M
- [x] RBAC básico por módulo - M
- [x] TenantId + filtros globales en EF - L
- [x] Auditoría de accesos - M
- [x] Health checks - S

### Milestone 2 - Customer 360
- [ ] Contactos + roles - M
- [ ] Jerarquías de cuentas - M
- [ ] Preferencias/consentimientos - M
- [ ] Dedupe + merge (golden record) - XL

### Milestone 3 - Ventas (MVP)
- [x] Leads + conversión - M
- [x] Oportunidades + etapas - M
- [x] Forecast básico - M
- [x] Cotizaciones y descuentos - L

### Milestone 4 - Servicio (MVP)
- [x] Casos + estados - M
- [x] SLA básico - M
- [x] Colas y routing - L
- [x] Base de conocimiento - M

### Milestone 5 - Marketing (MVP)
- [ ] Segmentación simple - M
- [ ] Campañas básicas - M
- [ ] Journeys + triggers - L
- [ ] Scoring + atribución - L

### Milestone 6 - Enterprise Ops
- [ ] Observabilidad (logs estructurados + tracing) - M
- [ ] Rate limiting + retries + circuit breakers - M
- [ ] GDPR/LPDP + retención - L
- [ ] Webhooks + eventos - M
- [ ] Integración email/calendario - L
- [ ] Integración ERP - XL
