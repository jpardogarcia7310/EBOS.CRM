# Hoja de ruta EBOS.CRM

Esta hoja de ruta se deriva de `documentation/Features.md` y está pensada para GitHub. Agrupa el trabajo por módulo y luego por hitos con estimación de esfuerzo.

Leyenda:
- Niveles: MVP, Empresarial, Mejor en su clase
- Esfuerzo: S (≤1 semana), M (1–2 semanas), L (3–5 semanas), XL (6–10 semanas)

## Alcance general

- Ventas: Prospecto → Oportunidad → Cotización → Pedido/Contrato → Renovación, pronóstico, embudo, aprobaciones, descuentos.
- Servicio: Casos/tickets, SLA, colas/escalado, base de conocimiento, omnicanal, métricas.
- Marketing: Segmentación, campañas multicanal, recorridos, puntuación, consentimientos/baja, atribución.

## Checklist por módulo (priorizado)

1) Seguridad e identidad (bloqueante)
- [ ] MVP: Autenticación JWT/OIDC + UseAuthentication (Alto Impacto / Esfuerzo Medio)
- [ ] MVP: RBAC básico (roles por módulo) (Alto / Medio)
- [ ] Empresarial: ABAC por entidad/registro (Alto / Alto)
- [ ] Empresarial: MFA + SSO empresarial (Alto / Alto)
- [ ] Empresarial: Auditoría de accesos (Medio / Medio)

2) Multi-tenant y gobernanza
- [x] MVP: TenantId en entidades + filtros globales (Alto / Alto)
- [x] Empresarial: Aislamiento por esquema/BD (Alto / Alto)
- [x] Empresarial: Configuración por tenant (campos, layouts, reglas) (Alto / Alto)
- [x] Mejor en su clase: Métricas/cuotas/facturación por tenant (Medio / Alto)

3) Customer 360 (CRM central)
- [ ] MVP: Contactos y roles dentro de cuentas (Alto / Medio)
- [ ] MVP: Relación cuenta‑cuenta (holding/sucursal) (Medio / Medio)
- [ ] Empresarial: Deduplicación + fusión + registro maestro (Alto / Alto)
- [ ] Empresarial: Preferencias y consentimientos por canal (Alto / Medio)

4) Embudo de ventas
- [ ] MVP: Prospectos (CRUD + conversión) (Alto / Medio)
- [ ] MVP: Oportunidades por etapas (Alto / Medio)
- [ ] MVP: Pronóstico básico (Medio / Medio)
- [ ] Empresarial: Cotizaciones y descuentos (Alto / Alto)
- [ ] Empresarial: Aprobaciones de precio (Medio / Alto)
- [ ] Mejor en su clase: CPQ completo (Medio / Alto)

5) Servicio (Casos)
- [ ] MVP: Casos/tickets + estados (Alto / Medio)
- [ ] MVP: SLA básico (Alto / Medio)
- [ ] Empresarial: Colas, enrutamiento, escalado (Alto / Alto)
- [ ] Empresarial: Base de conocimiento (Medio / Medio)
- [ ] Mejor en su clase: Omnicanal (email/chat/voz) (Alto / Alto)

6) Marketing
- [ ] MVP: Segmentación simple (Medio / Medio)
- [ ] MVP: Campañas y envíos básicos (Medio / Medio)
- [ ] Empresarial: Recorridos + Disparadores (Alto / Alto)
- [ ] Empresarial: Puntuación y atribución (Medio / Alto)
- [ ] Mejor en su clase: CDP básico (Medio / Alto)

7) Integraciones
- [ ] MVP: Ganchos web + eventos de dominio (Medio / Medio)
- [ ] Empresarial: Integración correo/calendario (Medio / Alto)
- [ ] Empresarial: ERP/Facturación (Alto / Alto)

8) Observabilidad y resiliencia
- [ ] MVP: Comprobaciones de salud BD/servicios (Medio / Bajo)
- [ ] MVP: Registros estructurados + correlación (Medio / Medio)
- [ ] Empresarial: Trazas (OpenTelemetry) (Medio / Medio)
- [ ] Empresarial: Limitación de tasa, reintentos, cortacircuitos (Alto / Alto)

9) Cumplimiento y auditoría
- [ ] MVP: Auditoría de cambios por entidad (Alto / Alto)
- [ ] Empresarial: GDPR/LPDP (borrado/portabilidad) (Alto / Alto)
- [ ] Empresarial: Retención de datos (Medio / Medio)

## Hitos (GitHub-ready)

Hito 1 — Fundación (Seguridad + base multi-tenant)
- [ ] JWT/OIDC Auth + UseAuthentication — M
- [ ] RBAC básico por módulo — M
- [x] TenantId + filtros globales en EF — L
- [ ] Auditoría de accesos — M
- [ ] Health checks — S

Hito 2 — Customer 360
- [ ] Contactos + roles — M
- [ ] Jerarquías de cuentas — M
- [ ] Preferencias/consentimientos — M
- [ ] Dedupe + merge (golden record) — XL

Hito 3 — Ventas (MVP)
- [ ] Prospectos + conversión — M
- [ ] Oportunidades + etapas — M
- [ ] Pronóstico básico — M
- [ ] Cotizaciones y descuentos — L

Hito 4 — Servicio (MVP)
- [ ] Casos + estados — M
- [ ] SLA básico — M
- [ ] Colas y enrutamiento — L
- [ ] Base de conocimiento — M

Hito 5 — Marketing (MVP)
- [ ] Segmentación simple — M
- [ ] Campañas básicas — M
- [ ] Recorridos + Disparadores — L
- [ ] Puntuación + Atribución — L

Hito 6 — Operaciones empresariales
- [ ] Observabilidad (registros estructurados + trazas) — M
- [ ] Limitación de tasa + reintentos + cortacircuitos — M
- [ ] GDPR/LPDP + retención — L
- [ ] Ganchos web + eventos — M
- [ ] Integración correo/calendario — L
- [ ] Integración ERP — XL
