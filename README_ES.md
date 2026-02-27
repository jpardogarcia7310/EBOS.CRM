# EBOS.CRM

EBOS.CRM es un CRM libre y de código abierto basado en .NET 8. Proporciona una API REST limpia para gestionar datos maestros de clientes y está diseñado para crecer hasta convertirse en una plataforma CRM completa y modular.

Este proyecto es **Software Libre**. Su objetivo es llegar a ser una pila CRM integral, impulsada por la comunidad, que funcione en Windows (IIS), Linux (Apache + proxy inverso) y macOS (proxy inverso).

## Características destacadas

- API REST enfocada en datos maestros de CRM (países, estados, tipos de dirección, tipos de identificación, direcciones).
- Arquitectura limpia con separación de capas API, Aplicación, Dominio e Infraestructura.
- Swagger/OpenAPI para explorar la API.
- Construido con .NET 8 y librerías OSS comunes.

## Funcionalidades actuales

- Puntos finales CRUD para entidades de catálogo CRM.
- Versionado de API con ejemplos `v1` y `v2`.
- Formato de errores Problem Details (RFC 7807).
- Interfaz de Swagger con filtros y validaciones.
- Base multi-tenant: modelo de dominio, validación, capa intermedia y aislamiento de datos.

## Funcionalidades multi-tenant implementadas

- Entidad Tenant y TenantId en agregados CRM.
- Invariantes con alcance por tenant y aplicación en escritura.
- Abstracción de servicio de contexto de tenant.
- Validación para imponer aislamiento de tenant.
- Filtros globales de EF Core por TenantId.
- Estrategia configurable de aislamiento por esquema/BD.
- Capa intermedia de resolución de tenant (encabezado y subdominio).
- Propagación del contexto de tenant en el manejo de solicitudes.

## Hoja de ruta (futuro)

- Soporte multi-tenant y aislamiento de datos.
- Autenticación OAuth2/OpenID Connect con roles.
- Auditoría e historial de cambios.
- Ganchos web e integraciones con eventos.
- Módulo de interfaz de usuario para administración e informes.
- Imágenes Docker y gráficos Helm para despliegues.

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

## Obtener el código

### Descargar una versión

1. Ir a la página de versiones del repositorio.
2. Descargar el ZIP o paquete más reciente.
3. Descomprimir y seguir los pasos de despliegue.

### Descargar el código fuente

```bash
git clone https://github.com/jpardogarcia7310/EBOS.CRM.git
cd EBOS.CRM
```

## Compilar y ejecutar

1) Configura la cadena de conexión en `EBOS.CRM.Api/appsettings.json`:

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

4) Abre la interfaz de Swagger:

```
https://localhost:5001/swagger
```

## Autenticación (EBOS.Auth)

El IdP aún no existe. Para que la API funcione hoy, se deja configuración lista para dos modos:

- Modo local (sin IdP): `UseAuthority=false` y un `SigningKey` simétrico.
- Modo IdP (cuando EBOS.Auth exista): `UseAuthority=true` y llenar `Authority`/`Audience`.

Ejemplo recomendado para desarrollo local (sin 401 en Swagger):

```json
"Authentication": {
  "Enabled": false,
  "UseAuthority": false,
  "Authority": "http://localhost:5100",
  "Audience": "ebos.crm.api",
  "ValidIssuer": "http://localhost:5100",
  "ValidAudiences": [ "ebos.crm.api" ],
  "SigningKey": "dev-only-ebos-auth-signing-key-change-me"
}
```

Cuando EBOS.Auth esté disponible, cambia a:

```json
"Authentication": {
  "Enabled": true,
  "UseAuthority": true,
  "Authority": "https://auth.tu-dominio.com",
  "Audience": "ebos.crm.api",
  "SigningKey": ""
}
```

## Instalación

### Windows (IIS)

1. Publica la API:

```bash
dotnet publish EBOS.CRM.Api -c Release -o publish
```

2. Instala IIS y el .NET 8 Hosting Bundle.
3. Crea un sitio en IIS apuntando a la carpeta `publish`.
4. Configura el grupo de aplicaciones en **No Managed Code**.
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

4. Habilita módulos requeridos (`proxy`, `proxy_http`) y reinicia Apache.

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
  "title": "One or more validation errors occurred..",
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

## Configuración

### Aislamiento de tenant

`TenantIsolation:TraversalDepth` controla cuán profundo se recorre el grafo de solicitudes para validar el tenant.
El rango permitido se configura con `TenantIsolation:MinTraversalDepth` y
`TenantIsolation:MaxTraversalDepth`.

- Rango: `1` a `50`
- Default: `10`

Ejemplo:

```json
"TenantIsolation": {
  "MinTraversalDepth": 1,
  "MaxTraversalDepth": 50,
  "TraversalDepth": 10
}
```

## Tecnologías principales

- ASP.NET Core 8
- Entity Framework Core
- MediatR
- FluentValidation
- Swagger / Swashbuckle
- AutoMapper
