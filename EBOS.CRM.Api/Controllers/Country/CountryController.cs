using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Features.Countries.Queries.GetAllCountries;
using EBOS.CRM.Application.Features.Countries.Queries.GetCountryById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EBOS.CRM.Api.Controllers.Country;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class CountryController(IMediator mediator) : ControllerBase
{
    #region Queries
    /// <summary>
    /// Returns a country by its identifier.
    /// </summary>
    /// <example>
    /// GET /api/v1/Country/1
    /// </example>
    /// <response code="200">Country found.</response>
    /// <response code="404">Country not found.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(CountryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken)
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
    /// Returns all countries.
    /// </summary>
    /// <example>
    /// GET /api/v1/Country
    /// </example>
    /// <response code="200">List of countries.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ICollection<CountryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetAllCountriesQuery(), cancellationToken));
    }
    #endregion
}
