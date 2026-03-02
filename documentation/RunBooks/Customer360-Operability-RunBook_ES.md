# Runbook de Operabilidad Customer 360

## Alcance
- Operabilidad de dedupe, merge, consent y audit outbox de Customer 360.
- Aplica a `EBOS.CRM.Api` y a los servicios de infraestructura dependientes.

## Endpoints Operativos
- Dashboard de readiness:
  - `GET /api/v2.0/OperationalReadiness/dashboard`
- Resumen de estado de alertas:
  - `GET /api/v2.0/OperationalReadiness/alerts`
- Health checks:
  - Liveness: `GET /health/live`
  - Readiness: `GET /health/ready`

## Métricas (Customer 360)
- Meter origen: `EBOS.CRM.Customer360`
- Contadores:
  - `customer360.merge.total`
  - `customer360.dedupe.query.total`
  - `customer360.consent.event.total`
  - `customer360.audit.outbox.total`
  - `customer360.concurrency.total`

## Paneles Recomendados de Dashboard
- Dedupe:
  - total de consultas dedupe por minuto
  - percentiles de candidatos por consulta
- Merge:
  - ratio de éxito/fallo de merges
  - clientes fusionados por operación
- Consent:
  - tendencia de consent granted/revoked por tipo
- Outbox:
  - tamaño de cola pendiente
  - mensajes outbox fallidos
  - marca temporal del último dispatch exitoso
- Concurrencia:
  - conflictos por minuto
  - cantidad de retries agotados

## Reglas de Alerta (baseline)
- Outbox crítico:
  - `outbox.failed >= OutboxFailedCriticalThreshold`
- Backlog outbox crítico:
  - `outbox.pending >= OutboxPendingCriticalThreshold`
- Dispatch outbox estancado:
  - último dispatch más antiguo que `OutboxDispatchStaleMinutesThreshold` y `pending > 0`
- Concurrencia crítica:
  - `concurrency.failures.total >= ConcurrencyFailuresCriticalThreshold`

## Configuración
- Sección: `OperationalReadiness`
  - `OutboxPendingWarningThreshold`
  - `OutboxPendingCriticalThreshold`
  - `OutboxFailedCriticalThreshold`
  - `ConcurrencyFailuresCriticalThreshold`
  - `OutboxDispatchStaleMinutesThreshold`

## Procedimiento de Migración
1. Realizar backup de DB y registrar versión actual de esquema.
2. Desplegar la API con artefactos de migración.
3. Ejecutar migraciones automáticamente al arranque o con `dotnet ef database update`.
4. Validar:
   - `/health/ready` devuelve `Healthy` o `Degraded` esperado.
   - endpoints críticos de Customer 360 responden correctamente.
5. Monitorizar alertas de outbox y concurrencia durante al menos 30 minutos.

## Procedimiento de Rollback
1. Detener escrituras entrantes si es posible.
2. Revertir versión de aplicación.
3. Si requiere rollback de esquema, ejecutar migración objetivo de rollback en ventana controlada.
4. Validar health endpoints y flujos principales de Customer 360.
5. Reprocesar backlog de outbox si quedan mensajes pendientes.

## Troubleshooting
- Readiness degraded/unhealthy:
  - revisar `/api/v2.0/OperationalReadiness/dashboard`
  - inspeccionar `outbox.pending`, `outbox.failed` y stale dispatch
- Incremento de fallos de outbox:
  - validar `AuditService:BaseUrl`
  - verificar conectividad de red y autenticación contra servicio de auditoría
  - revisar `AuditOutboxMessage.LastError`
- Alto volumen de fallos por concurrencia:
  - identificar aggregates/endpoints con más contención
  - ajustar retries en `CommandExecution`
  - revisar flujos de negocio con escrituras conflictivas

## Checklist de Verificación
- `dotnet build EBOS.CRM.slnx -c Debug`
- `dotnet test tests/EBOS.CRM.IntegrationTests/EBOS.CRM.IntegrationTests.csproj -c Debug --filter "FullyQualifiedName~Customer360"`
- endpoints de readiness y dashboard accesibles en entorno desplegado.
