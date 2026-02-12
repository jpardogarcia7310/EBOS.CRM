using EBOS.CRM.Api.Constants;
using EBOS.CRM.Api.Options;
using EBOS.CRM.Application.Features.CRM.Service.Queue.Commands.AddQueue;
using EBOS.CRM.Application.Features.CRM.Service.Queue.Commands.AssignQueueDefaultOwner;
using EBOS.CRM.Application.Features.CRM.Service.Queue.Commands.ToggleQueue;
using EBOS.CRM.Application.Features.CRM.Service.Queue.Commands.UpdateQueue;
using EBOS.CRM.Application.Features.CRM.Service.Queue.Queries.GetAllQueues;
using EBOS.CRM.Application.Features.CRM.Service.Queue.Queries.GetQueueById;
using EBOS.CRM.Contracts.Requests.CRM.Service.Queue;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Api.Controllers.CRM.Service.Queue;

[ApiController]
[ApiVersion("2.0")]
[Route(ApiRouteTemplates.Versioned)]
[Produces("application/json")]
public class QueueController(IMediator mediator) : ControllerBase
{
    #region Commands
    [Authorize(Policy = PolicyKeys.Crm.QueueCreate)]
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(QueueResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAsync([FromBody] AddQueueRequest request,
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new AddQueueCommand(request), cancellationToken));
    }

    [Authorize(Policy = PolicyKeys.Crm.QueueUpdate)]
    [HttpPut("{id:long}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(QueueResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync([FromRoute] long id, [FromBody] UpdateQueueRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new UpdateQueueCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Queue with id {id} not found."));
        }

        return Ok(dto);
    }

    [Authorize(Policy = PolicyKeys.Crm.QueuePatch)]
    [HttpPatch("{id:long}/toggle")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(QueueResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleAsync([FromRoute] long id, [FromBody] ToggleQueueRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new ToggleQueueCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Queue with id {id} not found."));
        }

        return Ok(dto);
    }

    [Authorize(Policy = PolicyKeys.Crm.QueuePatch)]
    [HttpPatch("{id:long}/default-owner")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(QueueResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignDefaultOwnerAsync([FromRoute] long id,
        [FromBody] AssignQueueDefaultOwnerRequest request, CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new AssignQueueDefaultOwnerCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Queue with id {id} not found."));
        }

        return Ok(dto);
    }
    #endregion

    #region Queries
    [Authorize(Policy = PolicyKeys.Crm.QueueRead)]
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(QueueResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(new GetQueueByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Queue with id {id} not found."));
        }

        return Ok(dto);
    }

    [Authorize(Policy = PolicyKeys.Crm.QueueRead)]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<QueueResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllAsync([FromServices] IOptions<PaginationOptions> paginationOptions,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var settings = paginationOptions.Value;
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = pageSize <= 0 ? settings.DefaultPageSize : pageSize;
        var result = await mediator.Send(new GetAllQueuesQuery(safePageNumber, safePageSize), cancellationToken);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result.Items);
    }
    #endregion
}
