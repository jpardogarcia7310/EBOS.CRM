using EBOS.CRM.Api.Constants;
using EBOS.CRM.Api.Options;
using EBOS.CRM.Application.Contracts.Requests.CRM.Service.CaseActivity;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Commands.AddCaseActivity;
using EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Commands.DeleteCaseActivity;
using EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Commands.UpdateCaseActivity;
using EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Queries.GetAllCaseActivities;
using EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Queries.GetCaseActivityById;
using EBOS.CRM.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Api.Controllers.CRM.Service.CaseActivity;

[ApiController]
[ApiVersion("2.0")]
[Route(ApiRouteTemplates.Versioned)]
[Produces("application/json")]
public class CaseActivityController(IMediator mediator) : ControllerBase
{
    #region Commands
    [Authorize(Policy = PolicyKeys.Crm.CaseActivityCreate)]
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CaseActivityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAsync([FromBody] AddCaseActivityRequest request,
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new AddCaseActivityCommand(request), cancellationToken));
    }

    [Authorize(Policy = PolicyKeys.Crm.CaseActivityUpdate)]
    [HttpPut("{id:long}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CaseActivityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync([FromRoute] long id, [FromBody] UpdateCaseActivityRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new UpdateCaseActivityCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"CaseActivity with id {id} not found."));
        }

        return Ok(dto);
    }

    [Authorize(Policy = PolicyKeys.Crm.CaseActivityDelete)]
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync([FromRoute] long id, CancellationToken cancellationToken = default)
    {
        var deleted = await mediator.Send(new DeleteCaseActivityCommand(id), cancellationToken);
        if (!deleted)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"CaseActivity with id {id} not found."));
        }

        return Ok();
    }
    #endregion

    #region Queries
    [Authorize(Policy = PolicyKeys.Crm.CaseActivityRead)]
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(CaseActivityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(new GetCaseActivityByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"CaseActivity with id {id} not found."));
        }

        return Ok(dto);
    }

    [Authorize(Policy = PolicyKeys.Crm.CaseActivityRead)]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CaseActivityResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllAsync([FromServices] IOptions<PaginationOptions> paginationOptions,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var settings = paginationOptions.Value;
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = pageSize <= 0 ? settings.DefaultPageSize : pageSize;
        var result = await mediator.Send(new GetAllCaseActivitiesQuery(safePageNumber, safePageSize), cancellationToken);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result.Items);
    }
    #endregion
}
