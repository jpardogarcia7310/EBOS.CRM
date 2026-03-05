# Backlog de Observabilidad y Resiliencia

Mini TOC:
1. [Alcance y objetivo](#alcance-y-objetivo)
2. [MVP](#mvp)
3. [Empresarial](#empresarial)
4. [Suites de pruebas unitarias](#suites-de-pruebas-unitarias)
5. [Runbooks](#runbooks)
6. [Definición de terminado](#definición-de-terminado)

## Alcance y objetivo

Este backlog define el trabajo de implementación de observabilidad y resiliencia en todas las capas del backend:
- API
- Application
- Contracts (requests y responses)
- Domain
- Infrastructure

También define la cobertura de pruebas para:
- `tests/EBOS.CRM.ApiTests`
- `tests/EBOS.CRM.ConcurrencyTests`
- `tests/EBOS.CRM.IntegrationTests`
- `tests/EBOS.CRM.StressTests`

## MVP

### API

- Agregar middleware de correlación de solicitudes:
  - Leer el header `X-Correlation-Id`.
  - Generar uno cuando no exista.
  - Incluir el correlation id en headers de respuesta.
- Agregar middleware global de manejo de excepciones:
  - Normalizar excepciones no controladas a `ProblemDetails`.
  - Mapear fallos transitorios de dependencias a códigos de reintento (`503`/`504`).
- Agregar comportamiento resiliente en endpoints:
  - Aplicar limites de timeout por grupo de endpoints.
  - Devolver payload de error determinístico para timeout/cancelacion.
- Agregar telemetría base:
  - Logs estructurados de inicio/fin de solicitud con latencia y resultado.
  - Métrica base (contador e histograma) para volumen, fallos y duración.
- Definición y criterios operativos:
  - La observabilidad de API es la primera capa de triage: toda solicitud fallida debe poder diagnosticarse con `correlationId` + `traceId`.
  - El mapeo de errores debe ser determinístico y estable entre versiones para la misma clase de fallo.
  - Las respuestas de timeout/cancelación deben usar una sola forma de payload e incluir guía de reintento.
- Expectativas de pruebas unitarias:
  - Verificar mapeo de estado para cada clase de taxonomía de error de dominio.
  - Verificar esquema de payload de timeout y propagación de headers.
  - Verificar que logs incluyan nombre de operación, bucket de latencia, clase de estado e identificadores.
- Referencias de runbook para API:
  - El flujo de triage inicia en log de solicitud API -> búsqueda por correlación -> timeline de trazas.
  - Incluir rama de decisión para `4xx no reintentable` vs `5xx reintentable/transitorio`.

### Application

- Agregar pipeline behaviors de MediatR:
  - Behavior de logging con correlation id y nombre del handler.
  - Instrumentación del behavior de validación con eventos de fallo claros.
  - Behavior de retry para excepciones transitorias de infraestructura (reintentos acotados + jitter).
- Introducir políticas de resiliencia por categoria de caso de uso:
  - Consultas idempotentes: retry + timeout.
  - Comandos con efectos secundarios: timeout + guarda con circuit breaker (sin retries inseguros).
- Agregar verificaciones de propagación de `CancellationToken` en todos los handlers.
- Definición y criterios operativos:
  - Application define los límites de orquestación de resiliencia; los reintentos son por política, no ad hoc en handlers.
  - Comandos con efectos secundarios no son reintentables por defecto salvo idempotencia explícitamente garantizada.
  - Los pipeline behaviors deben emitir eventos estructurados de validación, reintentos y resultado final.
- Expectativas de pruebas unitarias:
  - Validar selección de políticas por tipo de handler (query vs command).
  - Validar límite de reintentos y ventana de jitter ante fallos transitorios.
  - Validar que `CancellationToken` se respete antes de I/O costoso.
- Referencias de runbook para Application:
  - Triage de fallo a nivel handler: inspeccionar eventos de behavior antes de logs de infraestructura.
  - Verificación de tormenta de retries: detectar intentos repetidos para la misma clave de operación.

### Contracts (Requests/Responses)

- Estandarizar contrato de error:
  - `code`, `message`, `correlationId`, `details[]`, `retryable`.
- Agregar contrato opcional de metadata de respuesta:
  - `traceId`, `elapsedMs`, `timestampUtc`.
- Versionar contratos de request para incluir:
  - Clave de idempotencia en operaciones de escritura cuando aplique.
  - Pistas opcionales de timeout del cliente para operaciones largas.
- Definición y criterios operativos:
  - Contracts define el límite de soporte con clientes; los campos de observabilidad forman parte del compromiso de compatibilidad.
  - `retryable` se calcula por política del servidor, no por intención del cliente.
  - `details[]` debe ser legible por máquina y acotado para evitar abuso del payload.
- Expectativas de pruebas unitarias:
  - Validar conformidad de esquema para payloads de éxito/error.
  - Validar compatibilidad hacia atrás de campos opcionales de observabilidad.
  - Validar generación determinística de códigos de error por taxonomía.
- Referencias de runbook para Contracts:
  - Verificación de drift de contrato en versiones desplegadas.
  - Guía de troubleshooting para cliente basada en `code`, `retryable` y `details[]`.

### Domain

- Agregar taxonomía de errores de dominio:
  - `DomainValidation`, `DomainConflict`, `DomainRuleViolation`, `TransientDomainFailure`.
- Aclarar el concepto de taxonomía y su uso:
  - En este backlog, una "taxonomía" es un modelo de clasificación de fallos de dominio, no entidades de dominio.
  - Estos tipos son categorías de error (normalmente representadas como tipos de excepción o códigos de error) para estandarizar comportamiento, logging y mapeo en API.
  - Objetivo de la taxonomía: manejo determinístico de fallos, evitar excepciones genéricas, mejorar observabilidad y habilitar retries/fallbacks consistentes.
- Definir cada tipo de la taxonomía de error de dominio:
  - `DomainValidation`: La forma del input/estado es inválida antes de aplicar reglas de negocio (valor obligatorio ausente, formato inválido, valor fuera de rango). Usualmente se mapea a corrección del cliente sin reintento.
  - `DomainConflict`: La operación solicitada colisiona con el estado actual persistido/de dominio (clave duplicada, mismatch de versión, comando ya procesado). Puede ser reintentable solo cuando el conflicto es de concurrencia y el cliente puede reintentar de forma segura.
  - `DomainRuleViolation`: Se rompe una invariante estricta de negocio aun con input sintácticamente válido (límite de crédito excedido, transición de estado ilegal). No reintentable hasta que cambien las condiciones de negocio.
  - `TransientDomainFailure`: Barrera temporal de ejecución a nivel de dominio causada por condiciones de corta duración (servicio de dominio temporalmente no disponible, timeout de bloqueo, lectura obsoleta transitoria). Reintentable con backoff/jitter acotado.
- Asegurar que los agregados expongan motivos de fallo deterministas (sin excepciones genéricas).
- Agregar semántica de comandos de dominio idempotente para rutas criticas de escritura.
- Expectativas de pruebas unitarias:
  - Un set de pruebas por tipo de taxonomía con aserciones de clasificación determinística.
  - Pruebas de concurrencia para `DomainConflict` (choque de versión/comando repetido).
  - Pruebas de invariantes para `DomainRuleViolation` con precondiciones de negocio explícitas.
- Referencias de runbook para Domain:
  - Árbol de decisión de clasificación: validation vs conflict vs rule violation vs transient.
  - Matriz de acción de recuperación: corrección de cliente, retry seguro o remediación de negocio.
  - Sección fuente del runbook:
    - `documentation/RunBooks/Observability/Observability-Operability-RunBook_ES.md` -> `Clasificacion de Dominio y Recuperacion (MVP)`.

### Infrastructure

- Implementar base resiliente de acceso a datos:
  - Configuración de timeout de comandos de base de datos.
  - Política de retry para errores transitorios de red/DB.
  - Logging de salud del pool de conexiones y advertencias de saturación.
- Agregar protección a dependencias salientes:
  - Políticas HttpClient (timeout + retry + circuit breaker).
  - Logs estructurados para latencia y clase de fallo de dependencias externas.
- Agregar health checks:
  - Endpoints de liveness y readiness.
  - Readiness debe incluir conectividad DB y dependencias críticas.
- Definición y criterios operativos:
  - La observabilidad de Infrastructure aporta evidencia de salud, latencia y saturación de dependencias para acotar el incidente.
  - Los criterios de clasificación transitoria deben ser explícitos por tipo de dependencia (DB, red, HTTP, broker).
  - Un fallo de readiness debe incluir identidad de dependencia y clase de fallo.
- Expectativas de pruebas unitarias:
  - Validar composición de políticas timeout/retry/circuit-breaker por dependencia.
  - Validar comportamiento de health checks al activar/desactivar dependencias.
  - Validar advertencias de saturación en umbrales del pool de conexiones.
- Referencias de runbook para Infrastructure:
  - Triage de caídas de dependencias por clase de fallo y radio de impacto.
  - Checklist de recuperación: failover, rollback y verificación post-recuperación.

## Empresarial

### API

- Agregar rate limiting adaptativo y protección ante sobrecarga:
  - Cuotas por tenant/cliente.
  - Respuestas de degradación controlada con guía de reintento.
- Agregar headers avanzados de observabilidad:
  - Propagación de contexto W3C (`traceparent`, `tracestate`).
- Agregar anotaciones de SLO por endpoint y logging de incumplimientos en runtime.
- Definición y criterios operativos:
  - API Empresarial extiende MVP con controles adaptativos y gobierno por SLO.
  - Los límites de tasa deben ser observables por dimensiones tenant/cliente y vinculados al modo de degradación.
- Expectativas de pruebas unitarias:
  - Verificar throttling por tenant y semántica consistente de retry-after.
  - Verificar propagación de contexto de trazas en fronteras asíncronas.
- Referencias de runbook para API:
  - Procedimiento de manejo de sobrecarga con pasos de ajuste de throttling y criterios de escalado.

### Application

- Agregar orquestación de resiliencia a nivel de workflow:
  - Hooks de saga/compensacion para recuperación ante fallos parciales.
  - Hedging en operaciones de lectura seleccionadas, de bajo riesgo.
- Agregar registro de políticas por criticidad:
  - Perfiles de confiabilidad Platinum/Gold/Silver.
- Agregar recarga dinámica de configuración de políticas sin reinicio.
- Definición y criterios operativos:
  - Los workflows Empresarial requieren lógica compensatoria y perfiles de políticas por criticidad.
  - La recarga dinámica de políticas debe ser auditable y reversible.
- Expectativas de pruebas unitarias:
  - Verificar orden de ejecución de compensaciones y recuperación idempotente.
  - Verificar que cambios de política en caliente no romºpan operaciones en vuelo.
- Referencias de runbook para Application:
  - Procedimiento de recuperación de workflow con fallo parcial.
  - Playbook de rollback de políticas ante comportamiento inestable en runtime.

### Contracts (Requests/Responses)

- Extender contratos con pistas de resiliencia:
  - `retryAfterMs`, `throttleScope`, `degradationMode`.
- Agregar contratos para operaciones asíncronas:
  - Respuesta estándar de estado (`pending`, `running`, `failed`, `completed`).
  - Campos de correlación para polling y callbacks.
- Agregar estrategia de compatibilidad para clientes multiversion con campos de observabilidad.
- Definición y criterios operativos:
  - Los contratos Empresarial formalizan ciclo de vida asíncrono, pistas de throttling y transparencia de degradación.
  - El soporte multiversión debe incluir ventanas de deprecación y guía de migración.
- Expectativas de pruebas unitarias:
  - Verificar contratos de transición de estado asíncrono y campos de correlación.
  - Verificar matriz de compatibilidad hacia atrás para clientes versionados.
- Referencias de runbook para Contracts:
  - Triage de incidentes por versión de cliente y pasos de verificación de compatibilidad.

### Domain

- Agregar acciones compensatorias explicitas a nivel dominio para operaciones reversibles.
- Introducir invariantes orientadas a confiabilidad:
  - Evitar acciones de negocio duplicadas bajo retries.
  - Forzar transiciones de estado monotónicas en workflows largos.
- Agregar clasificación de eventos de dominio para analítica operacional:
  - Evento de negocio vs técnico vs anomalía.
  - Aclaración: esta taxonomía es un modelo de clasificación para analítica operacional, no entidades de dominio.
  - `Business`: eventos que representan progreso real de negocio (solicitud aceptada, paso de workflow completado, acción de cliente exitosa).
  - `Technical`: eventos de mecánica de confiabilidad/control sin significado directo de negocio (deduplicación aplicada, intento de retry, guarda por timeout, disparo de compensación).
  - `Anomaly`: eventos que señalan comportamiento de dominio anómalo o sospechoso que requiere triage (brecha inesperada de invariante, combinación de estado imposible, patrón repetido de violación de reglas).
  - La clasificación debe ser determinística por nombre de evento para que los consumidores analíticos reciban dimensiones estables entre versiones.
- Definición y criterios operativos:
  - Domain Empresarial extiende la taxonomía MVP con compensaciones, invariantes de confiabilidad y gobierno de eventos.
  - La prevención de acción de negocio duplicada es obligatoria bajo retries y retries distribuidos.
  - Criterio operativo de compensación: toda operación reversible debe definir comando compensatorio explícito, precondiciones de ejecución y evidencia auditable del resultado.
  - Criterio operativo de invariantes: no se aceptan transiciones regresivas o ambiguas en workflows largos; cada transición debe ser monotónica y validada por estado previo esperado.
  - Criterio operativo de deduplicación: comandos repetidos con misma intención de negocio deben resolverse de forma idempotente (sin efecto adicional) aun con reintentos concurrentes o reentrega distribuida.
  - Criterio operativo de gobernanza de eventos: cada evento de dominio debe publicarse con nombre estable y categoría determinística (`Business`/`Technical`/`Anomaly`) para mantener continuidad analítica entre versiones.
  - Evidencia mínima de cumplimiento: tests unitarios de invariantes/compensación, pruebas de concurrencia para duplicados bajo retry y validación del catálogo de clasificación de eventos.
- Expectativas de pruebas unitarias:
  - Verificar que acciones compensatorias preserven invariantes tras fallo parcial.
  - Verificar transiciones monotónicas en workflows de larga duración.
  - Verificar consistencia de clasificación de eventos para consumidores analíticos.
- Referencias de runbook para Domain:
  - Ruta de remediación de negocio para violaciones no reintentables.
  - Procedimiento de replay de compensaciones y evidencia de auditoría.
  - Sección fuente del runbook:
    - `documentation/RunBooks/Observability/Observability-Operability-RunBook_ES.md` -> `Referencias de Runbook Domain Empresarial`.

### Infrastructure

- Implementar integración con plataforma de telemetría:
  - Exportación OpenTelemetry de trazas, métricas y logs.
  - Atributos unificados de recurso (servicio/version/entorno/alcance tenant).
- Agregar resiliencia en mensajería durable:
  - Patrones outbox/inbox con deduplication.
  - Manejo de dead-letter y diagnóstico de mensajes poison.
- Agregar endurecimiento de persistencia:
  - Estrategia de replicas de lectura/failover.
  - Aislamiento tipo bulkhead para repositorios críticos.
- Agregar operación de observabilidad:
  - Definición de tableros para SLI/SLO.
  - Reglas de alerta y mapeo de escalamiento.
- Definición y criterios operativos:
  - Infrastructure Empresarial debe proveer telemetría end-to-end con dimensiones útiles operativamente.
  - La resiliencia de mensajería requiere evidencia de deduplicación y diagnósticos de dead-letter.
- Expectativas de pruebas unitarias:
  - Verificar corrección de deduplicación outbox/inbox bajo entrega concurrente.
  - Verificar enrutamiento a dead-letter y diagnóstico de mensajes poison.
  - Verificar corrección de señales de dashboard SLI/SLO desde la telemetría emitida.
- Referencias de runbook para Infrastructure:
  - Procedimiento ante degradación del pipeline de telemetría.
  - Procedimiento de drenado/replay de dead-letter con controles de riesgo.

## Suites de pruebas unitarias

Política de cobertura por capa (MVP y Empresarial):
- API: mapeo determinístico de errores, propagación de headers, contratos de timeout, comportamiento de endpoints de salud.
- Application: selección de políticas, retries/timeouts/cancelación, compensaciones y comportamiento de recarga dinámica.
- Contracts: compatibilidad de schema/versiones, determinismo de códigos de error, contratos de ciclo de vida asíncrono.
- Domain: clasificación de taxonomía, cumplimiento de invariantes, comportamiento de conflicto/idempotencia.
- Infrastructure: simulación de fallos de dependencias, composición de políticas, saturación y resiliencia de mensajería.

### ApiTests

- Middleware de correlation id:
  - Usa id entrante cuando existe.
  - Genera id cuando no existe.
  - Retorna correlation id en respuesta.
- Middleware de mapeo de excepciones:
  - Mapea errores de dominio ha estado y contrato esperado.
  - Mapea errores transitorios de dependencias a respuesta de reintento.
- Comportamiento de timeout/cancelacion:
  - Retorna payload determinístico de timeout.
  - Conserva correlation id en respuestas de error.
- Comportamiento de endpoints de salud:
  - Liveness independiente de dependencias.
  - Readiness falla cuando una dependencia critica no esta disponible.

### ConcurrencyTests

- Idempotencia ante comandos idénticos concurrentes.
- Condiciones de carrera entre retry + timeout sin duplicar escrituras.
- Transiciones de circuit breaker bajo ráfagas concurrentes de fallo.
- Escenarios de contención que validen invariantes y ausencia de estados inconsistentes.

### IntegrationTests

- Propagación end-to-end de trazas entre API -> Application -> Infrastructure.
- Comportamiento de políticas de resiliencia con dobles reales de infraestructura:
  - fallo transitorio de DB y posterior éxito.
  - timeout de dependencia y luego ruta fallback/degradada.
- Verificación de contratos:
  - Forma de payload de error (`code/message/correlationId/retryable`).
  - Consistencia de metadata de respuesta (`traceId/elapsedMs/timestampUtc`).
- Comportamiento de health checks/readiness con dependencias activadas/desactivadas.

### StressTests

- Carga sostenida válida objetiva de percentiles de latencia y presupuesto de error.
- Carga por ráfagas válida rate limiting y degradación controlada.
- Simulación de brownout de dependencias válida efectividad del circuit breaker.
- Soak test de larga duración válida ausencia de cuellos de botella en telemetria/logs y regresiones de memoria.

## Runbooks

- Runbook principal (ingles):
  - `documentation/RunBooks/Observability/Observability-Operability-RunBook.md`
- Runbook principal (castellano):
  - `documentation/RunBooks/Observability/Observability-Operability-RunBook_ES.md`
- Checklist post-deploy (ingles):
  - `documentation/RunBooks/Observability/Observability-PostDeploy-Checklist.md`
- Checklist post-deploy (castellano):
  - `documentation/RunBooks/Observability/Observability-PostDeploy-Checklist_ES.md`
- Plantilla de evidencia de drill (ingles):
  - `documentation/RunBooks/Drills/Observability/Observability-Drill-Execution-Template.md`
- Plantilla de evidencia de drill (castellano):
  - `documentation/RunBooks/Drills/Observability/Observability-Drill-Execution-Template_ES.md`

Alcance de runbook a mantener actualizado con la implementación:
- Flujo de triage de incidentes usando `correlationId` y `traceId` desde solicitudes fallidas.
- Endpoints operativos (`/health/live`, `/health/ready` y endpoint de readiness de observabilidad cuando aplique).
- Reglas de alerta de resiliencia/observabilidad alineadas a umbrales productivos.
- Procedimientos de recuperación (validación de migración, rollback y evidencia post-incidente).
- Cadencia de drills y evidencia minima para trazabilidad y auditoria.

Entregables del backlog relacionados con runbooks:
- MVP:
  - Implementar contratos de error listos para triage con `correlationId` y `traceId`.
  - Asegurar que logs y trazas se puedan consultar por esos identificadores.
  - Validar pasos del runbook con al menos una simulación de incidente en entorno no productivo.
- Empresarial:
  - Mantener runbook alineado con rate limiting adaptativo, propagación avanzada de trazas e integración de plataforma de telemetría.
  - Agregar guía de ajuste de alertas por SLO y refinamiento de rutas de escalado.
  - Registrar resultados de drills y mejorar procedimientos según RTO/RPO medidos.

Contenido mínimo de runbook por capa (aplica a MVP y Empresarial):
- API: pasos de triage a nivel solicitud, interpretación de contrato de error, respuesta ante throttling/degradación.
- Application: secuencia de inspección handler/pipeline, contención de tormenta de retries, validación de compensaciones.
- Contracts: checklist de validación de payload, triage de compatibilidad/versiones, plantilla de comunicación a cliente.
- Domain: árbol de decisión de taxonomía, remediación de negocio no reintentable, resolución de conflictos de idempotencia.
- Infrastructure: matriz de diagnóstico de dependencias, checklist de failover/recuperación, chequeos de salud del pipeline de telemetría.

## Definición de terminado

- Todas las tareas de capas del MVP implementadas.
- Items Empresarial desglosados en historias listas para sprint con responsable y estimación.
- Casos de prueba agregados a las cuatro suites con ejecución estable en CI.
- Tableros operacionales y alertas disponibles para rutas críticas del MVP.
- Runbooks creados y mantenidos EN/ES con pasos de triage de incidentes usando correlation id y trace id.

## Anexo A - Versión Ejecutiva

Propósito:
- Proveer una vista concisa de implementación y gobierno para liderazgo y planificación.

MVP por capa:
- API: mapeo determinístico de errores, correlación de solicitudes, manejo de timeout y telemetría base.
- Application: políticas de resiliencia MediatR por tipo de operación, retries acotados para fallos transitorios y propagación de cancelación.
- Contracts: sobre de error estandarizado (`code/message/correlationId/details/retryable`) y metadata de observabilidad.
- Domain: taxonomía explícita de errores de dominio con clasificación determinística y semántica idempotente en escrituras críticas.
- Infrastructure: acceso resiliente a datos/dependencias, health checks y visibilidad de saturación.

Empresarial por capa:
- API: rate limiting adaptativo, propagación avanzada de trazas y seguimiento de SLO por endpoint.
- Application: workflows de compensación, perfiles por criticidad y recarga dinámica de políticas.
- Contracts: contratos de ciclo de vida asíncrono, pistas de throttling/degradación y compatibilidad multiversión.
- Domain: invariantes de confiabilidad, acciones compensatorias y clasificación operacional de eventos.
- Infrastructure: integración de plataforma OpenTelemetry, resiliencia de mensajería durable y tableros/alertas SLI/SLO.

Hitos ejecutivos:
- Entrega: controles MVP aplicados en rutas productivas e items Empresarial desglosados con responsable/estimación.
- Operabilidad: runbooks validados con drills y simulaciones de incidente.
- Calidad: CI estable con cobertura unit/integration/concurrency/stress para controles de resiliencia.

## Anexo B - Versión de Auditoría

Propósito:
- Definir controles auditables, evidencia esperada y criterios de cumplimiento para observabilidad y resiliencia.

Modelo de control:
- Formato de ID de control: `OBS-{MVP|ENT}-{CAPA}-{NN}`.
- Fuentes de evidencia: pruebas, logs de CI, logs de aplicación, trazas, dashboards y registros de drills de runbook.
- Estados de resultado: `Pass`, `Partial`, `Fail`, con acción de remediación obligatoria para resultados no conformes.

Controles MVP por capa:
- API:
  - Control: errores de taxonomía de dominio mapean a estados HTTP y contrato documentado.
  - Evidencia: pruebas unitarias API + muestra de logs/trazas por clase de error.
  - Criterio de cumplimiento: mapeo determinístico, identificadores presentes, bandera de reintento consistente.
- Application:
  - Control: retries/timeouts/cancelación se aplican por política según categoría de handler.
  - Evidencia: pruebas unitarias de pipeline behaviors + telemetría de retries.
  - Criterio de cumplimiento: retries acotados, sin retries inseguros en comandos, cancelación respetada.
- Contracts:
  - Control: estabilidad de esquema de error/metadata y compatibilidad.
  - Evidencia: pruebas de contrato y verificaciones de compatibilidad de versión.
  - Criterio de cumplimiento: sin regresiones de esquema para clientes soportados.
- Domain:
  - Control: clasificación determinística en `DomainValidation`, `DomainConflict`, `DomainRuleViolation`, `TransientDomainFailure`.
  - Evidencia: pruebas unitarias por taxonomía y concurrencia para conflicto/idempotencia.
  - Criterio de cumplimiento: ausencia de excepciones genéricas de dominio en rutas críticas.
- Infrastructure:
  - Control: resiliencia de dependencias y corrección de health/readiness.
  - Evidencia: pruebas de integración con inyección de fallos + checks de endpoints de salud.
  - Criterio de cumplimiento: fallos transitorios reintentados por política y readiness fallando con contexto de dependencia.

Controles Empresarial por capa:
- API:
  - Control: throttling adaptativo y observabilidad de brecha SLO.
  - Evidencia: stress tests, telemetría de throttling y registros de alertas.
  - Criterio de cumplimiento: throttling alineado a política con alertado accionable.
- Application:
  - Control: seguridad de lógica compensatoria y recarga dinámica de políticas.
  - Evidencia: pruebas de workflows con fallos parciales + logs de auditoría de recarga.
  - Criterio de cumplimiento: workflows recuperables y cambios de política reversibles.
- Contracts:
  - Control: ciclo de vida de contratos asíncronos y compatibilidad multiversión.
  - Evidencia: matriz de compatibilidad + pruebas de contrato asíncrono.
  - Criterio de cumplimiento: clientes versionados operan dentro de la ventana publicada.
- Domain:
  - Control: invariantes de confiabilidad y gobierno de clasificación de eventos.
  - Evidencia: pruebas de invariantes + validación de catálogo de eventos.
  - Criterio de cumplimiento: transiciones monotónicas preservadas y duplicados de negocio prevenidos.
- Infrastructure:
  - Control: resiliencia de mensajería, manejo de dead-letter e integridad de plataforma de telemetría.
  - Evidencia: pruebas outbox/inbox, evidencia de drills de dead-letter y chequeos de salud del pipeline.
  - Criterio de cumplimiento: deduplicación correcta y procesamiento dead-letter recuperable.

Cadencia de auditoría y obligaciones de runbook:
- MVP: revisión mensual de controles y al menos un drill de incidente no productivo por trimestre.
- Empresarial: revisión mensual de controles, game day trimestral y revisión de ajuste de alertas por SLO.
- Evidencia mínima obligatoria de runbook: timeline de triage, ruta de decisión, acción de remediación, verificación de recuperación e ítem de mejora en backlog.
