# Checklist Post-Deploy Customer 360

Usa este checklist tras cada despliegue en staging/producción.
Marca cada línea como `PASS`, `FAIL` o `N/A` y adjunta evidencias.

## 1) Plataforma y API
- Proceso de API en ejecución estable durante 10+ minutos.
- `GET /health/live` devuelve `200`.
- `GET /health/ready` devuelve `200` (o `503` esperado con causa documentada).
- Sin errores de migración al arranque en logs de API.

## 2) Seguridad y Acceso
- `/metrics` no está expuesto públicamente sin auth/policy requerida en el entorno objetivo.
- Endpoints operativos requieren policy y devuelven `401/403/200` esperados.
- Resolución de tenant por header/subdominio sigue funcionando en endpoints Customer 360.

## 3) Smoke Funcional Customer 360
- Endpoint de dedupe responde correctamente.
- Endpoint de comando merge responde correctamente (o validación de negocio controlada).
- Endpoints de consent add/revoke responden correctamente.
- Endpoints de register/execute privacy request responden correctamente.

## 4) Outbox y Concurrencia
- `OperationalReadiness/dashboard` muestra valores esperados de outbox pending/failed.
- `OperationalReadiness/alerts` no muestra flags críticos inesperados.
- Fallos de concurrencia dentro de la línea base normal.

## 5) Observabilidad
- Target de Prometheus `up{job="ebos-crm-api"}` en `1`.
- Grupo de reglas `customer360-operability` cargado en Prometheus.
- Dashboard de Grafana `Customer360 Operability` carga sin errores de datasource.
- Al menos un punto visible para:
  - `customer360_merge_total`
  - `customer360_audit_outbox_total`
  - `customer360_concurrency_total`

## 6) Routing de Alertas
- Alerta de prueba warning llega al canal esperado.
- Alerta de prueba critical llega al canal esperado.
- Notificaciones de resolución de alertas se entregan correctamente.

## 7) Cierre
- Referencias de incidente/runbook actualizadas si hubo desviaciones.
- Ticket de despliegue incluye enlaces a todas las evidencias.
- Estado final aprobado por guardia/operador.

