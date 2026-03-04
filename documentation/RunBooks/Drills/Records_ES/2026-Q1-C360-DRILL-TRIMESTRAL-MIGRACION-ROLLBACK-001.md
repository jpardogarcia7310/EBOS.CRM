# Evidencia de Drill Customer 360

## Metadatos del Drill
- Drill ID: `C360-DRILL-TRIMESTRAL-MIGRACION-ROLLBACK-001`
- Fecha/Hora (UTC): `2026-03-04T00:00:00Z`
- Entorno: `GitHub Actions + SQL Server Testcontainers`
- Operador(es): `customer360-suites-ci`
- Revisor: `jpardogarcia7310`
- Frecuencia: `trimestral`
- Tipo de drill: `migración+rollback`

## Alcance y Objetivo
- Objetivo: Validar apply/rollback de migraciones y consistencia post-fallo en SQL Server.
- Componentes en alcance: migraciones EF, suite SQL hardening y guard test de migraciones.
- Componentes fuera de alcance: ventanas de despliegue productivo.

## Precondiciones
- Runbook utilizado: `documentation/RunBooks/Customer360-Operability-RunBook_ES.md`
- Accesos requeridos verificados: Sí
- Feature flags/configuración: `USE_TESTCONTAINERS=true`
- Datos/setup de tenant: nombres de base aislados por test

## Pasos de Ejecución
1. Ejecutar `SqlServerMigrationHardeningTests`.
2. Ejecutar `Customer360SqlServerIdempotencyTests`.
3. Ejecutar `MigrationDuplicateCreateTableGuardTest`.

## Detección y Respuesta
- Fuente de detección: fallos CI en `integration-sqlserver-tests`.
- Tiempo de detección (minutos): `<= 2`
- Acciones de respuesta:
  - corregir ruta de migración duplicada `CreateTable`,
  - añadir guard test,
  - estabilizar test de retry.
- ¿Requirió escalado?: `No`

## Recuperación y Validación
- Tiempo de recuperación (minutos): `~60`
- Objetivo RTO cumplido: `PASS`
- Objetivo RPO cumplido: `PASS`
- Validación funcional realizada: sí (migraciones + rollback + idempotencia + consistencia).
- Resumen de impacto de negocio: recuperada fiabilidad del gate de PR; sin impacto productivo.

## Evidencias
- URL de ejecución CI/pipeline: `customer360-suites-ci / integration-sqlserver-tests`.
- Consultas/resultados Prometheus: no es evidencia principal en este drill.
- Capturas Grafana: N/A.
- Notificaciones de alerta: N/A.
- Logs/trazas: `integration-sqlserver-tests.trx`.
- Tickets relacionados: `N/A`

## Lecciones Aprendidas y Acciones
- Qué funcionó: suite dedicada SQL Server con testcontainers.
- Qué falló: simulación transitoria flaky y artefacto de migración duplicada.
- Acciones:
  - Responsable: `CRM Platform Team`
  - Fecha objetivo: `2026-03-31`
  - Estado: `Abierto`
