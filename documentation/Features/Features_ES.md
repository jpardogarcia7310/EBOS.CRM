# Funcionalidades y hoja de ruta

Definición de alcance Empresarial (Ventas, Servicio, Marketing), con una hoja de ruta lista para GitHub y estimaciones por tarea.

## Alcance

- Ventas: Prospecto -> Oportunidad -> Cotización -> Pedido/Contrato -> Renovación. Pronóstico, embudo, equipo/territorios, reglas de aprobación, descuentos.
- Servicio: Casos/tickets, SLA, colas y escalamiento, base de conocimiento, omnicanal (correo/chat/voz), métricas.
- Marketing: Segmentación dinámica, campañas multicanal, recorridos, puntuación, consentimientos/baja, atribución.

## Notas de organización del código

- Las funcionalidades de esquema EBOS viven en `EBOS.CRM.Application/Features/EBOS/`.
- Las funcionalidades de esquema CRM viven en `EBOS.CRM.Application/Features/CRM/`.

## Checklist por módulo (niveles + impacto/esfuerzo)

Leyenda:
- Niveles: MVP, Empresarial, Mejor en su clase.
- Impacto/Esfuerzo: Bajo/Medio/Alto.

### 1) Seguridad e identidad (bloqueante)
- [ ] MVP: Autenticación JWT/OIDC + UseAuthentication (Impacto Alto / Esfuerzo Medio)
- [ ] MVP: RBAC básico (roles por módulo) (Alto / Medio)
- [ ] Empresarial: ABAC por entidad/registro (Alto / Alto)
- [ ] Empresarial: MFA + SSO corporativo (Alto / Alto)
- [ ] Empresarial: Auditoría de accesos (Medio / Medio)

### 2) Multi-tenant y gobierno
- [x] MVP: TenantId en entidades + filtros globales (Alto / Alto)
- [x] Empresarial: Aislamiento por esquema/BD (Alto / Alto)
- [x] Empresarial: Configuración por tenant (campos, diseños, reglas) (Alto / Alto)
- [x] Mejor en su clase: Métricas/cuota/facturación por tenant (Medio / Alto)

### 3) Customer 360 (Core CRM)
- [ ] MVP: Contactos y roles dentro de cuentas (Alto / Medio)
- [ ] MVP: Relación cuenta-cuenta (holding/sucursal) (Medio / Medio)
- [ ] Empresarial: Deduplicación + fusión + registro maestro (Alto / Alto)
- [ ] Empresarial: Preferencias y consentimientos por canal (Alto / Medio)

### 4) Ventas (Embudo)
- [x] MVP: Prospectos (CRUD + conversión) (Alto / Medio)
- [x] MVP: Oportunidades con etapas (Alto / Medio)
- [x] MVP: Pronóstico básico (Medio / Medio)
- [x] Empresarial: Cotizaciones y descuentos (Alto / Alto)
- [x] Empresarial: Aprobaciones de precio (Medio / Alto)
- [x] Mejor en su clase: CPQ completo (Medio / Alto)

### 5) Servicio (Casos)
- [x] MVP: Casos/tickets + estados (Alto / Medio)
- [x] MVP: SLA básico (Alto / Medio)
- [x] Empresarial: Colas, enrutamiento, escalamiento (Alto / Alto)
- [x] Empresarial: Base de conocimiento (Medio / Medio)
- [x] Mejor en su clase: Omnicanal (correo/chat/voz) (Alto / Alto)

### 6) Marketing
- [ ] MVP: Segmentación simple (Medio / Medio)
- [ ] MVP: Campañas y envíos básicos (Medio / Medio)
- [ ] Empresarial: Recorridos + disparadores (Alto / Alto)
- [ ] Empresarial: Puntuación y atribución (Medio / Alto)
- [ ] Mejor en su clase: CDP básico (Medio / Alto)

### 7) Integraciones
- [ ] MVP: Ganchos web + eventos de dominio (Medio / Medio)
- [ ] Empresarial: Integración correo/calendario (Medio / Alto)
- [ ] Empresarial: ERP/Facturación (Alto / Alto)

### 8) Observabilidad y resiliencia
- [ ] MVP: Comprobaciones de salud BD/servicios (Medio / Bajo)
- [ ] MVP: Registro estructurado + correlación (Medio / Medio)
- [ ] Empresarial: Trazas (OpenTelemetry) (Medio / Medio)
- [ ] Empresarial: Limitación de tasa, reintentos, cortacircuitos (Alto / Alto)

### 9) Cumplimiento y auditoría
- [ ] MVP: Auditoría de cambios por entidad (Alto / Alto)
- [ ] Empresarial: GDPR/LPDP (borrado/portabilidad) (Alto / Alto)
- [ ] Empresarial: Retención de datos (Medio / Medio)

## Hoja de ruta para GitHub (esfuerzo estimado)

Formato: M (1-2 sem), L (3-5 sem), XL (6-10 sem), S (<=1 sem).

### Hito 1 - Fundación (Seguridad + base multi-tenant)
- [x] Autenticación JWT/OIDC + UseAuthentication - M
- [x] RBAC básico por módulo - M
- [x] TenantId + filtros globales en EF - L
- [x] Auditoría de accesos - M
- [x] Health checks - S

### Hito 2 - Customer 360
- [ ] Contactos + roles - M
- [ ] Jerarquías de cuentas - M
- [ ] Preferencias/consentimientos - M
- [ ] Dedupe + merge (golden record) - XL

### Hito 3 - Ventas (MVP)
- [x] Prospectos + conversión - M
- [x] Oportunidades + etapas - M
- [x] Pronóstico básico - M
- [x] Cotizaciones y descuentos - L

### Hito 4 - Servicio (MVP)
- [x] Casos + estados - M
- [x] SLA básico - M
- [x] Colas y enrutamiento - L
- [x] Base de conocimiento - M

### Hito 5 - Marketing (MVP)
- [ ] Segmentación simple - M
- [ ] Campañas básicas - M
- [ ] Recorridos + disparadores - L
- [ ] Puntuación + atribución - L

### Hito 6 - Operaciones empresariales
- [ ] Observabilidad (registros estructurados + trazas) - M
- [ ] Limitación de tasa + reintentos + cortacircuitos - M
- [ ] GDPR/LPDP + retención - L
- [ ] Ganchos web + eventos - M
- [ ] Integración correo/calendario - L
- [ ] Integración ERP - XL
