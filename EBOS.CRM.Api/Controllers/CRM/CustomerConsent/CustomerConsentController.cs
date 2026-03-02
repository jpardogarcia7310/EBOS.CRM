using EBOS.CRM.Api.Constants;
using EBOS.CRM.Api.Options;
using EBOS.CRM.Application.Features.CRM.CustomerConsent.Commands.AddCustomerConsent;
using EBOS.CRM.Application.Features.CRM.CustomerConsent.Commands.RevokeCustomerConsent;
using EBOS.CRM.Application.Features.CRM.CustomerConsent.Queries.GetCustomerConsentsByCustomer;
using EBOS.CRM.Contracts.Requests.CRM.CustomerConsent;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Api.Controllers.CRM.CustomerConsent;

[ApiController]
[ApiVersion("2.0")]
[Route(ApiRouteTemplates.Versioned)]
[Produces("application/json")]
public class CustomerConsentController(IMediator mediator) : ControllerBase
{
    #region Queries
    [HttpGet("by-customer/{customerId:long}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<CustomerConsentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCustomerAsync([FromServices] IOptions<PaginationOptions> paginationOptions,
        [FromRoute] long customerId, [FromQuery] long tenantId, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var settings = paginationOptions.Value;
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = pageSize <= 0 ? settings.DefaultPageSize : pageSize;
        var result = await mediator.Send(new GetCustomerConsentsByCustomerQuery(tenantId, customerId, safePageNumber, safePageSize),
            cancellationToken);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result.Items);
    }
    #endregion

    #region Commands
    [HttpPost]
    [ProducesResponseType(typeof(CustomerConsentResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddAsync([FromBody] AddCustomerConsentRequest request,
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new AddCustomerConsentCommand(request), cancellationToken));
    }

    [HttpPatch("{id:long}/revoke")]
    [ProducesResponseType(typeof(CustomerConsentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeAsync([FromRoute] long id, [FromBody] RevokeCustomerConsentRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new RevokeCustomerConsentCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext, statusCode: StatusCodes.Status404NotFound,
                title: ProblemDetailsDefaults.NotFoundTitle, detail: $"CustomerConsent with id {id} not found."));
        }

        return Ok(dto);
    }
    #endregion
}
