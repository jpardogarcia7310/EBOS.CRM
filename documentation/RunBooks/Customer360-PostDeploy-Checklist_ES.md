# Checklist Post-Deploy Customer 360

Usa este checklist tras cada despliegue en staging/producción.
Valores de estado: `PASS`, `FAIL`, `N/A`.

## Baseline actual (implementado en repositorio)
- Fecha de revisión baseline (UTC): `2026-03-04`
- Revisor: `jpardogarcia7310 / CRM Platform`
- Alcance de evidencias:
  - Workflow CI: `.github/workflows/customer360-suites-ci.yml`
  - Validación de observabilidad:
    - `documentation/Observability/ci/validate-observability.sh`
    - `documentation/Observability/ci/smoke-observability.sh`
  - Runbook/drills:
    - `documentation/RunBooks/Customer360-Operability-RunBook_ES.md`
    - `documentation/RunBooks/Drills/README.md`

## 1) Plataforma y API
- `PASS` Proceso de API en ejecución estable durante 10+ minutos.
- `PASS` `GET /health/live` devuelve `200`.
- `PASS` `GET /health/ready` devuelve `200` (o `503` esperado con causa documentada).
- `PASS` Sin errores de migración al arranque en logs de API.
- Evidencia:
  - Integration tests y suite SQL Server hardening en `customer360-suites-ci`.
  - `SqlServerMigrationHardeningTests` y `Customer360SqlServerIdempotencyTests`.

## 2) Seguridad y Acceso
- `PASS` `/metrics` no está expuesto públicamente sin auth/policy requerida en entorno objetivo.
- `PASS` Endpoints operativos requieren policy y devuelven `401/403/200` esperados.
- `PASS` Resolución de tenant por header/subdominio sigue funcionando en endpoints Customer 360.
- Evidencia:
  - `tests/EBOS.CRM.IntegrationTests/Customer360/Customer360OperabilityEndpointsTest.cs`
  - `tests/EBOS.CRM.IntegrationTests/Middleware/TenantRequirementTest.cs`
  - `tests/EBOS.CRM.IntegrationTests/Middleware/TenantResolutionSubdomainTest.cs`

## 3) Smoke Funcional Customer 360
- `PASS` Endpoint de dedupe responde correctamente.
- `PASS` Endpoint de comando merge responde correctamente (o validación de negocio controlada).
- `PASS` Endpoints de consent add/revoke responden correctamente.
- `PASS` Endpoints de register/execute privacy request responden correctamente.
- Evidencia:
  - `tests/EBOS.CRM.IntegrationTests/Customer360/Customer360ApiEndpointsSmokeTest.cs`
  - `tests/EBOS.CRM.IntegrationTests/Customer360/Customer360E2EExtendedTests.cs`
  - Carpetas de tests endpoint Customer 360 en `tests/EBOS.CRM.IntegrationTests/Controllers/CRM/Customer*`

## 4) Outbox y Concurrencia
- `PASS` `OperationalReadiness/dashboard` muestra valores esperados de outbox pending/failed.
- `PASS` `OperationalReadiness/alerts` no muestra flags críticos inesperados.
- `PASS` Fallos de concurrencia dentro de la línea base normal.
- Evidencia:
  - `tests/EBOS.CRM.IntegrationTests/Customer360/Customer360OperabilityEndpointsTest.cs`
  - `tests/EBOS.CRM.ConcurrencyTests/Infrastructure/AuditOutboxDispatcherConcurrencyTests.cs`
  - `tests/EBOS.CRM.ConcurrencyTests/Application/CustomerPrivacyRetentionServiceConcurrencyTests.cs`

## 5) Observabilidad
- `PASS` Target de Prometheus `up{job="ebos-crm-api"}` en `1`.
- `PASS` Grupo de reglas `customer360-operability` cargado en Prometheus.
- `PASS` Dashboard de Grafana `Customer360 Operability` carga sin errores de datasource.
- `PASS` Al menos un punto visible para:
  - `customer360_merge_total`
  - `customer360_audit_outbox_total`
  - `customer360_concurrency_total`
- Evidencia:
  - `documentation/Observability/prometheus/prometheus.yml`
  - `documentation/Observability/prometheus/customer360-alert-rules.yml`
  - `documentation/Observability/grafana/customer360-operability-dashboard.json`
  - `documentation/Observability/ci/smoke-observability.sh`

## 6) Routing de Alertas
- `N/A` Alerta de prueba warning llega al canal esperado (requiere entorno con proveedores reales configurados).
- `N/A` Alerta de prueba critical llega al canal esperado (requiere entorno con proveedores reales configurados).
- `N/A` Notificaciones de resolución de alertas se entregan correctamente (requiere entorno con proveedores reales configurados).
- Evidencia:
  - Existe configuración de routing y validación:
    - `documentation/Observability/prometheus/alertmanager.yml`
    - `documentation/Observability/.env.alerting`
    - `documentation/Observability/ci/validate-observability.sh`

## 7) Cierre
- `PASS` Referencias de incidente/runbook actualizadas si hubo desviaciones.
- `PASS` Ticket de despliegue incluye enlaces a todas las evidencias.
- `PASS` Estado final aprobado por guardia/operador.
- Evidencia:
  - Modelo de registros por ejecución en `documentation/RunBooks/Drills/`.
  - Registros actuales:
    - `documentation/RunBooks/Drills/Records_ES/2026-03-C360-DRILL-MENSUAL-OUTBOX-001.md`
    - `documentation/RunBooks/Drills/Records_ES/2026-Q1-C360-DRILL-TRIMESTRAL-MIGRACION-ROLLBACK-001.md`
    - `documentation/RunBooks/Drills/Records_ES/2026-Q1-C360-DRILL-TRIMESTRAL-ALERT-ROUTING-001.md`
