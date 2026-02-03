using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Features.Statuses.Queries.GetAllStatuses;
using EBOS.CRM.Application.Features.Statuses.Queries.GetStatusById;
using MediatR;

namespace EBOS.CRM.Api.Controllers.Status;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class StatusController(IMediator mediator) : ControllerBase
{
    #region Queries
    /// <summary>
    /// Returns a status by its identifier.
    /// </summary>
    /// <example>
    /// GET /api/v1/Status/1
    /// </example>
    /// <response code="200">Status found.</response>
    /// <response code="404">Status not found.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(StatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(new GetStatusByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"Status with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(dto);
    }

    /// <summary>
    /// Returns all statuses.
    /// </summary>
    /// <example>
    /// GET /api/v1/Status
    /// </example>
    /// <response code="200">List of statuses.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<StatusResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetAllStatusesQuery(), cancellationToken));
    }
    #endregion
}






