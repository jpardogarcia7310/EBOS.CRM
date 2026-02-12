using EBOS.CRM.Api.Constants;
using EBOS.CRM.Api.Options;
using EBOS.CRM.Application.Features.CRM.Service.Sla.Commands.AddSla;
using EBOS.CRM.Application.Features.CRM.Service.Sla.Commands.ToggleSla;
using EBOS.CRM.Application.Features.CRM.Service.Sla.Commands.UpdateSla;
using EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.CheckCaseSla;
using EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.GetAllSlas;
using EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.GetSlaById;
using EBOS.CRM.Contracts.Requests.CRM.Service.Sla;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Api.Controllers.CRM.Service.Sla;

[ApiController]
[ApiVersion("2.0")]
[Route(ApiRouteTemplates.Versioned)]
[Produces("application/json")]
public class SlaController(IMediator mediator) : ControllerBase
{
    #region Commands
    [Authorize(Policy = PolicyKeys.Crm.SlaCreate)]
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(SlaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAsync([FromBody] AddSlaRequest request,
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new AddSlaCommand(request), cancellationToken));
    }

    [Authorize(Policy = PolicyKeys.Crm.SlaUpdate)]
    [HttpPut("{id:long}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(SlaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync([FromRoute] long id, [FromBody] UpdateSlaRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new UpdateSlaCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"SLA with id {id} not found."));
        }

        return Ok(dto);
    }

    [Authorize(Policy = PolicyKeys.Crm.SlaPatch)]
    [HttpPatch("{id:long}/toggle")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(SlaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleAsync([FromRoute] long id, [FromBody] ToggleSlaRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new ToggleSlaCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"SLA with id {id} not found."));
        }

        return Ok(dto);
    }
    #endregion

    #region Queries
    [Authorize(Policy = PolicyKeys.Crm.SlaRead)]
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(SlaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(new GetSlaByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"SLA with id {id} not found."));
        }

        return Ok(dto);
    }

    [Authorize(Policy = PolicyKeys.Crm.SlaRead)]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<SlaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllAsync([FromServices] IOptions<PaginationOptions> paginationOptions,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var settings = paginationOptions.Value;
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = pageSize <= 0 ? settings.DefaultPageSize : pageSize;
        var result = await mediator.Send(new GetAllSlasQuery(safePageNumber, safePageSize), cancellationToken);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result.Items);
    }

    [Authorize(Policy = PolicyKeys.Crm.SlaRead)]
    [HttpGet("{id:long}/check")]
    [ProducesResponseType(typeof(SlaCheckResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CheckAsync([FromRoute] long id, [FromQuery] long tenantId,
        [FromQuery] long caseId, [FromQuery] DateTime? now, CancellationToken cancellationToken = default)
    {
        var request = new CheckCaseSlaRequest(tenantId, caseId, now ?? DateTime.UtcNow);
        var dto = await mediator.Send(new CheckCaseSlaQuery(request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Case with id {caseId} or SLA not found."));
        }

        if (dto.SlaId != id)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"SLA with id {id} not found for case {caseId}."));
        }

        return Ok(dto);
    }
    #endregion
}
