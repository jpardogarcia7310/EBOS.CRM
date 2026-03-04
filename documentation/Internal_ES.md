# Arquitectura Interna de EBOS.CRM (Actual)

Este documento es la referencia técnica interna para la línea base enterprise actual de la solución EBOS.CRM.

## Módulos de la Solución

- `EBOS.CRM.Api`
  - superficie HTTP, endpoints versionados, policies, middleware, Swagger, health/readiness y endpoint de métricas.
- `EBOS.CRM.Application`
  - casos de uso (commands/queries), validadores, handlers y reglas de orquestación.
- `EBOS.CRM.Domain`
  - agregados/entidades, invariantes, servicios de dominio y contratos de dominio.
- `EBOS.CRM.Infrastructure`
  - persistencia EF Core, migraciones, repositorios, outbox, integraciones y wiring de telemetría.
- `EBOS.CRM.Contracts`
  - contratos DTO de API/aplicación y modelos de payload críticos para compatibilidad.

## Capa API (EBOS.CRM.Api)

- Los controladores están divididos por dominio:
  - `Controllers/CRM/*`
  - `Controllers/EBOS/*`
  - `Controllers/Observability/*`
  - `Controllers/Operations/*`
- Estándares:
  - rutas versionadas (`/api/v{version}/...`)
  - middleware centralizado de manejo de errores
  - middleware de correlation ID
  - autorización basada en policies (incluyendo operaciones sensibles de Customer 360)
  - OpenAPI generado y protegido con tests de compatibilidad por snapshot

## Capa Application (EBOS.CRM.Application)

- Patrón:
  - DTO de command/query
  - validator
  - handler
  - abstracciones de repositorio + contexto tenant/usuario actual
- Alcance:
  - módulos CRM (Customer 360, Sales, Service, entidades maestras)
  - módulos de gobierno EBOS
  - flujos de privacidad (solicitud, ejecución, retención)
  - operaciones de merge lineage y dedupe

## Capa Domain (EBOS.CRM.Domain)

- Enfoque enterprise:
  - invariantes forzadas por métodos de dominio
  - cambios de estado basados en transiciones
  - reducción de mutabilidad anémica en entidades clave de Customer 360
  - entidades con control de concurrencia (row version donde aplica)

## Capa Infrastructure (EBOS.CRM.Infrastructure)

- EF Core:
  - mappings por agregado
  - migraciones SQL Server
  - artefactos snapshot/designer
- Repositorios:
  - implementaciones CRM y EBOS
  - comportamiento contrato base (tenant, paginación, filtros soft-delete/erased)
- Hardening Customer 360:
  - persistencia de lineage de merge (`CustomerMergeHistories`)
  - persistencia de solicitudes de privacidad (`CustomerPrivacyRequests`)
  - persistencia outbox (`AuditOutboxMessages`)
  - hardening de estrategia/índices de dedupe
- Outbox:
  - servicio de dispatch + dispatcher en background
  - comportamiento de retry/fallos transitorios cubierto por pruebas

## Suites de Pruebas

- `tests/EBOS.CRM.ApiTests`
  - pruebas de controladores, validators/handlers, invariantes de dominio, contratos y servicios/infra unitarios.
- `tests/EBOS.CRM.IntegrationTests`
  - comportamiento de endpoints, auth/aislamiento tenant, E2E Customer 360, hardening/idempotencia SQL Server, compatibilidad OpenAPI.
- `tests/EBOS.CRM.ConcurrencyTests`
  - escenarios de concurrencia en endpoint e infra/app (outbox dispatcher, retention service, conflictos de repositorio).
- `tests/EBOS.CRM.StressTests`
  - escenarios de alto volumen Customer 360 (outbox backlog, merge/dedupe, throughput/latencia de retención).

## Quality Gates CI/CD

- Workflow: `.github/workflows/customer360-suites-ci.yml`
- Jobs separados:
  - API suite
  - Integration suite
  - Concurrency suite
  - Stress suite
  - Integration SQL Server suite (`USE_TESTCONTAINERS=true`)
- El hardening SQL Server incluye:
  - verificación de apply/rollback de migraciones
  - escenarios de contención de escritura y consistencia
  - chequeos de idempotencia
  - guard test para duplicados `CreateTable` en migraciones

## Observabilidad y Operabilidad

- Activos:
  - `documentation/Observability/prometheus/*`
  - `documentation/Observability/grafana/*`
  - `documentation/Observability/docker-compose.observability.yml`
- Provisioning:
  - datasource + dashboard de Grafana
  - reglas de alerta Prometheus + routing Alertmanager
- Scripts de validación en CI:
  - `documentation/Observability/ci/validate-observability.sh`
  - `documentation/Observability/ci/smoke-observability.sh`

## Runbooks y Documentación Operativa

- `documentation/RunBooks/Customer360-Operability-RunBook_ES.md`
- `documentation/RunBooks/Customer360-PostDeploy-Checklist_ES.md`
- Plantilla legacy de drill en archivo único:
  - `documentation/RunBooks/Customer360-Drill-Record-Template_ES.md`
- Modelo recomendado por ejecución:
  - `documentation/RunBooks/Drills/README.md`
  - `documentation/RunBooks/Drills/Customer360-Drill-Execution-Template_ES.md`
  - `documentation/RunBooks/Drills/Records_ES/`
- También existen equivalentes en inglés en la misma carpeta.

## Notas Internas

- Tratar migraciones como artefactos históricos inmutables; corregir hacia delante cuando sea posible.
- Mantener checks estrictos de seguridad/policies en pruebas; evitar desactivar auditoría/auth global salvo en fixtures de prueba explícitamente acotadas.
- Preferir pruebas deterministas frente a comportamientos sensibles al timing para estabilidad en CI.
