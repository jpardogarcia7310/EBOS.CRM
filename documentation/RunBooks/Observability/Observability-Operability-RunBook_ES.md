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

## Clasificacion de Dominio y Recuperacion (MVP)

Arbol de decision de clasificacion (`DomainValidation` vs `DomainConflict` vs `DomainRuleViolation` vs `TransientDomainFailure`):
1. El input o la forma del estado del agregado es invalida antes de ejecutar invariantes de negocio?
   - Si -> clasificar como `DomainValidation`.
   - No -> continuar.
2. La solicitud colisiona con el estado persistido/actual (mismatch de version, comando duplicado/repetido, escritor concurrente)?
   - Si -> clasificar como `DomainConflict`.
   - No -> continuar.
3. Se viola una invariante de negocio con input por lo demas valido (transicion ilegal, violacion append-only, accion de negocio no permitida)?
   - Si -> clasificar como `DomainRuleViolation`.
   - No -> continuar.
4. El fallo esta causado por condiciones temporales y de corta duracion en el limite de ejecucion de dominio (bloqueo transitorio/indisponibilidad/barrera de lectura obsoleta)?
   - Si -> clasificar como `TransientDomainFailure`.
   - No -> clasificar como fallo de dominio desconocido y escalar para analisis de gap de taxonomia.

Matriz de accion de recuperacion:
- `DomainValidation`:
  - Accion primaria: correccion de cliente.
  - Politica de retry: sin retry automatico.
  - Accion operativa: confirmar code/message determinista y guiar correccion al consumidor.
- `DomainConflict`:
  - Accion primaria: retry seguro solo para conflictos de concurrencia/version.
  - Politica de retry: retry acotado con jitter solo si la operacion es idempotente.
  - Accion operativa: identificar subtipo de conflicto (`version_mismatch`, `command_replay`, `already_processed`) y verificar clave de idempotencia/identidad del comando.
- `DomainRuleViolation`:
  - Accion primaria: remediacion de negocio.
  - Politica de retry: no reintentar hasta que cambien las precondiciones de negocio.
  - Accion operativa: escalar a owner funcional con codigo de invariante y id de entidad impactada.
- `TransientDomainFailure`:
  - Accion primaria: retry seguro.
  - Politica de retry: retry acotado con backoff+jitter, luego degradar/fail-fast si se supera el umbral.
  - Accion operativa: validar indicadores transitorios/dependencias y limpiar alertas tras recuperar estabilidad.

## Referencias de Runbook Domain Empresarial

### Ruta de Remediacion de Negocio para Violaciones No Reintentables
1. Identificar la violacion por codigo de dominio determinista (`DOMAIN_RULE_VIOLATION_*`) y capturar `correlationId` + `traceId`.
2. Confirmar clasificacion no reintentable:
   - taxonomia `DomainRuleViolation`
   - precondiciones de negocio aun no satisfechas.
3. Abrir ticket de remediacion de negocio con:
   - codigo de invariante
   - ids de entidad/tenant impactados
   - marcas de tiempo UTC (primera/ultima ocurrencia)
   - impacto operativo.
4. Ejecutar accion de remediacion aprobada (correccion de datos, desbloqueo de estado, override de politica o flujo de aprobacion).
5. Reintentar operacion una vez aplicada la remediacion y verificar:
   - la invariante deja de fallar
   - no hay efectos de negocio duplicados
   - la categoria de evento de dominio esperada se mantiene estable.
6. Adjuntar evidencia de auditoria:
   - id de ticket y aprobador
   - evidencia de estado antes/despues
   - muestras de trazas/logs ligadas a la remediacion.

### Procedimiento de Replay de Compensaciones y Evidencia de Auditoria
1. Seleccionar instancias fallidas de workflow reversible elegibles para replay de compensacion.
2. Validar precondiciones de replay:
   - estado actual elegible (`FAILED` en flujo de privacidad)
   - comando compensatorio disponible y determinista
   - guarda de idempotencia activa.
3. Ejecutar replay del comando compensatorio en lotes controlados con tracking de correlacion.
4. Verificar invariantes post-replay:
   - estado transicionado al estado compensado esperado
   - marcadores de fallo limpiados cuando aplique
   - reglas de transicion monotónica respetadas.
5. Verificar eventos operacionales emitidos:
   - evento tecnico de compensacion emitido
   - sin deriva de categoria frente al catalogo de eventos.
6. Guardar evidencia de auditoria:
   - id de lote de replay y ventana UTC de ejecucion
   - lista de ids de entidades afectadas
   - conteo de operaciones replayed/skipped/failed
   - muestras de `correlationId`/`traceId`
   - operador y aprobador.

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
- Usar: `documentation/RunBooks/Drills/Observability/Observability-Drill-Execution-Template_ES.md`
- Guardar registros completados en: `documentation/RunBooks/Drills/Records_ES/Observability/`

## Checklist Post-Deploy
- Usar: `documentation/RunBooks/Observability/Observability-PostDeploy-Checklist_ES.md`
- Marcar cada item como `PASS/FAIL/N/A` y adjuntar enlaces de evidencia.

## Checklist de Verificacion
- `dotnet build EBOS.CRM.slnx -c Debug`
- `dotnet test tests/EBOS.CRM.ApiTests/EBOS.CRM.ApiTests.csproj -c Debug --filter "FullyQualifiedName~Observability|FullyQualifiedName~Resilience"`
- `dotnet test tests/EBOS.CRM.IntegrationTests/EBOS.CRM.IntegrationTests.csproj -c Debug --filter "FullyQualifiedName~Observability|FullyQualifiedName~Resilience"`
- confirmar visibilidad de logs, metricas y trazas en el entorno objetivo.
