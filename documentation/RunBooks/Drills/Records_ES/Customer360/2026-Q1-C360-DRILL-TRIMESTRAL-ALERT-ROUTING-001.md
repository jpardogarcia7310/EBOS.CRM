# Evidencia de Drill Customer 360

## Metadatos del Drill
- Drill ID: `C360-DRILL-TRIMESTRAL-ALERT-ROUTING-001`
- Fecha/Hora (UTC): `2026-03-04T00:00:00Z`
- Entorno: `validación CI de observabilidad + stack docker compose`
- Operador(es): `CRM Platform Team`
- Revisor: `jpardogarcia7310`
- Frecuencia: `trimestral`
- Tipo de drill: `routing de alertas warning+critical`

## Alcance y Objetivo
- Objetivo: Verificar reglas Prometheus, routing Alertmanager y smoke de observabilidad.
- Componentes en alcance: `prometheus.yml`, `customer360-alert-rules.yml`, `alertmanager.yml`, provisioning de Grafana.
- Componentes fuera de alcance: confirmación de entrega en proveedores externos (Slack/Teams/Email/PagerDuty) dentro de CI.

## Precondiciones
- Runbook utilizado: `documentation/RunBooks/Customer360-Operability-RunBook_ES.md`
- Accesos requeridos verificados: Sí
- Feature flags/configuración: `.env.alerting` con placeholders seguros para CI
- Datos/setup de tenant: matcher exacto `job="ebos-crm-api"`

## Pasos de Ejecución
1. Ejecutar `documentation/Observability/ci/validate-observability.sh`.
2. Ejecutar `documentation/Observability/ci/smoke-observability.sh`.
3. Verificar readiness de Prometheus y query `up{job="ebos-crm-api"}`.

## Detección y Respuesta
- Fuente de detección: salida de scripts y códigos de salida.
- Tiempo de detección (minutos): `<= 2`
- Acciones de respuesta: correcciones de paths/mounts/render y estabilización del smoke.
- ¿Requirió escalado?: `No`

## Recuperación y Validación
- Tiempo de recuperación (minutos): `~45`
- Objetivo RTO cumplido: `PASS`
- Objetivo RPO cumplido: `PASS`
- Validación funcional realizada: sí (config checks + smoke stack + carga de reglas).
- Resumen de impacto de negocio: estabilizado el gate CI de observabilidad.

## Evidencias
- URL de ejecución CI/pipeline: jobs/steps de validación de observabilidad.
- Consultas/resultados Prometheus: `up{job="ebos-crm-api"}`.
- Capturas Grafana: JSON/provisioning validados.
- Notificaciones de alerta: routing validado de forma sintáctica/estructural.
- Logs/trazas: salidas de `validate-observability.sh` y `smoke-observability.sh`.
- Tickets relacionados: `N/A`

## Lecciones Aprendidas y Acciones
- Qué funcionó: validadores CI estrictos y matcher exacto de job.
- Qué falló: drift inicial de paths/mounts/config y render de entorno.
- Acciones:
  - Responsable: `CRM Platform Team`
  - Fecha objetivo: `2026-03-31`
  - Estado: `Abierto`
