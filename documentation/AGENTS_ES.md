# AGENTS

## Alcance
Este repositorio contiene la solucion EBOS.CRM (API, Application, Domain, Infrastructure) mas pruebas unitarias e integracion.
Use las instrucciones de abajo al hacer cambios en esta solucion.

## Mapa rapido de arquitectura
- API: `EBOS.CRM.Api`
- Application (CQRS/Handlers, contratos): `EBOS.CRM.Application`
- Domain (entidades, interfaces): `EBOS.CRM.Domain`
- Infrastructure (EF Core, repositorios): `EBOS.CRM.Infrastructure`
- Pruebas:
  - Pruebas unitarias: `tests/EBOS.CRM.ApiTests`
  - Pruebas de integracion: `tests/EBOS.CRM.Api.IntegrationTests`

## Convenciones de pruebas
- Los endpoints de listas devuelven una **lista sin contenedor**:
  - DTO: `IReadOnlyCollection<T>`
  - Las pruebas deben deserializar endpoints de listas como `IReadOnlyCollection<T>`.
  - Helpers:
    - Pruebas unitarias/de integracion: `ReadItemsAsync<T>()` en `tests/**/TestUtils/HttpContentExtensions.cs`
    - Helper de controladores: `GetFirstIdAsync<T>()` en `tests/EBOS.CRM.ApiTests/TestUtils/ControllerTestHelper.cs`

## Ejecucion de pruebas
- Todas las pruebas:
  - `dotnet test`
- Solo pruebas unitarias:
  - `dotnet test tests/EBOS.CRM.ApiTests/EBOS.CRM.ApiTests.csproj`
- Solo pruebas de integracion:
  - `dotnet test tests/EBOS.CRM.Api.IntegrationTests/EBOS.CRM.Api.IntegrationTests.csproj`

## Patrones de repositorio
- Los repositorios exponen lecturas de listas via `GetAllAsync` y devuelven `ICollection<T>`.

## CQRS/Handlers
- Los handlers viven en `EBOS.CRM.Application/Features/**`.
- Los query handlers que devuelven listas deben devolver `IReadOnlyCollection<T>`.

## Controladores API
- Los controladores residen en `EBOS.CRM.Api/Controllers/**`.
- Los endpoints de listas deben devolver `IReadOnlyCollection<T>` con HTTP 200.

## Directrices de codigo
- Prefiere cambios pequenos y enfocados.
- Mantiene adiciones consistentes con los nombres y la estructura de carpetas existentes.
- Usa ASCII para contenidos de archivos a menos que el archivo ya contenga caracteres no ASCII.
