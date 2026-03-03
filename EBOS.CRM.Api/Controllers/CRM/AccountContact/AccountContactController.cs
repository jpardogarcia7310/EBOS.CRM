using EBOS.CRM.Api.Constants;
using EBOS.CRM.Api.Options;
using EBOS.CRM.Application.Features.CRM.AccountContact.Commands.AddAccountContact;
using EBOS.CRM.Application.Features.CRM.AccountContact.Commands.DeleteAccountContact;
using EBOS.CRM.Application.Features.CRM.AccountContact.Commands.SetPrimaryAccountContact;
using EBOS.CRM.Application.Features.CRM.AccountContact.Commands.UpdateAccountContact;
using EBOS.CRM.Application.Features.CRM.AccountContact.Queries.GetAccountContactById;
using EBOS.CRM.Application.Features.CRM.AccountContact.Queries.GetAccountContactsByAccount;
using EBOS.CRM.Application.Features.CRM.AccountContact.Queries.GetAllAccountContacts;
using EBOS.CRM.Contracts.Requests.CRM.AccountContact;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Api.Controllers.CRM.AccountContact;

[ApiController]
[ApiVersion("2.0")]
[Route(ApiRouteTemplates.Versioned)]
[Produces("application/json")]
[Authorize(Policy = "ApiUser")]
public class AccountContactController(IMediator mediator) : ControllerBase
{
    #region Queries
    [HttpGet]
    [Authorize(Policy = PolicyKeys.Crm.CustomerRead)]
    [ProducesResponseType(typeof(IReadOnlyCollection<AccountContactResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync([FromServices] IOptions<PaginationOptions> paginationOptions,
        [FromQuery] long tenantId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var settings = paginationOptions.Value;
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = pageSize <= 0 ? settings.DefaultPageSize : pageSize;
        var result = await mediator.Send(new GetAllAccountContactsQuery(tenantId, safePageNumber, safePageSize), cancellationToken);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result.Items);
    }

    [HttpGet("{id:long}")]
    [Authorize(Policy = PolicyKeys.Crm.CustomerRead)]
    [ProducesResponseType(typeof(AccountContactResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new GetAccountContactByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext, statusCode: StatusCodes.Status404NotFound,
                title: ProblemDetailsDefaults.NotFoundTitle, detail: $"AccountContact with id {id} not found."));
        }

        return Ok(dto);
    }

    [HttpGet("by-account/{corporateCustomerId:long}")]
    [Authorize(Policy = PolicyKeys.Crm.CustomerRead)]
    [ProducesResponseType(typeof(IReadOnlyCollection<AccountContactResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByAccountAsync([FromServices] IOptions<PaginationOptions> paginationOptions,
        [FromRoute] long corporateCustomerId, [FromQuery] long tenantId, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var settings = paginationOptions.Value;
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = pageSize <= 0 ? settings.DefaultPageSize : pageSize;
        var result = await mediator.Send(
            new GetAccountContactsByAccountQuery(tenantId, corporateCustomerId, safePageNumber, safePageSize),
            cancellationToken);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result.Items);
    }
    #endregion

    #region Commands
    [HttpPost]
    [Authorize(Policy = PolicyKeys.Crm.CustomerCreate)]
    [ProducesResponseType(typeof(AccountContactResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddAsync([FromBody] AddAccountContactRequest request,
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new AddAccountContactCommand(request), cancellationToken));
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = PolicyKeys.Crm.CustomerUpdate)]
    [ProducesResponseType(typeof(AccountContactResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync([FromRoute] long id, [FromBody] UpdateAccountContactRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new UpdateAccountContactCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext, statusCode: StatusCodes.Status404NotFound,
                title: ProblemDetailsDefaults.NotFoundTitle, detail: $"AccountContact with id {id} not found."));
        }

        return Ok(dto);
    }

    [HttpPatch("{id:long}/primary")]
    [Authorize(Policy = PolicyKeys.Crm.CustomerPatch)]
    [ProducesResponseType(typeof(AccountContactResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetPrimaryAsync([FromRoute] long id, [FromBody] SetPrimaryAccountContactRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new SetPrimaryAccountContactCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext, statusCode: StatusCodes.Status404NotFound,
                title: ProblemDetailsDefaults.NotFoundTitle, detail: $"AccountContact with id {id} not found."));
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
        var deleted = await mediator.Send(new DeleteAccountContactCommand(id, new DeleteAccountContactRequest(tenantId)),
            cancellationToken);
        if (!deleted)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext, statusCode: StatusCodes.Status404NotFound,
                title: ProblemDetailsDefaults.NotFoundTitle, detail: $"AccountContact with id {id} not found."));
        }

        return Ok();
    }
    #endregion
}
