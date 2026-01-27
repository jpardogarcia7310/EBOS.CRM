using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Features.Statuses.Queries.GetAllStatuses;
using EBOS.CRM.Application.Features.Statuses.Queries.GetStatusById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EBOS.CRM.Api.Controllers.Status;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class StatusController(IMediator mediator) : ControllerBase
{
    #region Queries
    [HttpGet("{id:long}")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
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

    [HttpGet]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(typeof(ICollection<StatusResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetAllStatusesQuery(), cancellationToken));
    }
    #endregion
}
