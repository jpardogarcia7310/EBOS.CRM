# AGENTS

## Alcance
Este repositorio contiene la solución EBOS.CRM (API, Application, Domain, Infrastructure) más pruebas unitarias e integración.
Use las instrucciones de abajo al hacer cambios en esta solución.

## Mapa rápido de arquitectura
- API: `EBOS.CRM.Api`
- Aplicación (CQRS/Manejadores, contratos): `EBOS.CRM.Application`
- Dominio (entidades, interfaces): `EBOS.CRM.Domain`
- Infraestructura (EF Core, repositorios): `EBOS.CRM.Infrastructure`
- Pruebas:
  - Pruebas unitarias: `tests/EBOS.CRM.ApiTests`
  - Pruebas de integración: `tests/EBOS.CRM.Api.IntegrationTests`

## Convenciones de pruebas
- Los endpoints de listas devuelven una **lista sin contenedor**:
  - DTO: `IReadOnlyCollection<T>`
  - Las pruebas deben deserializar endpoints de listas como `IReadOnlyCollection<T>`.
  - Ayudantes:
    - Pruebas unitarias/de integración: `ReadItemsAsync<T>()` en `tests/**/TestUtils/HttpContentExtensions.cs`
    - Ayudante de controladores: `GetFirstIdAsync<T>()` en `tests/EBOS.CRM.ApiTests/TestUtils/ControllerTestHelper.cs`

## Ejecución de pruebas
- Todas las pruebas:
  - `dotnet test`
- Solo pruebas unitarias:
  - `dotnet test tests/EBOS.CRM.ApiTests/EBOS.CRM.ApiTests.csproj`
- Solo pruebas de integración:
  - `dotnet test tests/EBOS.CRM.Api.IntegrationTests/EBOS.CRM.Api.IntegrationTests.csproj`

## Patrones de repositorio
- Los repositorios exponen lecturas de listas via `GetAllAsync` y devuelven `ICollection<T>`.

## CQRS/Manejadores
- Los manejadores viven en `EBOS.CRM.Application/Features/**`.
- Los manejadores de consulta que devuelven listas deben devolver `IReadOnlyCollection<T>`.

## Controladores API
- Los controladores residen en `EBOS.CRM.Api/Controllers/**`.
- Los puntos finales de listas deben devolver `IReadOnlyCollection<T>` con HTTP 200.

## Directrices de código
- Prefiere cambios pequeños y enfocados.
- Mantiene adiciones consistentes con los nombres y la estructura de carpetas existentes.
- Usa ASCII para contenidos de archivos a menos que el archivo ya contenga caracteres no ASCII.
