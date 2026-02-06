# EBOS.CRM

EBOS.CRM is a free and open-source CRM service built on .NET 8. It provides a clean REST API for managing core customer data and is designed to grow into a full-featured, modular CRM platform.

This project is **Free Software**. It aims to become a comprehensive, community-driven CRM stack that can run on Windows (IIS), Linux (Apache + reverse proxy), and macOS (reverse proxy).

## Highlights

- REST API focused on CRM master data (countries, statuses, address types, identification types, addresses).
- Clean architecture with separation of API, Application, Domain, and Infrastructure layers.
- Swagger/OpenAPI for API exploration.
- Built with modern .NET 8 and common OSS libraries.

## Current features

- CRUD-style endpoints for CRM catalog entities.
- API versioning with `v1` and `v2` examples.
- Problem Details (RFC 7807) error format.
- Swagger UI with custom filters and validation details.

## Roadmap (planned)

- Multi-tenant support and tenant-aware data isolation.
- OAuth2/OpenID Connect authentication with fine-grained roles.
- Audit trails and data history for key entities.
- Integration webhooks and event-driven data sync.
- UI module for CRM administration and reporting.
- Docker images and Helm charts for production deployments.

## Project structure

```
EBOS.CRM.Api
|-- Controllers
|-- Extensions
|-- Middleware
|-- Swagger
|-- Validation
|-- appsettings.json
|-- EBOS.CRM.Api.http
`-- Program.cs
```

## Requirements

- .NET 8 SDK
- SQL Server (local or remote)

## Get the code

### Download a release

1. Go to the repository Releases page.
2. Download the latest ZIP or package for your platform.
3. Unzip and run the deployment steps below.

### Download the source

```bash
git clone https://github.com/jpardogarcia7310/EBOS.CRM.git
cd EBOS.CRM
```

## Build and run

1) Configure the connection string in `EBOS.CRM.Api/appsettings.json`:

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

## Installation

### Windows (IIS)

1. Publish the API:

```bash
dotnet publish EBOS.CRM.Api -c Release -o publish
```

2. Install IIS and the .NET 8 Hosting Bundle.
3. Create a new IIS site pointing to the `publish` folder.
4. Ensure the app pool is set to **No Managed Code**.
5. Configure environment variables and `appsettings.*.json`.
6. Restart the site.

### Linux (Apache + reverse proxy)

1. Publish the API:

```bash
dotnet publish EBOS.CRM.Api -c Release -o /var/www/eboscrm
```

2. Create a `systemd` service to run the API on a local port (e.g., 5000).
3. Configure Apache reverse proxy:

```
ProxyPass / http://127.0.0.1:5000/
ProxyPassReverse / http://127.0.0.1:5000/
```

4. Enable required modules (`proxy`, `proxy_http`) and restart Apache.

### macOS

1. Publish the API:

```bash
dotnet publish EBOS.CRM.Api -c Release -o /usr/local/eboscrm
```

2. Run the API with `launchd` or a process manager.
3. Use a reverse proxy (Apache or Nginx) to expose the service.

## API usage examples

```bash
curl -s https://localhost:5001/api/v1/Country
curl -s https://localhost:5001/api/v1/Country/1
curl -s https://localhost:5001/api/v1/Status
curl -s https://localhost:5001/api/v1/AddressType
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

## Configuration

### Tenant isolation

`TenantIsolation:TraversalDepth` controls how deep the tenant validation scans request graphs.
The allowed range is configured with `TenantIsolation:MinTraversalDepth` and
`TenantIsolation:MaxTraversalDepth`.

- Range: `1` to `50`
- Default: `10`

Example:

```json
"TenantIsolation": {
  "MinTraversalDepth": 1,
  "MaxTraversalDepth": 50,
  "TraversalDepth": 10
}
```

## Main technologies

- ASP.NET Core 8
- Entity Framework Core
- MediatR
- FluentValidation
- Swagger / Swashbuckle
- AutoMapper
