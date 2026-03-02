# Activos de Observabilidad Customer 360

Esta carpeta contiene activos base de observabilidad para la PR-6:

- Dashboard de Grafana (JSON):
  - `grafana/customer360-operability-dashboard.json`
- Reglas de alerta de Prometheus:
  - `prometheus/customer360-alert-rules.yml`

## Nombres de métricas

El dashboard y las reglas de alerta asumen nomenclatura OpenTelemetry hacia Prometheus con guiones bajos:

- `customer360_merge_total`
- `customer360_dedupe_query_total`
- `customer360_consent_event_total`
- `customer360_audit_outbox_total`
- `customer360_concurrency_total`

Si tu pipeline de Prometheus expone nombres distintos, ajusta los `expr` de las reglas y las consultas de paneles.

## Labels usadas en consultas

- `job` (job de scrape de Prometheus para esta API)
- `instance`
- Atributos opcionales emitidos por la app:
  - `tenant_id`
  - `operation`
  - `event`
  - `success`
  - `exhausted_retries`

## Adaptación a producción aplicada

- Las consultas del dashboard están filtradas por variable de Grafana `job` (`job=~"$job"`).
- Las reglas de alerta incluyen un matcher orientado a producción:
  - `job=~"(?i).*(ebos.*crm.*api|crm.*api|ebos.*crm).*"`

Si tu `job_name` de scrape es diferente, ajusta este matcher en:
- `prometheus/customer360-alert-rules.yml`

## Importar/Aplicar

1. Importa el JSON del dashboard en Grafana.
2. Crea/actualiza un archivo de reglas de Prometheus con `prometheus/customer360-alert-rules.yml`.
3. Recarga la configuración de reglas en Prometheus.
4. Verifica en Grafana:
   - los paneles muestran datos
   - el estado de alertas aparece en Alertmanager/Grafana Alerting.
