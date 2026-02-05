# Multi-tenant & Governance

This milestone can be implemented without EBOS.Auth because the core work is data isolation and tenant scoping. The
identity provider is only an input source for the effective TenantId, not a dependency for the data model or filters.

## What is the "later input"

The later input is the **effective TenantId** for each request. When EBOS.Auth exists, it will include a claim such as
`tenant_id` (or `tid`) in the JWT. EBOS.CRM will read that claim and set the TenantId in the request context.

## How it works today (before EBOS.Auth)

You can still implement and test multi-tenancy by resolving TenantId from:

- A request header, e.g. `X-Tenant-Id`
- A subdomain, e.g. `tenant1.api.domain`
- A fixed value in configuration for local development

This allows all tenant-aware code to run without a real IdP.

## Why it does not block the base work

The base work is structural and independent of the IdP:

- Add `TenantId` to multi-tenant entities and database tables.
- Add global query filters in EF Core to enforce tenant isolation.
- Add tenant-aware uniqueness constraints (e.g., `(TenantId, Code)`).
- Add tenant-aware indexes for common lookup paths.

All of this can be built and validated with a fixed TenantId or a header-based resolver. When EBOS.Auth is ready, you
only change the **source** of TenantId (claim instead of header), not the multi-tenant infrastructure itself.

## Suggested technical shape

- API: `TenantResolutionMiddleware` sets `ICurrentTenantContext.TenantId`.
- Application: `ICurrentTenantContext` available to handlers and services.
- Infrastructure: `DbContext` applies `HasQueryFilter(e => e.TenantId == currentTenant.TenantId)`.

## Migration path to EBOS.Auth

1) Keep the same context interface.
2) Switch the middleware to read `tenant_id` from JWT claims.
3) Remove the fallback (header/config) if desired.
