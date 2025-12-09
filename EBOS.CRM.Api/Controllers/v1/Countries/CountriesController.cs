using EBOS.CRM.Api.Controllers.v1.Countries.Requests;
using EBOS.CRM.Application.Features.Countries.Commands.AddCountry;
using EBOS.CRM.Application.Features.Countries.Dtos;
using EBOS.CRM.Application.Features.Countries.Queries.GetAllCountries;
using EBOS.CRM.Application.Features.Countries.Queries.GetCountryById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EBOS.CRM.Api.Controllers.v1.Countries;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class CountriesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Crea un nuevo Country.
    /// </summary>
    /// <remarks>
    /// Ejemplo de petición:
    /// 
    ///     POST /api/countries
    ///     {
    ///       "name": "España",
    ///       "iso31661A2Code": "ES",
    ///       "iso31661A3Code": "ESP",
    ///       "iso31661NumCode": "724",
    ///       "domain": "es",
    ///       "currency": "Euro",
    ///       "currencyCode": "EUR",
    ///       "internationalPhoneCode": "+34"
    ///     }
    /// 
    /// Ejemplo de respuesta 201 Created:
    /// 
    ///     {
    ///       "id": 1,
    ///       "name": "España",
    ///       "iso31661A2Code": "ES",
    ///       "iso31661A3Code": "ESP",
    ///       "iso31661NumCode": "724",
    ///       "domain": "es",
    ///       "currency": "Euro",
    ///       "currencyCode": "EUR",
    ///       "internationalPhoneCode": "+34"
    ///     }
    /// 
    /// Ejemplo de respuesta 400 ValidationProblemDetails:
    /// 
    ///     {
    ///       "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    ///       "title": "One or more validation errors occurred.",
    ///       "status": 400,
    ///       "errors": {
    ///         "Name": [ "El campo Name es obligatorio." ],
    ///         "Iso31661A2Code": [ "El campo Iso31661A2Code debe tener 2 caracteres." ]
    ///       }
    ///     }
    /// 
    /// Ejemplo de respuesta 500 ProblemDetails:
    /// 
    ///     {
    ///       "title": "An unexpected error occurred.",
    ///       "detail": "Descripción del error interno.",
    ///       "status": 500
    ///     }
    /// </remarks>
    /// <response code="201">Country creado correctamente. Devuelve el CountryDto con Id.</response>
    /// <response code="400">Error de validación. Devuelve ValidationProblemDetails.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpPost]
    [ProducesResponseType(typeof(CountryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Add([FromBody] AddCountryRQ request, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(
            new AddCountryCommand(
                request.Name, request.Iso31661A2Code, request.Iso31661A3Code,
                request.Iso31661NumCode, request.Domain, request.Currency,
                request.CurrencyCode, request.InternationalPhoneCode), 
            cancellationToken
        ));
    }

    /// <summary>
    /// Obtiene un Country por su Id.
    /// </summary>
    /// <remarks>
    /// Ejemplo de respuesta 200 OK:
    /// 
    ///     {
    ///       "id": 1,
    ///       "name": "España",
    ///       "iso31661A2Code": "ES",
    ///       "iso31661A3Code": "ESP",
    ///       "iso31661NumCode": "724",
    ///       "domain": "es",
    ///       "currency": "Euro",
    ///       "currencyCode": "EUR",
    ///       "internationalPhoneCode": "+34"
    ///     }
    /// 
    /// Ejemplo de respuesta 404 NotFound:
    /// 
    ///     {
    ///       "title": "Resource not found",
    ///       "detail": "Country with id 999 not found.",
    ///       "status": 404
    ///     }
    /// 
    /// Ejemplo de respuesta 400 ValidationProblemDetails (Id inválido):
    /// 
    ///     {
    ///       "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    ///       "title": "One or more validation errors occurred.",
    ///       "status": 400,
    ///       "errors": {
    ///         "Id": [ "El identificador debe ser un número entero positivo mayor que 0." ]
    ///       }
    ///     }
    /// </remarks>
    /// <response code="200">Devuelve el CountryDto.</response>
    /// <response code="400">Error de validación del Id.</response>
    /// <response code="404">No se encuentra el Country.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(CountryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] long id, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(new GetCountryByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"Country with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(dto);
    }

    /// <summary>
    /// Lista todos los Countries.
    /// </summary>
    /// <remarks>
    /// Ejemplo de respuesta 200 OK:
    /// 
    ///     [
    ///       {
    ///         "id": 1,
    ///         "name": "España",
    ///         "iso31661A2Code": "ES",
    ///         "iso31661A3Code": "ESP",
    ///         "iso31661NumCode": "724",
    ///         "domain": "es",
    ///         "currency": "Euro",
    ///         "currencyCode": "EUR",
    ///         "internationalPhoneCode": "+34"
    ///       }
    ///     ]
    /// </remarks>
    /// <response code="200">Devuelve la lista de CountryDto.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ICollection<CountryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetAllCountriesQuery(), cancellationToken));
    }
}