# Backlog de Observabilidad y Resiliencia

Mini TOC:
1. [Alcance y objetivo](#alcance-y-objetivo)
2. [MVP](#mvp)
3. [Empresarial](#empresarial)
4. [Suites de pruebas unitarias](#suites-de-pruebas-unitarias)
5. [Runbooks](#runbooks)
6. [Definicion de terminado](#definicion-de-terminado)

## Alcance y objetivo

Este backlog define el trabajo de implementacion de observabilidad y resiliencia en todas las capas del backend:
- API
- Application
- Contracts (requests y responses)
- Domain
- Infrastructure

Tambien define la cobertura de pruebas para:
- `tests/EBOS.CRM.ApiTests`
- `tests/EBOS.CRM.ConcurrencyTests`
- `tests/EBOS.CRM.IntegrationTests`
- `tests/EBOS.CRM.StressTests`

## MVP

### API

- Agregar middleware de correlacion de solicitudes:
  - Leer el header `X-Correlation-Id`.
  - Generar uno cuando no exista.
  - Incluir el correlation id en headers de respuesta.
- Agregar middleware global de manejo de excepciones:
  - Normalizar excepciones no controladas a `ProblemDetails`.
  - Mapear fallos transitorios de dependencias a codigos reintentables (`503`/`504`).
- Agregar comportamiento resiliente en endpoints:
  - Aplicar limites de timeout por grupo de endpoints.
  - Devolver payload de error deterministico para timeout/cancelacion.
- Agregar telemetria base:
  - Logs estructurados de inicio/fin de solicitud con latencia y resultado.
  - Metricas base (contador e histograma) para volumen, fallos y duracion.

### Application

- Agregar pipeline behaviors de MediatR:
  - Behavior de logging con correlation id y nombre del handler.
  - Instrumentacion del behavior de validacion con eventos de fallo claros.
  - Behavior de retry para excepciones transitorias de infraestructura (reintentos acotados + jitter).
- Introducir politicas de resiliencia por categoria de caso de uso:
  - Consultas idempotentes: retry + timeout.
  - Comandos con efectos secundarios: timeout + guarda con circuit breaker (sin retries inseguros).
- Agregar verificaciones de propagacion de `CancellationToken` en todos los handlers.

### Contracts (Requests/Responses)

- Estandarizar contrato de error:
  - `code`, `message`, `correlationId`, `details[]`, `retryable`.
- Agregar contrato opcional de metadata de respuesta:
  - `traceId`, `elapsedMs`, `timestampUtc`.
- Versionar contratos de request para incluir:
  - Clave de idempotencia en operaciones de escritura cuando aplique.
  - Pistas opcionales de timeout del cliente para operaciones largas.

### Domain

- Agregar taxonomia de errores de dominio:
  - `DomainValidation`, `DomainConflict`, `DomainRuleViolation`, `TransientDomainFailure`.
- Asegurar que los agregados expongan motivos de fallo deterministas (sin excepciones genericas).
- Agregar semantica de comandos de dominio idempotente para rutas criticas de escritura.

### Infrastructure

- Implementar base resiliente de acceso a datos:
  - Configuracion de timeout de comandos de base de datos.
  - Politica de retry para errores transitorios de red/DB.
  - Logging de salud del pool de conexiones y advertencias de saturacion.
- Agregar proteccion a dependencias salientes:
  - Politicas HttpClient (timeout + retry + circuit breaker).
  - Logs estructurados para latencia y clase de fallo de dependencias externas.
- Agregar health checks:
  - Endpoints de liveness y readiness.
  - Readiness debe incluir conectividad DB y dependencias criticas.

## Empresarial

### API

- Agregar rate limiting adaptativo y proteccion ante sobrecarga:
  - Cuotas por tenant/cliente.
  - Respuestas de degradacion controlada con guia de reintento.
- Agregar headers avanzados de observabilidad:
  - Propagacion de contexto W3C (`traceparent`, `tracestate`).
- Agregar anotaciones de SLO por endpoint y logging de incumplimientos en runtime.

### Application

- Agregar orquestacion de resiliencia a nivel de workflow:
  - Hooks de saga/compensacion para recuperacion ante fallos parciales.
  - Hedging en operaciones de lectura seleccionadas, de bajo riesgo.
- Agregar registro de politicas por criticidad:
  - Perfiles de confiabilidad Platinum/Gold/Silver.
- Agregar recarga dinamica de configuracion de politicas sin reinicio.

### Contracts (Requests/Responses)

- Extender contratos con pistas de resiliencia:
  - `retryAfterMs`, `throttleScope`, `degradationMode`.
- Agregar contratos para operaciones asincronas:
  - Respuesta estandar de estado (`pending`, `running`, `failed`, `completed`).
  - Campos de correlacion para polling y callbacks.
- Agregar estrategia de compatibilidad para clientes multiversion con campos de observabilidad.

### Domain

- Agregar acciones compensatorias explicitas a nivel dominio para operaciones reversibles.
- Introducir invariantes orientadas a confiabilidad:
  - Evitar acciones de negocio duplicadas bajo retries.
  - Forzar transiciones de estado monotonicas en workflows largos.
- Agregar clasificacion de eventos de dominio para analitica operacional:
  - Evento de negocio vs tecnico vs anomalia.

### Infrastructure

- Implementar integracion con plataforma de telemetria:
  - Exportacion OpenTelemetry de trazas, metricas y logs.
  - Atributos unificados de recurso (servicio/version/entorno/alcance tenant).
- Agregar resiliencia en mensajeria durable:
  - Patrones outbox/inbox con deduplicacion.
  - Manejo de dead-letter y diagnostico de mensajes poison.
- Agregar endurecimiento de persistencia:
  - Estrategia de replicas de lectura/failover.
  - Aislamiento tipo bulkhead para repositorios criticos.
- Agregar operacion de observabilidad:
  - Definicion de tableros para SLI/SLO.
  - Reglas de alertamiento y mapeo de escalamiento.

## Suites de pruebas unitarias

### ApiTests

- Middleware de correlation id:
  - Usa id entrante cuando existe.
  - Genera id cuando no existe.
  - Retorna correlation id en respuesta.
- Middleware de mapeo de excepciones:
  - Mapea errores de dominio a estado y contrato esperado.
  - Mapea errores transitorios de dependencias a respuesta reintentable.
- Comportamiento de timeout/cancelacion:
  - Retorna payload deterministico de timeout.
  - Conserva correlation id en respuestas de error.
- Comportamiento de endpoints de salud:
  - Liveness independiente de dependencias.
  - Readiness falla cuando una dependencia critica no esta disponible.

### ConcurrencyTests

- Idempotencia ante comandos identicos concurrentes.
- Condiciones de carrera entre retry + timeout sin duplicar escrituras.
- Transiciones de circuit breaker bajo rafagas concurrentes de fallo.
- Escenarios de contencion que validen invariantes y ausencia de estados inconsistentes.

### IntegrationTests

- Propagacion end-to-end de trazas entre API -> Application -> Infrastructure.
- Comportamiento de politicas de resiliencia con dobles reales de infraestructura:
  - fallo transitorio de DB y posterior exito.
  - timeout de dependencia y luego ruta fallback/degradada.
- Verificacion de contratos:
  - Forma de payload de error (`code/message/correlationId/retryable`).
  - Consistencia de metadata de respuesta (`traceId/elapsedMs/timestampUtc`).
- Comportamiento de health checks/readiness con dependencias activadas/desactivadas.

### StressTests

- Carga sostenida valida objetivos de percentiles de latencia y presupuesto de error.
- Carga por rafagas valida rate limiting y degradacion controlada.
- Simulacion de brownout de dependencias valida efectividad del circuit breaker.
- Soak test de larga duracion valida ausencia de cuellos de botella en telemetria/logs y regresiones de memoria.

## Runbooks

- Runbook principal (ingles):
  - `documentation/RunBooks/Observability-Operability-RunBook.md`
- Runbook principal (castellano):
  - `documentation/RunBooks/Observability-Operability-RunBook_ES.md`
- Checklist post-deploy (ingles):
  - `documentation/RunBooks/Observability-PostDeploy-Checklist.md`
- Checklist post-deploy (castellano):
  - `documentation/RunBooks/Observability-PostDeploy-Checklist_ES.md`
- Plantilla de evidencia de drill (ingles):
  - `documentation/RunBooks/Observability-Drill-Record-Template.md`
- Plantilla de evidencia de drill (castellano):
  - `documentation/RunBooks/Observability-Drill-Record-Template_ES.md`

Alcance de runbook a mantener actualizado con la implementacion:
- Flujo de triage de incidentes usando `correlationId` y `traceId` desde solicitudes fallidas.
- Endpoints operativos (`/health/live`, `/health/ready` y endpoint de readiness de observabilidad cuando aplique).
- Reglas de alerta de resiliencia/observabilidad alineadas a umbrales productivos.
- Procedimientos de recuperacion (validacion de migracion, rollback y evidencia post-incidente).
- Cadencia de drills y evidencia minima para trazabilidad y auditoria.

Entregables del backlog relacionados con runbooks:
- MVP:
  - Implementar contratos de error listos para triage con `correlationId` y `traceId`.
  - Asegurar que logs y trazas se puedan consultar por esos identificadores.
  - Validar pasos del runbook con al menos una simulacion de incidente en entorno no productivo.
- Empresarial:
  - Mantener runbook alineado con rate limiting adaptativo, propagacion avanzada de trazas e integracion de plataforma de telemetria.
  - Agregar guia de ajuste de alertas por SLO y refinamiento de rutas de escalado.
  - Registrar resultados de drills y mejorar procedimientos segun RTO/RPO medidos.

## Definicion de terminado

- Todas las tareas de capas del MVP implementadas.
- Items Empresarial desglosados en historias listas para sprint con responsable y estimacion.
- Casos de prueba agregados a las cuatro suites con ejecucion estable en CI.
- Tableros operacionales y alertas disponibles para rutas criticas del MVP.
- Runbooks creados y mantenidos en EN/ES con pasos de triage de incidentes usando correlation id y trace id.
