using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Features.IdentificationType.Query.GetAllIdentificationType;
using EBOS.CRM.Application.Features.IdentificationType.Query.GetIdentificationTypeByIdQuery;
using MediatR;
using EBOS.CRM.Api.Options;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Localization;
using EBOS.CRM.Api.Resources;

namespace EBOS.CRM.Api.Controllers.IdentificationType;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class IdentificationTypeController(IMediator mediator, IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    #region Queries
    /// <summary>
    /// Gets an identification type by id.
    /// </summary>
    /// <param name="id">Identification type id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Identification type found.</response>
    /// <response code="400">Invalid id.</response>
    /// <response code="404">Identification type not found.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(IdentificationTypeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(new GetIdentificationTypeByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"IdentificationType with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
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
    [ProducesResponseType(typeof(IReadOnlyCollection<IdentificationTypeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllAsync([FromServices] IOptions<PaginationOptions> paginationOptions, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var settings = paginationOptions.Value;
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = pageSize <= 0 ? settings.DefaultPageSize : pageSize;
        if (safePageSize > settings.MaxPageSize)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid pageSize",
                Detail = localizer["InvalidPageSize", settings.MaxPageSize],
                Status = StatusCodes.Status400BadRequest
            });
        }

        var result = await mediator.Send(new GetAllIdentificationTypeQuery(safePageNumber, safePageSize), cancellationToken);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result.Items);
    }

    #endregion
}






















