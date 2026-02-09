# EBOS.CRM.Api structure

This document describes folders, namespaces, and responsibilities in EBOS.CRM.Api.

## Folder tree

```
EBOS.CRM.Api
|
|-- Controllers
|   |-- CRM
|   |   |-- Address
|   |   |   `-- AddressController.cs
|   |   |-- BankInformation
|   |   |   `-- BankInformationController.cs
|   |   |-- BranchOffice
|   |   |   `-- BranchOfficeController.cs
|   |   |-- BranchOfficeAddress
|   |   |   `-- BranchOfficeAddressController.cs
|   |   |-- CorporateCustomer
|   |   |   `-- CorporateCustomerController.cs
|   |   |-- CreditAccount
|   |   |   `-- CreditAccountController.cs
|   |   |-- CreditTransaction
|   |   |   `-- CreditTransactionController.cs
|   |   |-- Customer
|   |   |   `-- CustomerController.cs
|   |   |-- CustomerAddress
|   |   |   `-- CustomerAddressController.cs
|   |   |-- IndividualCustomer
|   |   |   `-- IndividualCustomerController.cs
|   |   |-- TaxInformation
|   |   |   `-- TaxInformationController.cs
|   |   `-- TaxInformationAddress
|   |       `-- TaxInformationAddressController.cs
|   `-- EBOS
|       |-- AddressType
|       |   `-- AddressTypeController.cs
|       |-- Country
|       |   `-- CountryController.cs
|       |-- IdentificationType
|       |   `-- IdentificationTypeController.cs
|       |-- Status
|       |   `-- StatusController.cs
|       |-- TenantConfiguration
|       |   `-- TenantConfigurationController.cs
|       |-- TenantQuota
|       |   `-- TenantQuotaController.cs
|       `-- TenantUsageMetric
|           `-- TenantUsageMetricController.cs
|
|-- Extensions
|   |-- ApiBehaviorConfig.cs
|   |-- ConfigureSwaggerOptions.cs
|   |-- CorrelationIdExtensions.cs
|   |-- ServiceCollectionExtensions.cs
|   `-- SwaggerConfig.cs
|
|-- Middleware
|   |-- CorrelationIdMiddleware.cs
|   `-- ErrorHandlingMiddleware.cs
|
|-- Services
|   `-- HttpContextCurrentUserContext.cs
|
|-- Swagger
|   |-- DebugGroupNameOperationFilter.cs
|   |-- ErrorResponsesOperationFilter.cs
|   |-- ValidationProblemDetailsOperationFilter.cs
|   `-- ValidationProblemDetailsSchemaFilter.cs
|
|-- appsettings.Development.json
|-- appsettings.json
|-- appsettings.Staging.json
|-- EBOS.CRM.Api.csproj
|-- EBOS.CRM.Api.csproj.user
|-- EBOS.CRM.Api.http
`-- Program.cs
```

## Namespaces and responsibilities

### EBOS.CRM.Api.Extensions
- ApiBehaviorConfig.cs
  - Centralizes ApiBehaviorOptions configuration and builds ValidationProblemDetails.
  - Usage example:
    - Registered in Program.cs: services.Configure<ApiBehaviorOptions>(ApiBehaviorConfig.Configure)

- ConfigureSwaggerOptions.cs
  - Builds Swagger documents per API version and applies filters.
  - Usage example:
    - Registered in Program.cs: services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
    - Requires SwaggerGen: services.AddSwaggerGen();

- CorrelationIdExtensions.cs
  - Registers correlation ID middleware and related services.
  - Usage example:
    - app.UseCorrelationId()

- SwaggerConfig.cs
  - Central Swagger/OpenAPI configuration.
  - Usage example:
    - Called from Program.cs: SwaggerConfig.ApiVersioning(services)
    - Typical setup:
      - services.AddApiVersioning(...)
      - services.AddVersionedApiExplorer(...)

- ServiceCollectionExtensions.cs
  - Extension methods for app configuration.
  - Usage example:
    - app.UseApiErrorHandling()

### EBOS.CRM.Api.Middleware
- CorrelationIdMiddleware.cs
  - Ensures each request has a correlation ID and adds it to responses.
  - Usage example:
    - app.UseCorrelationId()

- ErrorHandlingMiddleware.cs
  - Global exception handling and consistent JSON errors.
  - Usage example:
    - app.UseApiErrorHandling()

### EBOS.CRM.Api.Services
- HttpContextCurrentUserContext.cs
  - Resolves the current user context from the HTTP request.
  - Usage example:
    - services.AddScoped<ICurrentUserContext, HttpContextCurrentUserContext>();

### EBOS.CRM.Api.Swagger
- DebugGroupNameOperationFilter.cs
  - Adds diagnostic metadata to Swagger operations.
- ValidationProblemDetailsSchemaFilter.cs
  - Documents ValidationProblemDetails in Swagger.
- ValidationProblemDetailsOperationFilter.cs
  - Adds validation error responses.
- ErrorResponsesOperationFilter.cs
  - Adds common error responses (400, 404, 500).

### EBOS.CRM.Api.Controllers
- Domain-focused API controllers.
- CRM controllers live under Controllers/CRM and share the same route pattern.
- EBOS controllers live under Controllers/EBOS and are read-only or governance endpoints.
- All routes are singular and versioned: /api/v{version}/{Controller}
- Usage examples:
  - GET /api/v1/Country
  - GET /api/v1/Status
  - GET /api/v1/AddressType
  - GET /api/v2/IdentificationType

## Design benefits
- Clarity: each folder maps to a single responsibility.
- Maintainability: Program.cs stays thin.
- Scalability: easy to add new middleware and Swagger filters.
- Consistency: namespaces follow EBOS.CRM.Api.[Area].
