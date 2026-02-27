# Multi-tenant y Gobierno

Este hito puede implementarse sin EBOS.Auth porque el trabajo base es aislamiento de datos y alcance por tenant.
El proveedor de identidad solo es una fuente del TenantId efectivo, no una dependencia del modelo ni de los filtros.

Mini TOC:
1. [Qué es la "entrada posterior"](#que-es-la-entrada-posterior)
2. [Cómo funciona hoy (antes de EBOS.Auth)](#como-funciona-hoy-antes-de-ebosauth)
3. [Por qué no bloquea el trabajo base](#por-que-no-bloquea-el-trabajo-base)
4. [Forma técnica sugerida](#forma-tecnica-sugerida)
5. [Ruta de migración a EBOS.Auth](#ruta-de-migracion-a-ebosauth)
6. [Pruebas](#pruebas)
7. [Pruebas de dominio](#pruebas-de-dominio)
8. [Pruebas de aplicación](#pruebas-de-aplicacion)
9. [Pruebas de controladores](#pruebas-de-controladores)
10. [Pruebas de integración](#pruebas-de-integracion)
11. [Pruebas de mapeo](#pruebas-de-mapeo)
12. [Referencia de suites de pruebas existentes](#referencia-de-suites-de-pruebas-existentes)

## Qué es la "entrada posterior"

La entrada posterior es el **TenantId efectivo** de cada solicitud. Cuando EBOS.Auth exista, incluirá un atributo como
`tenant_id` (o `tid`) dentro del JWT. EBOS.CRM leerá ese atributo y lo asignará al contexto de la solicitud.

## Cómo funciona hoy (antes de EBOS.Auth)

Se puede implementar y probar multitenencia resolviendo el TenantId desde:

- Un encabezado, por ejemplo `X-Tenant-Id`
- Un subdominio, por ejemplo `tenant1.api.domain`
- Un valor fijo en configuración para desarrollo local

Esto permite ejecutar todo el código con alcance por tenant sin un IdP real.

## Por qué no bloquea el trabajo base

El trabajo base es estructural e independiente del IdP:

- Agregar `TenantId` a las entidades multi-tenant y tablas.
- Agregar filtros globales en EF Core para aislamiento por tenant.
- Agregar restricciones únicas por tenant (por ejemplo `(TenantId, Code)`).
- Agregar índices con alcance por tenant para consultas frecuentes.

Todo esto se puede construir y validar con un TenantId fijo o por header. Cuando EBOS.Auth esté listo, solo cambia la
**fuente** del TenantId (claim en lugar de header), no la infraestructura multi-tenant.

## Forma técnica sugerida

- API: la capa intermedia `TenantResolutionMiddleware` define `ICurrentTenantContext.TenantId`.
- Application: `ICurrentTenantContext` disponible para handlers y servicios.
- Infraestructura: `DbContext` aplica `HasQueryFilter(e => e.TenantId == currentTenant.TenantId)`.
- Configuración: `MultiTenant:SchemaTargets` controla qué esquemas se renombran en esquema-por-tenant; incluye `CRM` y `EBOS` cuando ambos requieren aislamiento.

## Ruta de migración a EBOS.Auth

1) Mantener la misma interfaz de contexto.
2) Cambiar la capa intermedia para leer `tenant_id` desde atributos.
3) Eliminar el fallback (header/config) si se desea.

## Pruebas

### Pruebas de dominio

- Invariantes con alcance por tenant en entidades que definen límites de aislamiento (`TenantId` obligatorio, identidades/llaves seguras por tenant).
- Reglas de aislamiento para referencias entre tenants y prevención de cruces no permitidos.

### Pruebas de aplicación

- Resolución y propagación del contexto de tenant a través de comportamientos/canales de aplicación.
- Aislamiento por tenant en manejadores (guardas de lectura/escritura, políticas y validaciones con tenant).

### Pruebas de controladores

- Pruebas de resolución por encabezado/subdominio y rechazo de solicitudes con tenant inválido.
- Verificación de que el contexto de tenant es obligatorio en operaciones con alcance por tenant.

### Pruebas de integración

- Validación extremo a extremo del aislamiento por tenant entre API, aplicación y persistencia (mismo endpoint, distintos tenants).
- Verificación de que datos creados en un tenant no son visibles ni mutables desde otro tenant.

### Pruebas de mapeo

- Comprobaciones de mapeo/configuración para DTOs, modelos de solicitud y opciones con contexto de tenant.

### Referencia de suites de pruebas existentes

- `tests/EBOS.CRM.ApiTests`: validación unitaria de comportamientos de tenant (capa intermedia, propagación de contexto de tenant, aislamiento en comportamientos y validadores).
- `tests/EBOS.CRM.ConcurrencyTests`: escenarios concurrentes que verifican límites por tenant y consistencia cuando múltiples operaciones impactan recursos compartidos.
- `tests/EBOS.CRM.IntegrationTests`: pruebas extremo a extremo para acceso a datos con alcance por tenant y aislamiento entre capas API, aplicación y persistencia.
- `tests/EBOS.CRM.StressTests`: escenarios de carga sostenida en endpoints con alcance por tenant para validar estabilidad, latencia y ausencia de fuga entre tenants bajo presión.
