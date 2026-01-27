# 📂 Estructura del proyecto EBOS.CRM.Api

Este documento describe la organización de carpetas, namespaces y responsabilidades dentro del proyecto **EBOS.CRM.Api**.

---

## 📌 Árbol de carpetas

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

### `EBOS.CRM.Api.Extensions`
- **ApiBehaviorConfig.cs**  
  Encapsula la configuración de `ApiBehaviorOptions` y la construcción de `ValidationProblemDetails`.
- **SwaggerConfig.cs**  
  Configuración centralizada de Swagger/OpenAPI (documentación, filtros, respuestas de error).
- **ServiceCollectionExtensions.cs** *(opcional)*  
  Métodos de extensión para registrar servicios comunes.

### `EBOS.CRM.Api.Middleware`
- **ErrorHandlingMiddleware.cs**  
  Middleware global para capturar excepciones y devolver respuestas JSON consistentes (`ProblemDetails`).

### `EBOS.CRM.Api.Swagger`
- **ValidationProblemDetailsSchemaFilter.cs**  
  Define cómo se documenta el esquema de `ValidationProblemDetails` en Swagger.
- **ValidationProblemDetailsOperationFilter.cs**  
  Añade documentación de errores de validación a las operaciones.
- **ErrorResponsesOperationFilter.cs**  
  Registra respuestas comunes (400, 404, 500) en Swagger.

### `EBOS.CRM.Api.Validation`
- **FluentValidationActionFilter.cs**  
  Filtro que ejecuta validaciones de FluentValidation durante el model binding de MVC.

### `EBOS.CRM.Api.Controllers`
- Controladores de API organizados por dominio (ej. `CountriesController`).
- Cada controlador debe devolver respuestas consistentes (`ProblemDetails` en caso de error).

---

## 🎯 Beneficios de esta organización
- **Claridad:** cada carpeta agrupa una responsabilidad concreta.  
- **Mantenibilidad:** `Program.cs` queda minimalista y sin lógica pesada.  
- **Escalabilidad:** fácil añadir nuevos middlewares, filtros o configuraciones.  
- **Consistencia:** todos los namespaces siguen el patrón `EBOS.CRM.Api.[Área]`.

---