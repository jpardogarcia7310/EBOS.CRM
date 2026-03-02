using EBOS.CRM.Api.Constants;
using EBOS.CRM.Api.Options;
using EBOS.CRM.Application.Features.CRM.AccountHierarchy.Commands.AddAccountHierarchy;
using EBOS.CRM.Application.Features.CRM.AccountHierarchy.Commands.EndAccountHierarchy;
using EBOS.CRM.Application.Features.CRM.AccountHierarchy.Queries.GetAccountHierarchyByAccount;
using EBOS.CRM.Application.Features.CRM.AccountHierarchy.Queries.GetAccountHierarchyById;
using EBOS.CRM.Contracts.Requests.CRM.AccountHierarchy;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Api.Controllers.CRM.AccountHierarchy;

[ApiController]
[ApiVersion("2.0")]
[Route(ApiRouteTemplates.Versioned)]
[Produces("application/json")]
public class AccountHierarchyController(IMediator mediator) : ControllerBase
{
    #region Queries
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(AccountHierarchyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new GetAccountHierarchyByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext, statusCode: StatusCodes.Status404NotFound,
                title: ProblemDetailsDefaults.NotFoundTitle, detail: $"AccountHierarchy with id {id} not found."));
        }

        return Ok(dto);
    }

    [HttpGet("by-account/{corporateCustomerId:long}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AccountHierarchyResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByAccountAsync([FromServices] IOptions<PaginationOptions> paginationOptions,
        [FromRoute] long corporateCustomerId, [FromQuery] long tenantId, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var settings = paginationOptions.Value;
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = pageSize <= 0 ? settings.DefaultPageSize : pageSize;
        var result = await mediator.Send(
            new GetAccountHierarchyByAccountQuery(tenantId, corporateCustomerId, safePageNumber, safePageSize),
            cancellationToken);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result.Items);
    }
    #endregion

    #region Commands
    [HttpPost]
    [ProducesResponseType(typeof(AccountHierarchyResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddAsync([FromBody] AddAccountHierarchyRequest request,
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new AddAccountHierarchyCommand(request), cancellationToken));
    }

    [HttpPatch("{id:long}/end")]
    [ProducesResponseType(typeof(AccountHierarchyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EndAsync([FromRoute] long id, [FromBody] EndAccountHierarchyRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new EndAccountHierarchyCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext, statusCode: StatusCodes.Status404NotFound,
                title: ProblemDetailsDefaults.NotFoundTitle, detail: $"AccountHierarchy with id {id} not found."));
        }

        return Ok(dto);
    }
    #endregion
}
