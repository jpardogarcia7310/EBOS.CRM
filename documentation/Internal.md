# EBOS.CRM.Api structure

This document describes folders, namespaces, and responsibilities in EBOS.CRM.Api.

## Folder tree

EBOS.CRM.Api
|
|-- Controllers
|   |-- AddressType
|   |   `-- AddressTypeController.cs
|   |-- Country
|   |   `-- CountryController.cs
|   |-- CRM
|   |   `-- Address
|   |       `-- AddressController.cs
|   |-- IdentificationType
|   |   `-- IdentificationTypesController.cs
|   `-- Status
|       `-- StatusController.cs
|
|-- Extensions
|   |-- ApiBehaviorConfig.cs
|   |-- ConfigureSwaggerOptions.cs
|   |-- ServiceCollectionExtensions.cs
|   `-- SwaggerConfig.cs
|
|-- Middleware
|   `-- ErrorHandlingMiddleware.cs
|
|-- Swagger
|   |-- DebugGroupNameOperationFilter.cs
|   |-- ErrorResponsesOperationFilter.cs
|   |-- ValidationProblemDetailsOperationFilter.cs
|   `-- ValidationProblemDetailsSchemaFilter.cs
|
|-- Validation
|
|-- appsettings.json
|   |-- appsettings.Development.json
|   `-- appsettings.Staging.json
|
|-- EBOS.CRM.Api.http
`-- Program.cs

## Namespaces and responsibilities

### EBOS.CRM.Api.Extensions
- ApiBehaviorConfig.cs
  - Centralizes ApiBehaviorOptions configuration and builds ValidationProblemDetails.
  - Usage example:
    - Registered in Program.cs: services.Configure<ApiBehaviorOptions>(ApiBehaviorConfig.Configure)

- SwaggerConfig.cs
  - Central Swagger/OpenAPI configuration.
  - Usage example:
    - Called from Program.cs: SwaggerConfig.ApiVersioning(services)

- ServiceCollectionExtensions.cs
  - Extension methods for app configuration.
  - Usage example:
    - app.UseApiErrorHandling()

### EBOS.CRM.Api.Middleware
- ErrorHandlingMiddleware.cs
  - Global exception handling and consistent JSON errors.
  - Usage example:
    - app.UseApiErrorHandling()

### EBOS.CRM.Api.Swagger
- ValidationProblemDetailsSchemaFilter.cs
  - Documents ValidationProblemDetails in Swagger.
- ValidationProblemDetailsOperationFilter.cs
  - Adds validation error responses.
- ErrorResponsesOperationFilter.cs
  - Adds common error responses (400, 404, 500).

### EBOS.CRM.Api.Controllers
- Domain-focused API controllers.
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
