# Runbook de Operabilidad de Observabilidad y Resiliencia

## Alcance
- Operacion de observabilidad y resiliencia para manejo de solicitudes, dependencias, acceso a datos y procesamiento en segundo plano.
- Aplica a `EBOS.CRM.Api`, `EBOS.CRM.Application` y `EBOS.CRM.Infrastructure`.

## Objetivos de Servicio (SLO)
- RTO (recuperacion del servicio):
  - P1 (API no disponible): <= 30 minutos
  - P2 (comportamiento resiliente degradado): <= 2 horas
- RPO (perdida de datos/telemetria aceptable):
  - datos transaccionales: <= 5 minutos
  - continuidad de telemetria (logs/metricas/trazas): <= 15 minutos
- Escalado:
  - P1 -> guardia inmediata, canal de incidente activo, actualizaciones cada 15 minutos.
  - P2 -> respuesta de guardia en horario habil, actualizaciones cada 60 minutos.

## Endpoints Operativos
- Health checks:
  - Liveness: `GET /health/live`
  - Readiness: `GET /health/ready`
- Endpoint de metadata de observabilidad (si esta habilitado):
  - `GET /api/v2.0/OperationalReadiness/observability`

## Estandar de Correlacion y Trazas
- Header de correlacion entrante: `X-Correlation-Id`.
- Headers de contexto de traza: `traceparent`, `tracestate`.
- Toda respuesta de error debe incluir:
  - `correlationId`
  - `traceId`
- Toda nota de incidente debe registrar:
  - primer `correlationId` con fallo
  - `traceId` representativo
  - endpoint afectado y marca temporal UTC.

## Metricas (Observabilidad/Resiliencia)
- Metricas de solicitudes API:
  - `http.server.request.count`
  - `http.server.request.duration`
  - `http.server.request.failures`
- Metricas de dependencias:
  - `dependency.call.count`
  - `dependency.call.duration`
  - `dependency.call.failures`
- Metricas de politicas de resiliencia:
  - `resilience.retry.total`
  - `resilience.circuitbreaker.open.total`
  - `resilience.timeout.total`
  - `resilience.ratelimit.reject.total`

## Paneles Recomendados de Dashboard
- Trafico y latencia:
  - solicitudes por minuto
  - latencia p50/p95/p99
- Confiabilidad:
  - tasa de error por endpoint y clase de codigo de estado
  - tasa de retries y timeouts
- Salud de dependencias:
  - dependencias con mayor fallo por clase de error
  - duracion de estado abierto de circuit breaker por dependencia
- Saturacion:
  - longitud de cola del thread pool
  - uso del pool de conexiones DB
  - solicitudes limitadas por minuto

## Reglas de Alerta (baseline)
- Disponibilidad API critica:
  - ratio de fallos >= 10% durante 5 minutos.
- Latencia critica:
  - latencia p95 por encima del umbral objetivo durante 10 minutos.
- Circuit breaker critico:
  - estado abierto sostenido > 5 minutos en dependencia critica.
- Timeout critico:
  - total de timeouts sobre umbral durante 5 minutos.
- Readiness degradado/no saludable:
  - readiness en `Degraded` o `Unhealthy` durante 3 sondas consecutivas.

## Configuracion
- Seccion: `Observability`
  - `EnableCorrelationIdHeader`
  - `EnableTraceContextPropagation`
  - `SlowRequestThresholdMs`
- Seccion: `Resilience`
  - `RequestTimeoutMs`
  - `RetryMaxAttempts`
  - `RetryBaseDelayMs`
  - `CircuitBreakerFailureThreshold`
  - `CircuitBreakerSamplingWindowSeconds`
  - `CircuitBreakerBreakDurationSeconds`
  - `RateLimitPerMinute`

## Procedimiento de Triage de Incidentes (CorrelationId y TraceId)
1. Confirmar tipo de alerta, alcance, hora de inicio (UTC) y endpoints impactados.
2. Capturar una muestra de solicitud fallida desde logs o gateway y extraer:
   - `correlationId`
   - `traceId`
   - endpoint, estado HTTP y latencia.
