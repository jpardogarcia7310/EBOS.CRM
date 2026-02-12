using EBOS.CRM.Api.Constants;
using EBOS.CRM.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AddCase;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AssignCaseOwner;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AssignCaseQueue;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AssignCaseSla;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.CloseCase;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.DeleteCase;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.ReopenCase;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.UpdateCase;
using EBOS.CRM.Application.Features.CRM.Service.Case.Queries.GetAllCases;
using EBOS.CRM.Application.Features.CRM.Service.Case.Queries.GetCaseById;
using EBOS.CRM.Api.Options;
using EBOS.CRM.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Api.Controllers.CRM.Service.Case;

[ApiController]
[ApiVersion("2.0")]
[Route(ApiRouteTemplates.Versioned)]
[Produces("application/json")]
public class CaseController(IMediator mediator) : ControllerBase
{
    #region Commands
    [Authorize(Policy = PolicyKeys.Crm.CaseCreate)]
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAsync([FromBody] AddCaseRequest request,
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new AddCaseCommand(request), cancellationToken));
    }

    [Authorize(Policy = PolicyKeys.Crm.CaseUpdate)]
    [HttpPut("{id:long}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync([FromRoute] long id, [FromBody] UpdateCaseRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new UpdateCaseCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Case with id {id} not found."));
        }

        return Ok(dto);
    }

    [Authorize(Policy = PolicyKeys.Crm.CaseDelete)]
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync([FromRoute] long id, CancellationToken cancellationToken = default)
    {
        var deleted = await mediator.Send(new DeleteCaseCommand(id), cancellationToken);
        if (!deleted)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Case with id {id} not found."));
        }

        return Ok();
    }

    [Authorize(Policy = PolicyKeys.Crm.CasePatch)]
    [HttpPatch("{id:long}/close")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CloseAsync([FromRoute] long id, [FromBody] CloseCaseRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new CloseCaseCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Case with id {id} not found."));
        }

        return Ok(dto);
    }

    [Authorize(Policy = PolicyKeys.Crm.CasePatch)]
    [HttpPatch("{id:long}/reopen")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReopenAsync([FromRoute] long id, [FromBody] ReopenCaseRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new ReopenCaseCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Case with id {id} not found."));
        }

        return Ok(dto);
    }

    [Authorize(Policy = PolicyKeys.Crm.CasePatch)]
    [HttpPatch("{id:long}/queue")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignQueueAsync([FromRoute] long id, [FromBody] AssignCaseQueueRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new AssignCaseQueueCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Case with id {id} not found."));
        }

        return Ok(dto);
    }

    [Authorize(Policy = PolicyKeys.Crm.CasePatch)]
    [HttpPatch("{id:long}/owner")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignOwnerAsync([FromRoute] long id, [FromBody] AssignCaseOwnerRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new AssignCaseOwnerCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Case with id {id} not found."));
        }

        return Ok(dto);
    }

    [Authorize(Policy = PolicyKeys.Crm.CasePatch)]
    [HttpPatch("{id:long}/sla")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignSlaAsync([FromRoute] long id,
        [FromBody] AssignCaseSlaRequest request, CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new AssignCaseSlaCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Case with id {id} not found."));
        }

        return Ok(dto);
    }
    #endregion

    #region Queries
    [Authorize(Policy = PolicyKeys.Crm.CaseRead)]
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(CaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(new GetCaseByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Case with id {id} not found."));
        }

        return Ok(dto);
    }

    [Authorize(Policy = PolicyKeys.Crm.CaseRead)]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CaseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllAsync([FromServices] IOptions<PaginationOptions> paginationOptions,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var settings = paginationOptions.Value;
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = pageSize <= 0 ? settings.DefaultPageSize : pageSize;
        var result = await mediator.Send(new GetAllCasesQuery(safePageNumber, safePageSize), cancellationToken);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result.Items);
    }
    #endregion

    
}
