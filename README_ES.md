# EBOS.CRM

EBOS.CRM es un CRM libre y de codigo abierto basado en .NET 8. Proporciona una API REST limpia para gestionar datos maestros de clientes y esta disenado para crecer hasta convertirse en una plataforma CRM completa y modular.

Este proyecto es **Software Libre**. Su objetivo es llegar a ser un stack CRM integral, impulsado por la comunidad, que funcione en Windows (IIS), Linux (Apache + proxy inverso) y macOS (proxy inverso).

## Caracteristicas destacadas

- API REST enfocada en datos maestros de CRM (paises, estados, tipos de direccion, tipos de identificacion, direcciones).
- Arquitectura limpia con separacion de capas API, Application, Domain e Infrastructure.
- Swagger/OpenAPI para explorar la API.
- Construido con .NET 8 y librerias OSS comunes.

## Funcionalidades actuales

- Endpoints CRUD para entidades de catalogo CRM.
- Versionado de API con ejemplos `v1` y `v2`.
- Formato de errores Problem Details (RFC 7807).
- Swagger UI con filtros y validaciones.

## Roadmap (futuro)

- Soporte multi-tenant y aislamiento de datos.
- Autenticacion OAuth2/OpenID Connect con roles.
- Auditoria e historial de cambios.
- Webhooks e integraciones con eventos.
- Modulo UI para administracion y reporting.
- Imagenes Docker y Helm charts para despliegues.

## Estructura del proyecto

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

## Requisitos

- .NET 8 SDK
- SQL Server (local o remoto)

## Obtener el codigo

### Descargar una release

1. Ir a la pagina de Releases del repositorio.
2. Descargar el ZIP o paquete mas reciente.
3. Descomprimir y seguir los pasos de despliegue.

### Descargar el codigo fuente

```bash
git clone https://github.com/jpardogarcia7310/EBOS.CRM.git
cd EBOS.CRM
```

## Compilar y ejecutar

1) Configura la cadena de conexion en `EBOS.CRM.Api/appsettings.json`:

```json
"ConnectionStrings": {
  "CrmDb": "Server=localhost;Database=CrmDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

2) Aplica migraciones:

```bash
dotnet ef database update --project EBOS.CRM.Infrastructure --startup-project EBOS.CRM.Api
```

3) Ejecuta la API:

```bash
dotnet run --project EBOS.CRM.Api
```

4) Abre Swagger UI:

```
https://localhost:5001/swagger
```

## Instalacion

### Windows (IIS)

1. Publica la API:

```bash
dotnet publish EBOS.CRM.Api -c Release -o publish
```

2. Instala IIS y el .NET 8 Hosting Bundle.
3. Crea un sitio en IIS apuntando a la carpeta `publish`.
4. Configura el App Pool en **No Managed Code**.
5. Ajusta variables de entorno y `appsettings.*.json`.
6. Reinicia el sitio.

### Linux (Apache + proxy inverso)

1. Publica la API:

```bash
dotnet publish EBOS.CRM.Api -c Release -o /var/www/eboscrm
```

2. Crea un servicio `systemd` que ejecute la API en un puerto local (ej. 5000).
3. Configura Apache como proxy inverso:

```
ProxyPass / http://127.0.0.1:5000/
ProxyPassReverse / http://127.0.0.1:5000/
```

4. Habilita modulos requeridos (`proxy`, `proxy_http`) y reinicia Apache.

### macOS

1. Publica la API:

```bash
dotnet publish EBOS.CRM.Api -c Release -o /usr/local/eboscrm
```

2. Ejecuta la API con `launchd` o un gestor de procesos.
3. Usa un proxy inverso (Apache o Nginx) para exponer el servicio.

## Ejemplos de uso

```bash
curl -s https://localhost:5001/api/v1/Country
curl -s https://localhost:5001/api/v1/Country/1
curl -s https://localhost:5001/api/v1/Status
curl -s https://localhost:5001/api/v1/AddressType
curl -s https://localhost:5001/api/v2/IdentificationType
```

## Formato de errores

Los errores siguen `application/problem+json` (RFC 7807). Ejemplo:

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

## Tecnologias principales

- ASP.NET Core 8
- Entity Framework Core
- MediatR
- FluentValidation
- Swagger / Swashbuckle
- AutoMapper
