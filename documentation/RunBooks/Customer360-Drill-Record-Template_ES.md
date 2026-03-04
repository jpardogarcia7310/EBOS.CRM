# Evidencia de Drill Customer 360 (Legacy Unificado)

> Este archivo se mantiene por compatibilidad.
> Usa el modelo por ejecución en `documentation/RunBooks/Drills/`:
> - plantilla: `Customer360-Drill-Execution-Template_ES.md`
> - registros: `Records_ES/`
> - índice: `README.md`

Este registro se completó con evidencia real del repositorio (código, pruebas, workflows CI y activos de observabilidad).

## Metadatos del Drill
- Drill ID: `C360-DRILL-2026Q1-SQL-OBS-001`
- Fecha/Hora (UTC): `2026-03-04T00:00:00Z`
- Entorno: `CI (GitHub Actions) + SQL Server con Testcontainers`
- Operador(es): `workflow customer360-suites-ci`
- Revisor: `jpardogarcia7310`
- Tipo de drill:
  - migración+rollback (trimestral)
  - fallo/recuperación de outbox (mensual)
  - routing de alertas warning+critical (trimestral)

## Escenario
- Objetivo: Validar los gates enterprise de operabilidad de Customer 360 para migraciones, rollback, resiliencia de outbox y wiring de observabilidad.
- Precondiciones:
  - Workflow: `.github/workflows/customer360-suites-ci.yml`
  - Suite SQL hardening con `USE_TESTCONTAINERS=true`
  - Stack y validadores de observabilidad disponibles en `documentation/Observability`
  - Runbook y checklist disponibles:
    - `documentation/RunBooks/Customer360-Operability-RunBook_ES.md`
    - `documentation/RunBooks/Customer360-PostDeploy-Checklist_ES.md`
- Pasos de disparo:
  - Ejecutar suite SQL filtrada:
    - `SqlServerMigrationHardeningTests`
    - `Customer360SqlServerIdempotencyTests`
    - `MigrationDuplicateCreateTableGuardTest`
  - Validar configuración de observabilidad:
    - `documentation/Observability/ci/validate-observability.sh`
  - Validar smoke de observabilidad:
    - `documentation/Observability/ci/smoke-observability.sh`

## Detección y Respuesta
- Fuente de detección (alerta/dashboard/log): salida de pruebas CI (`trx`), logs de job, checks de readiness/query de Prometheus en script smoke.
- Tiempo de detección (minutos): `<= 2` (fallo visible de forma inmediata en el step del job).
- Acciones ejecutadas:
  - Se corrigió conflicto de migración duplicada (`Leads`) dejando `20260209213553_AddSalesEntities` como migración no-op de compatibilidad.
  - Se añadió guard test de migraciones para evitar `CreateTable` duplicado por `schema.table`.
  - Se corrigieron asserts de hardening SQL:
    - validación de tablas por esquema correcto (`CRM`, `EBOS`);
    - simulación determinista de retry en execution strategy.
- Referencia de runbook utilizada: `documentation/RunBooks/Customer360-Operability-RunBook_ES.md` (Procedimiento de Migración, Rollback, Playbooks de Incidente y Drills Operativos).

## Resultado de Recuperación
- Tiempo de recuperación (minutos): `~60` (ciclo de estabilización de código/pruebas en CI).
- Objetivo RTO cumplido (PASS/FAIL): `PASS` (objetivo P2 de runbook: `<= 4 horas`).
- Objetivo RPO cumplido (PASS/FAIL): `PASS` (sin datos productivos; solo base de pruebas CI).
- Resumen de impacto de negocio: Sin caída productiva. Impacto acotado a inestabilidad del gate de PR en suite Integration SQL Server.

## Evidencias
- Enlaces/resultados de consultas Prometheus:
  - Query exigida por script smoke: `up{job="ebos-crm-api"}`
  - Validación de reglas: `prometheus/customer360-alert-rules.yml`
- Capturas de Grafana:
  - No se adjuntan capturas automáticamente en artefactos del repo (`N/A` en esta ejecución).
  - Dashboard fuente: `documentation/Observability/grafana/customer360-operability-dashboard.json`
- Notificaciones de alerta (Slack/Teams/Email/PagerDuty):
  - Routing configurado en: `documentation/Observability/prometheus/alertmanager.yml`
  - Placeholders/secretos en: `documentation/Observability/.env.alerting`
  - En CI se valida config renderizada con valores seguros por defecto (sin confirmación externa de entrega).
- Logs/trazas relevantes:
  - Fallos SQL y retries en `integration-sqlserver-tests.trx`.
  - Logs de validación:
    - `documentation/Observability/ci/validate-observability.sh`
    - `documentation/Observability/ci/smoke-observability.sh`
- Referencias de ticket de despliegue/incidente:
  - Gate PR/CI: workflow `customer360-suites-ci`.
  - Ticket incidente: `N/A (actividad de hardening en repositorio/CI)`.

## Lecciones Aprendidas
- Qué funcionó:
  - Suite dedicada de hardening SQL con SQL Server real vía testcontainers.
  - Filtros explícitos por suite en CI y job de resumen consolidado.
  - Guard test que previene regresiones de migraciones en control de código.
- Qué falló:
  - Simulación no determinista de error transitorio en retry test provocó flakiness inicial.
  - Verificación de existencia de tablas asumía esquema incorrecto (`dbo`).
- Acciones:
  - Responsable: `CRM Platform Team`
  - Fecha objetivo: `2026-03-31`
  - Tareas:
    - Adjuntar capturas de Grafana como artefactos del workflow.
    - Publicar evidencia explícita de entrega de alertas (Slack/Teams/Email/PagerDuty) desde drills en staging.
    - Mantener tests de retry SQL deterministas y evitar escenarios sensibles a timing.
