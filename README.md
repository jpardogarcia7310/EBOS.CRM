# EBOS.CRM API

This document describes the EBOS.CRM REST API and how to run it locally.

## Project structure

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

## Requirements

- .NET 8 SDK
- SQL Server (local or remote)

## Run locally

1) Configure connection string in `EBOS.CRM.Api\appsettings.json`:

```json
"ConnectionStrings": {
  "CrmDb": "Server=localhost;Database=CrmDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

2) Apply migrations:

```bash
dotnet ef database update --project EBOS.CRM.Infrastructure --startup-project EBOS.CRM.Api
```

3) Run the API:

```bash
dotnet run --project EBOS.CRM.Api
```

4) Open Swagger UI:

```
https://localhost:5001/swagger
```

## API usage examples

All endpoints are singular. Replace `v1` with the version you need.

Get all countries:

```bash
curl -s https://localhost:5001/api/v1/Country
```

Get one country:

```bash
curl -s https://localhost:5001/api/v1/Country/1
```

Get all statuses:

```bash
curl -s https://localhost:5001/api/v1/Status
```

Get all address types:

```bash
curl -s https://localhost:5001/api/v1/AddressType
```

Get all identification types (v2):

```bash
curl -s https://localhost:5001/api/v2/IdentificationType
```

## Error format

Errors follow `application/problem+json` (RFC 7807). Example validation error:

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "name": [ "Name is required" ]
  },
  "errorsDetailed": {
    "name": [
      {
        "message": "Name is required",
        "code": "VAL_4A1F2C3D4B5E"
      }
    ]
  }
}
```

## Main technologies

- ASP.NET Core 8
- Entity Framework Core
- MediatR
- FluentValidation
- Swagger / Swashbuckle
- AutoMapper
