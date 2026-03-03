using EBOS.CRM.Api.Constants;
using EBOS.CRM.Api.Options;
using EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Commands.ExecuteCustomerPrivacyRequest;
using EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Commands.RegisterCustomerPrivacyRequest;
using EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Queries.GetCustomerPrivacyRequestsByCustomer;
using EBOS.CRM.Contracts.Requests.CRM.CustomerPrivacy;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Api.Controllers.CRM.CustomerPrivacy;

[ApiController]
[ApiVersion("2.0")]
[Route(ApiRouteTemplates.Versioned)]
[Produces("application/json")]
[Authorize(Policy = "ApiUser")]
public class CustomerPrivacyController(IMediator mediator) : ControllerBase
{
    #region Queries
    [HttpGet("by-customer/{customerId:long}")]
    [Authorize(Policy = PolicyKeys.Crm.CustomerRead)]
    [ProducesResponseType(typeof(IReadOnlyCollection<CustomerPrivacyRequestResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCustomerAsync([FromServices] IOptions<PaginationOptions> paginationOptions,
        [FromRoute] long customerId, [FromQuery] long tenantId, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var settings = paginationOptions.Value;
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = pageSize <= 0 ? settings.DefaultPageSize : pageSize;
        var result = await mediator.Send(
            new GetCustomerPrivacyRequestsByCustomerQuery(tenantId, customerId, safePageNumber, safePageSize),
            cancellationToken);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result.Items);
    }
    #endregion

    #region Commands
    [HttpPost("register")]
    [Authorize(Policy = PolicyKeys.Crm.CustomerCreate)]
    [ProducesResponseType(typeof(CustomerPrivacyRequestResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterCustomerPrivacyRequestRequest request,
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new RegisterCustomerPrivacyRequestCommand(request), cancellationToken));
    }

    [HttpPost("{id:long}/execute")]
    [Authorize(Policy = PolicyKeys.Crm.CustomerPatch)]
    [ProducesResponseType(typeof(CustomerPrivacyRequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExecuteAsync([FromRoute] long id, [FromBody] ExecuteCustomerPrivacyRequestRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(new ExecuteCustomerPrivacyRequestCommand(id, request), cancellationToken);
        if (response is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext, statusCode: StatusCodes.Status404NotFound,
                title: ProblemDetailsDefaults.NotFoundTitle, detail: $"CustomerPrivacyRequest with id {id} not found."));
        }

        return Ok(response);
    }
    #endregion
}
