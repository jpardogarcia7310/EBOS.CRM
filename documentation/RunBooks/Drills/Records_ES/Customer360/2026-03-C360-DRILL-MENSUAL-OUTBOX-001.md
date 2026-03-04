# Evidencia de Drill Customer 360

## Metadatos del Drill
- Drill ID: `C360-DRILL-MENSUAL-OUTBOX-001`
- Fecha/Hora (UTC): `2026-03-04T00:00:00Z`
- Entorno: `CI + entorno local de pruebas`
- Operador(es): `CRM Platform Team`
- Revisor: `jpardogarcia7310`
- Frecuencia: `mensual`
- Tipo de drill: `fallo/recuperación de outbox`

## Alcance y Objetivo
- Objetivo: Validar procesamiento de backlog de outbox y recuperación bajo estrés y concurrencia.
- Componentes en alcance: `AuditOutboxService`, `AuditOutboxDispatcher`, métricas y endpoints de readiness.
- Componentes fuera de alcance: SLA de sistema externo de auditoría.

## Precondiciones
- Runbook utilizado: `documentation/RunBooks/Customer360-Operability-RunBook_ES.md`
- Accesos requeridos verificados: Sí
- Feature flags/configuración: umbrales outbox y reintentos por defecto
- Datos/setup de tenant: seed de pruebas para aislamiento tenant y mensajes outbox

## Pasos de Ejecución
1. Ejecutar `AuditOutboxBacklogStressTests`.
2. Ejecutar `AuditOutboxDispatcherConcurrencyTests`.
3. Validar endpoints operativos y de métricas.

## Detección y Respuesta
- Fuente de detección: logs CI + aserciones de pruebas + `/api/v{version}/OperationalReadiness/*`.
- Tiempo de detección (minutos): `<= 2`
- Acciones de respuesta: endurecer comportamiento determinista y validar tendencias pending/failed.
- ¿Requirió escalado?: `No`

## Recuperación y Validación
- Tiempo de recuperación (minutos): `~20`
- Objetivo RTO cumplido: `PASS`
- Objetivo RPO cumplido: `PASS`
- Validación funcional realizada: sí (stress + concurrencia + integración de endpoints operativos).
- Resumen de impacto de negocio: sin impacto productivo.

## Evidencias
- URL de ejecución CI/pipeline: `customer360-suites-ci` (artefactos de jobs).
- Consultas/resultados Prometheus: `up{job="ebos-crm-api"}` y tendencias de métricas outbox.
- Capturas Grafana: dashboard disponible (`customer360-operability-dashboard.json`).
- Notificaciones de alerta: rutas validadas por checks de configuración.
- Logs/trazas: TRX de pruebas + logs de scripts de observabilidad.
- Tickets relacionados: `N/A`

## Lecciones Aprendidas y Acciones
- Qué funcionó: suites de outbox en estrés/concurrencia y endpoints de readiness.
- Qué falló: comportamiento transitorio inicial necesitó mayor determinismo.
- Acciones:
  - Responsable: `CRM Platform Team`
  - Fecha objetivo: `2026-03-31`
  - Estado: `Abierto`
