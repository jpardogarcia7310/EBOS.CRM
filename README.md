# 🚀 EBOS.CRM API

API REST para la gestión CRM de EBOS.  
Construida con **.NET 8**, siguiendo principios de arquitectura limpia, validación con **FluentValidation**, mediación con **MediatR**, y documentación con **Swagger/OpenAPI**.

---

## 📂 Estructura del proyecto

EBOS.CRM.Api 
│ 
├── Controllers 
│   ├── Countries
│   │ └── CountriesController.cs 
│   ├── Statuses
│   │ └── StatusesController.cs 
│   └── TaxRegimes
│	 └── TaxRegimesController.cs 
│
├── Extensions 
│ ├── ApiBehaviorConfig.cs 
│ ├── ConfigureSwaggerOptions.cs 
│ ├── ServiceCollectionExtensions.cs 
│ └── SwaggerConfig.cs 
│ 
├── Middleware 
│ └── ErrorHandlingMiddleware.cs 
│ 
├── Swagger 
│ ├── DebugGroupNameOperationFilter.cs 
│ ├── ErrorResponsesOperationFilter.cs 
│ ├── ValidationProblemDetailsOperationFilter.cs 
│ └── ValidationProblemDetailsSchemaFilter.cs 
│ 
├── Validation 
│ └── FluentValidationActionFilter.cs 
│ 
├── appsettings.json
│ ├── appsettings.Development.json
│ ├── appsettings.Staging.json
│ └── appsettings.Production.json
│
├── EBOS.CRM.Api.http
│
└── Program.cs 

---

## 📌 Namespaces y responsabilidades

- **EBOS.CRM.Api.Extensions**  
  Configuraciones auxiliares (`ApiBehaviorConfig`, `SwaggerConfig`, extensiones de servicios).

- **EBOS.CRM.Api.Middleware**  
  Middlewares personalizados (`ErrorHandlingMiddleware`).

- **EBOS.CRM.Api.Swagger**  
  Filtros y configuraciones de Swagger/OpenAPI.

- **EBOS.CRM.Api.Validation**  
  Filtros y utilidades de FluentValidation.

- **EBOS.CRM.Api.Controllers**  
  Controladores de API organizados por dominio.

---

## ⚙️ Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local o remoto)
- Visual Studio 2022 / VS Code

---

## ▶️ Cómo empezar

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/tu-org/ebos-crm-api.git
   cd ebos-crm-api
2. **Configurar la cadena de conexión**
   Edita appsettings.json y ajusta:
   ```json
    "ConnectionStrings": {
        "CrmDb": "Server=localhost;Database=CrmDb;Trusted_Connection=True;TrustServerCertificate=True;"
    }
3. **Aplicar migraciones**
   ```bash
   dotnet ef database update --project EBOS.CRM.Infrastructure --startup-project EBOS.CRM.Api
4. **Ejecutar API**
   ```bash
   dotnet run --project EBOS.CRM.Api
5. **Acceder a Swagger UI**
   Abre en el navegador: `https://localhost:5001/swagger`

## 🛠️ Tecnologías principales

- ASP.NET Core 8 – Web API
- Entity Framework Core – ORM
- MediatR – Patrón Mediator
- FluentValidation – Validación declarativa
- Swagger / Swashbuckle – Documentación interactiva
- AutoMapper – Mapeo de DTOs

## 📖 Convenciones de errores

La API devuelve errores en formato application/problem+json siguiendo [RFC 7807](https://datatracker.ietf.org/doc/html/rfc7807).
Ejemplo de error de validación:
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
           "code": "VAL_123456"
         }
       ]
     }
   }

## 👥 Contribución

1. Haz un fork del repositorio.
2. Crea una rama para tu feature/fix: git checkout -b feature/nueva-funcionalidad.
3. Haz commit de tus cambios: git commit -m "Añadir nueva funcionalidad".
4. Haz push a tu rama: git push origin feature/nueva-funcionalidad.
5. Abre un Pull Request.

## 📜 Licencia

Este proyecto está bajo la licencia [LGPL v3](https://www.gnu.org/licenses/lgpl-3.0.html).

---