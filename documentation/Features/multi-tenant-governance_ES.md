# Multi-tenant y Gobierno

Este milestone puede implementarse sin EBOS.Auth porque el trabajo base es aislamiento de datos y alcance por tenant.
El proveedor de identidad solo es una fuente del TenantId efectivo, no una dependencia del modelo ni de los filtros.

## Que es el "input posterior"

El input posterior es el **TenantId efectivo** de cada solicitud. Cuando EBOS.Auth exista, incluira un claim como
`tenant_id` (o `tid`) dentro del JWT. EBOS.CRM leera ese claim y lo asignara al contexto de la solicitud.

## Como funciona hoy (antes de EBOS.Auth)

Se puede implementar y probar multi-tenancy resolviendo el TenantId desde:

- Un header, por ejemplo `X-Tenant-Id`
- Un subdominio, por ejemplo `tenant1.api.domain`
- Un valor fijo en configuracion para desarrollo local

Esto permite ejecutar todo el codigo tenant-aware sin un IdP real.

## Por que no bloquea el trabajo base

El trabajo base es estructural e independiente del IdP:

- Agregar `TenantId` a las entidades multi-tenant y tablas.
- Agregar filtros globales en EF Core para aislamiento por tenant.
- Agregar restricciones unicas por tenant (por ejemplo `(TenantId, Code)`).
- Agregar indices tenant-aware para consultas frecuentes.

Todo esto se puede construir y validar con un TenantId fijo o por header. Cuando EBOS.Auth este listo, solo cambia la
**fuente** del TenantId (claim en lugar de header), no la infraestructura multi-tenant.

## Forma tecnica sugerida

- API: `TenantResolutionMiddleware` define `ICurrentTenantContext.TenantId`.
- Application: `ICurrentTenantContext` disponible para handlers y servicios.
- Infrastructure: `DbContext` aplica `HasQueryFilter(e => e.TenantId == currentTenant.TenantId)`.

## Ruta de migracion a EBOS.Auth

1) Mantener la misma interfaz de contexto.
2) Cambiar el middleware para leer `tenant_id` desde claims.
3) Eliminar el fallback (header/config) si se desea.