3. Consultar logs por `correlationId` para reconstruir ciclo de la solicitud:
   - log de entrada
   - logs del handler
   - llamadas a dependencias
   - log de excepcion/fallo.
4. Consultar trazas distribuidas por `traceId` para ubicar el span fallido y el cuello de botella:
   - span de API
   - span de handler en Application
   - span de Infrastructure/dependencia.
5. Clasificar causa del incidente:
   - caida/latencia de dependencia
   - saturacion/deadlock/timeout de base de datos
   - mala configuracion de politicas resilientes
   - regresion de codigo.
6. Ejecutar mitigacion segun causa:
   - dependencia: activar fallback/degradacion y reducir concurrencia de solicitudes.
   - DB: reducir presion de escritura, verificar saturacion del pool, ajustar timeout/retry como control de emergencia.
   - configuracion: rollback o hotfix de valores de politica.
   - regresion de codigo: rollback a la version estable previa.
7. Validar recuperacion:
   - readiness vuelve a estado saludable/degradado-aceptable
   - tasa de error y p95 dentro del objetivo
   - alertas de circuit breaker y timeout despejadas.
8. Cerrar incidente con evidencia:
   - muestras de correlationId/traceId
   - linea de tiempo (deteccion, mitigacion, recuperacion)
   - causa raiz y acciones preventivas.

## Procedimiento de Migracion
1. Respaldar DB y exportar configuracion vigente de resiliencia/observabilidad.
2. Desplegar cambios de API e infraestructura.
3. Validar arranque:
   - `/health/live` saludable
   - `/health/ready` saludable o degradado esperado.
4. Enviar solicitudes controladas y verificar `correlationId` y `traceId` en respuestas y logs.
5. Monitorear alertas y dashboards clave durante al menos 30 minutos.

## Procedimiento de Rollback
1. Detener el segmento de despliegue de riesgo (canary o despliegue completo).
2. Volver a la ultima version estable de aplicacion.
3. Restaurar valores previos de configuracion resiliente/observabilidad si aplica.
4. Validar estabilizacion de salud, latencia y presupuesto de error.
5. Documentar motivo del rollback con evidencia de correlacion y trazas.

## Drills Operativos
- Frecuencia:
  - Mensual: drill de timeout de dependencia y circuit breaker.
  - Mensual: drill de triage por alta tasa de error usando correlationId/traceId.
  - Trimestral: drill completo de rollback incluyendo rollback de configuracion.
- Evidencia minima:
  - fecha/hora de ejecucion, operador, escenario, tiempo de deteccion, tiempo de mitigacion, tiempo de recuperacion, lecciones aprendidas.
  - incluir al menos 3 pares de `correlationId` y `traceId`.
- Criterios de salida:
  - ruta de incidente reproducible por otro operador.
  - evidencia requerida completa.
  - recuperacion medida dentro de objetivos SLO.

## Plantilla de Registro de Drill
- Usar: `documentation/RunBooks/Observability-Drill-Record-Template_ES.md`
- Guardar registros completados en: `documentation/RunBooks/Drills/Records_ES/`

## Checklist Post-Deploy
- Usar: `documentation/RunBooks/Observability-PostDeploy-Checklist_ES.md`
- Marcar cada item como `PASS/FAIL/N/A` y adjuntar enlaces de evidencia.

## Checklist de Verificacion
- `dotnet build EBOS.CRM.slnx -c Debug`
- `dotnet test tests/EBOS.CRM.ApiTests/EBOS.CRM.ApiTests.csproj -c Debug --filter "FullyQualifiedName~Observability|FullyQualifiedName~Resilience"`
- `dotnet test tests/EBOS.CRM.IntegrationTests/EBOS.CRM.IntegrationTests.csproj -c Debug --filter "FullyQualifiedName~Observability|FullyQualifiedName~Resilience"`
- confirmar visibilidad de logs, metricas y trazas en el entorno objetivo.
