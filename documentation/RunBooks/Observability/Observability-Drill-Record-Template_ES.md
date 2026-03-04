# Plantilla de Evidencia de Drill de Observabilidad y Resiliencia

Usa esta plantilla para cada ejecucion de drill.
Guardar registros en `documentation/RunBooks/Drills/Records_ES/`.

## Metadatos del Drill
- Drill ID:
- Fecha/Hora (UTC):
- Entorno:
- Operador(es):
- Revisor:
- Tipo de drill:
  - timeout de dependencia + circuit breaker
  - triage de alta tasa de error por correlacion/traza
  - rollback (app + configuracion)

## Escenario
- Objetivo:
- Precondiciones:
- Pasos de disparo:

## Deteccion y Respuesta
- Fuente de deteccion (alerta/dashboard/log):
- Tiempo de deteccion (minutos):
- Primer `correlationId` con fallo:
- `traceId` representativo:
- Endpoint(s) impactado(s):
- Acciones ejecutadas:
- Seccion de runbook utilizada:

## Resultado de Recuperacion
- Tiempo de recuperacion (minutos):
- Objetivo RTO cumplido (PASS/FAIL):
- Objetivo RPO cumplido (PASS/FAIL):
- Resumen de impacto de negocio:

## Evidencias
- Evidencia de consulta de logs (por `correlationId`):
- Evidencia de trazas (por `traceId`):
- Enlaces/resultados de consultas de metricas:
- Evidencia de notificaciones/routing de alertas:
- Referencias de ticket de despliegue/incidente:

## Lecciones Aprendidas
- Que funciono:
- Que fallo:
- Acciones:
  - Responsable:
  - Fecha objetivo:
  - Tareas:
