# Plantilla de Ejecucion de Drill de Observabilidad y Resiliencia

Usa este archivo como plantilla base para cada registro de ejecucion de drill.

## Metadatos del Drill
- Drill ID:
- Fecha/Hora (UTC):
- Entorno:
- Operador(es):
- Revisor:
- Frecuencia:
  - mensual
  - trimestral
- Tipo de drill:
  - timeout de dependencia + circuit breaker
  - triage de alta tasa de error (correlationId/traceId)
  - rollback (aplicacion + configuracion de resiliencia)

## Alcance y Objetivo
- Objetivo:
- Componentes en alcance:
- Componentes fuera de alcance:

## Precondiciones
- Runbook utilizado:
- Accesos requeridos verificados:
- Feature flags/configuracion:
- Datos/setup de tenant:

## Pasos de Ejecucion
1.
2.
3.

## Deteccion y Respuesta
- Fuente de deteccion (alerta/dashboard/log):
- Tiempo de deteccion (minutos):
- Primer `correlationId` con fallo:
- `traceId` representativo:
- Acciones de respuesta:
- Requirio escalado? (Si/No):

## Recuperacion y Validacion
- Tiempo de recuperacion (minutos):
- Objetivo RTO cumplido (PASS/FAIL):
- Objetivo RPO cumplido (PASS/FAIL):
- Validacion funcional realizada:
- Resumen de impacto de negocio:

## Evidencias
- URL de ejecucion CI/pipeline:
- Consultas/resultados Prometheus:
- Capturas Grafana:
- Notificaciones de alerta:
- Consulta de logs por `correlationId`:
- Consulta de trazas por `traceId`:
- Tickets relacionados:

## Lecciones Aprendidas y Acciones
- Que funciono:
- Que fallo:
- Acciones:
  - Responsable:
  - Fecha objetivo:
  - Estado:
