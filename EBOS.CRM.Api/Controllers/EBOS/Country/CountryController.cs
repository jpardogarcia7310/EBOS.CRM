using EBOS.CRM.Api.Constants;
using EBOS.CRM.Api.Options;
using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Contracts.Responses.EBOS;
using EBOS.CRM.Application.Features.EBOS.Countries.Queries.GetAllCountries;
using EBOS.CRM.Application.Features.EBOS.Countries.Queries.GetCountryById;
using MediatR;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Api.Controllers.EBOS.Country;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route(ApiRouteTemplates.Versioned)]
[Produces("application/json")]
public class CountryController(IMediator mediator) : ControllerBase
{
    #region Queries
    /// <summary>
    /// Gets a country by id.
    /// </summary>
    /// <param name="id">Country id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Country found.</response>
    /// <response code="400">Invalid id.</response>
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
            var details = ProblemDetailsFactory.CreateProblemDetails(
                HttpContext,
                StatusCodes.Status404NotFound,
                title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Country with id {id} not found.");
            return NotFound(details);
        }
        return Ok(dto);
    }
    /// <summary>
    /// Returns all resources (paginated).
    /// </summary>
    /// <param name="paginationOptions">Pagination settings.</param>
    /// <param name="pageNumber">1-based page number.</param>
    /// <param name="pageSize">Page size (must be &lt;= configured max).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">List of resources. Adds X-Total-Count header.</response>
    /// <response code="400">Invalid pageSize.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CountryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllAsync([FromServices] IOptions<PaginationOptions> paginationOptions, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var settings = paginationOptions.Value;
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = pageSize <= 0 ? settings.DefaultPageSize : pageSize;
        var result = await mediator.Send(new GetAllCountriesQuery(safePageNumber, safePageSize), cancellationToken);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result.Items);
    }
    #endregion
}






