# Checklist Post-Deploy de Observabilidad y Resiliencia

Usa este checklist tras cada despliegue en staging/produccion.
Valores de estado: `PASS`, `FAIL`, `N/A`.

## Baseline actual
- Fecha de revision baseline (UTC): `2026-03-04`
- Alcance:
  - `documentation/RunBooks/Observability-Operability-RunBook_ES.md`
  - `documentation/RunBooks/Observability-Drill-Record-Template_ES.md`

## 1) Plataforma y API
- `PASS` API estable durante 10+ minutos.
- `PASS` `GET /health/live` devuelve `200`.
- `PASS` `GET /health/ready` devuelve `200` (o estado degradado esperado con causa documentada).
- `PASS` Arranque sin errores de migracion/configuracion.

## 2) Correlacion y Trazas
- `PASS` Respuestas incluyen `correlationId` en rutas de exito y fallo.
- `PASS` Respuestas de error incluyen `traceId`.
- `PASS` `X-Correlation-Id` se acepta y propaga cuando se envia.
- `PASS` `traceparent`/`tracestate` se aceptan y propagan cuando se envian.

## 3) Politicas de Resiliencia
- `PASS` Politica de timeout activa con payload de timeout deterministico.
- `PASS` Politica de retry activa solo para fallos transitorios.
- `PASS` Circuit breaker abre/cierra segun umbrales configurados.
- `PASS` Rate limiting/degradacion consistente con la politica.

## 4) Senales de Observabilidad
- `PASS` Logs consultables por `correlationId` y `traceId`.
- `PASS` Pipeline de metricas saludable y recibiendo datos.
- `PASS` Trazas visibles end-to-end (API -> Application -> Infrastructure).
- `PASS` Dashboards cargan sin errores de datasource/query.

## 5) Alertamiento y Readiness
- `PASS` Alertas de disponibilidad/latencia cargadas y evaluandose.
- `PASS` Alertas de resiliencia (timeouts/retries/circuit breaker) cargadas y evaluandose.
- `PASS` Estado de readiness saludable o degradado esperado con nota de incidente.

## 6) Smoke Funcional
- `PASS` Un endpoint de lectura y uno de escritura responden con latencia esperada.
- `PASS` Simulacion de timeout de dependencia dispara comportamiento resiliente esperado.
- `PASS` Sin escrituras duplicadas bajo escenario de retry para ruta idempotente.

## 7) Cierre
- `PASS` Referencias de incidente/runbook actualizadas si hubo desviaciones.
- `PASS` Registro de despliegue con enlaces de evidencia (logs, capturas de trazas, consultas de metricas).
- `PASS` Estado final aprobado por guardia/operador.
