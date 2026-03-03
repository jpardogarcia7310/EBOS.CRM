# Activos de Observabilidad Customer 360

Esta carpeta contiene activos base de observabilidad para PR-6:

- Dashboard de Grafana (JSON):
  - `grafana/customer360-operability-dashboard.json`
- Reglas de alerta de Prometheus:
  - `prometheus/customer360-alert-rules.yml`
- Configuración base lista para usar:
  - `prometheus/prometheus.yml`
  - `prometheus/alertmanager.yml`
  - `.env.alerting`
  - `docker-compose.observability.yml`
  - `grafana/provisioning/datasources/datasource.yml`
  - `grafana/provisioning/dashboards/dashboards.yml`

## Estado cerrado al 100% (matcher exacto)

Todo está fijado con matcher exacto:

- `job="ebos-crm-api"`

No se usa regex ni variable para el `job` en dashboard ni alertas.

## Métricas esperadas

- `customer360_merge_total`
- `customer360_dedupe_query_total`
- `customer360_consent_event_total`
- `customer360_audit_outbox_total`
- `customer360_concurrency_total`

## ¿Prometheus va dentro de la API?

No. Prometheus se despliega como servicio independiente y hace `scrape` del endpoint de la API:

- `http://<host-api>:<puerto>/metrics`

En este repo la API ya expone `/metrics`.
Para trazas distribuidas, habilita OpenTelemetry en `EBOS.CRM.Api/appsettings*.json`:
`OpenTelemetry:Enabled=true` y define `OpenTelemetry:OtlpEndpoint` (por ejemplo `http://localhost:4317`).

## Puesta en marcha rápida local (Docker)

1. Arranca la API en local en el perfil `http` (`http://localhost:5013`).
2. `prometheus/prometheus.yml` ya viene preconfigurado con `host.docker.internal:5013`.
3. Desde `documentation/Observability`, ejecuta:

```bash
docker compose -f docker-compose.observability.yml up -d
```

Antes del primer arranque, edita `.env.alerting` con tus credenciales reales
(SMTP/Slack/Teams/PagerDuty) para habilitar routing real de alertas.

4. Accede a:
- Prometheus: `http://localhost:9090`
- Alertmanager: `http://localhost:9093`
- Grafana: `http://localhost:3000` (admin/admin)

5. Importa el dashboard:
- Ya no hace falta importarlo manualmente. Grafana lo provisiona automáticamente al iniciar.

## Verificación mínima

1. En Prometheus, consulta:

```promql
up{job="ebos-crm-api"}
```

Debe devolver `1`.

2. Comprueba una métrica Customer 360:

```promql
sum(rate(customer360_merge_total{job="ebos-crm-api"}[5m]))
```

3. Comprueba reglas cargadas:
- En Prometheus > `Status` > `Rules`, grupo `customer360-operability`.

## Producción

- Mantén exactamente `job_name: ebos-crm-api` en el `scrape_config`.
- Si usas Kubernetes/ServiceMonitor, aplica relabel para que el label final `job` sea `ebos-crm-api`.
- Si cambias el job, tendrás que actualizar dashboard y reglas.
- `docker-compose.observability.yml` ya incluye volúmenes persistentes:
  - `prometheus_data`
  - `alertmanager_data`
  - `grafana_data`

## Siguientes pasos recomendados

1. Rellenar valores reales en `.env.alerting`.
2. Levantar stack:

```bash
docker compose -f documentation/Observability/docker-compose.observability.yml up -d
```

3. Verificar en Prometheus:

```promql
up{job="ebos-crm-api"}
```

## Routing de alertas (severidad)

`prometheus/alertmanager.yml` enruta por `severity`:

- `critical`:
  - PagerDuty
  - Slack (canal crítico)
  - Teams (webhook crítico)
  - Email crítico
- `warning`:
  - Slack (canal warning)
  - Teams (webhook warning)
  - Email warning

## Seguimiento Operativo

- Runbook:
  - `documentation/RunBooks/Customer360-Operability-RunBook_ES.md`
- Checklist post-deploy obligatorio:
  - `documentation/RunBooks/Customer360-PostDeploy-Checklist_ES.md`
- Cadencia recomendada de drills:
  - Drill mensual de fallo/recuperación de outbox
  - Drill trimestral de migración+rollback
  - Drill trimestral de routing de alertas
- Workflow de CI:
  - El workflow de GitHub Actions `Observability CI` se ejecuta en PR/push y también manualmente (`workflow_dispatch`).
