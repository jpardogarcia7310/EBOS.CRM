using EBOS.CRM.Api.Constants;
using EBOS.CRM.Api.Options;
using EBOS.CRM.Application.Features.CRM.AccountContactRole.Commands.AddAccountContactRole;
using EBOS.CRM.Application.Features.CRM.AccountContactRole.Commands.DeleteAccountContactRole;
using EBOS.CRM.Application.Features.CRM.AccountContactRole.Commands.UpdateAccountContactRole;
using EBOS.CRM.Application.Features.CRM.AccountContactRole.Queries.GetAccountContactRoleById;
using EBOS.CRM.Application.Features.CRM.AccountContactRole.Queries.GetAccountContactRolesByAccountContact;
using EBOS.CRM.Contracts.Requests.CRM.AccountContactRole;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Api.Controllers.CRM.AccountContactRole;

[ApiController]
[ApiVersion("2.0")]
[Route(ApiRouteTemplates.Versioned)]
[Produces("application/json")]
[Authorize(Policy = "ApiUser")]
public class AccountContactRoleController(IMediator mediator) : ControllerBase
{
    #region Queries
    [HttpGet("{id:long}")]
    [Authorize(Policy = PolicyKeys.Crm.CustomerRead)]
    [ProducesResponseType(typeof(AccountContactRoleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new GetAccountContactRoleByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext, statusCode: StatusCodes.Status404NotFound,
                title: ProblemDetailsDefaults.NotFoundTitle, detail: $"AccountContactRole with id {id} not found."));
        }

        return Ok(dto);
    }

    [HttpGet("by-account-contact/{accountContactId:long}")]
    [Authorize(Policy = PolicyKeys.Crm.CustomerRead)]
    [ProducesResponseType(typeof(IReadOnlyCollection<AccountContactRoleResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByAccountContactAsync([FromServices] IOptions<PaginationOptions> paginationOptions,
        [FromRoute] long accountContactId, [FromQuery] long tenantId, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var settings = paginationOptions.Value;
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = pageSize <= 0 ? settings.DefaultPageSize : pageSize;
        var result = await mediator.Send(
            new GetAccountContactRolesByAccountContactQuery(tenantId, accountContactId, safePageNumber, safePageSize),
            cancellationToken);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result.Items);
    }
    #endregion

    #region Commands
    [HttpPost]
    [Authorize(Policy = PolicyKeys.Crm.CustomerCreate)]
    [ProducesResponseType(typeof(AccountContactRoleResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddAsync([FromBody] AddAccountContactRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new AddAccountContactRoleCommand(request), cancellationToken));
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = PolicyKeys.Crm.CustomerUpdate)]
    [ProducesResponseType(typeof(AccountContactRoleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync([FromRoute] long id, [FromBody] UpdateAccountContactRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new UpdateAccountContactRoleCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext, statusCode: StatusCodes.Status404NotFound,
                title: ProblemDetailsDefaults.NotFoundTitle, detail: $"AccountContactRole with id {id} not found."));
        }

        return Ok(dto);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = PolicyKeys.Crm.CustomerDelete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync([FromRoute] long id, [FromQuery] long tenantId,
        CancellationToken cancellationToken = default)
    {
        var deleted = await mediator.Send(
            new DeleteAccountContactRoleCommand(id, new DeleteAccountContactRoleRequest(tenantId)), cancellationToken);
        if (!deleted)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext, statusCode: StatusCodes.Status404NotFound,
                title: ProblemDetailsDefaults.NotFoundTitle, detail: $"AccountContactRole with id {id} not found."));
        }

        return Ok();
    }
    #endregion
}
