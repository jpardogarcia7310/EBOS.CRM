# Multi-tenant & Governance

This milestone can be implemented without EBOS.Auth because the core work is data isolation and tenant scoping. The
identity provider is only an input source for the effective TenantId, not a dependency for the data model or filters.

Mini TOC:
1. [What is the "later input"](#what-is-the-later-input)
2. [How it works today (before EBOS.Auth)](#how-it-works-today-before-ebosauth)
3. [Why it does not block the base work](#why-it-does-not-block-the-base-work)
4. [Suggested technical shape](#suggested-technical-shape)
5. [Migration path to EBOS.Auth](#migration-path-to-ebosauth)
6. [Tests](#tests)
7. [Domain tests](#domain-tests)
8. [Application tests](#application-tests)
9. [Controller tests](#controller-tests)
10. [Integration tests](#integration-tests)
11. [Mapping tests](#mapping-tests)
12. [Existing test suites reference](#existing-test-suites-reference)

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
- Configuration: `MultiTenant:SchemaTargets` controls which schemas are renamed for schema-per-tenant; include `CRM` and `EBOS` when both require isolation.

## Migration path to EBOS.Auth

1) Keep the same context interface.
2) Switch the middleware to read `tenant_id` from JWT claims.
3) Remove the fallback (header/config) if desired.

## Tests

### Domain tests

- Tenant-scoped invariants on entities implementing tenant boundaries (`TenantId` required, tenant-safe identity/keys).
- Isolation rules for cross-tenant references and forbidden cross-tenant traversal assumptions.

### Application tests

- Tenant context resolution and propagation through application behaviors/pipelines.
- Tenant isolation behavior in handlers (read/write guards, policy behaviors, validation-time tenant checks).

### Controller tests

- Tenant header/subdomain resolution path tests and bad-tenant request rejection.
- Endpoint-level enforcement that tenant context is required for tenant-scoped operations.

### Integration tests

- End-to-end tenant isolation across API, application, and persistence (same endpoint, different tenants).
- Validation that data created under one tenant is never visible/mutable from another tenant.

### Mapping tests

- Mapping/configuration checks for tenant-aware DTOs, request models, and options that carry tenant context.

### Existing test suites reference

- `tests/EBOS.CRM.ApiTests`: unit-level validation of tenant behaviors (middleware, tenant context propagation, tenant isolation behavior, validators, and policy checks).
- `tests/EBOS.CRM.ConcurrencyTests`: concurrent request scenarios that assert tenant boundaries and consistency when multiple operations hit shared resources at the same time.
- `tests/EBOS.CRM.IntegrationTests`: end-to-end checks for tenant-scoped data access and isolation across API, application, and persistence layers.
- `tests/EBOS.CRM.StressTests`: sustained high-load scenarios over tenant-aware endpoints to validate stability, latency behavior, and absence of tenant leakage under pressure.
