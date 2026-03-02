using EBOS.CRM.Api.Constants;
using EBOS.CRM.Api.Options;
using EBOS.CRM.Application.Features.CRM.CustomerPreference.Commands.UpsertCustomerPreference;
using EBOS.CRM.Application.Features.CRM.CustomerPreference.Queries.GetCustomerPreferencesByCustomer;
using EBOS.CRM.Contracts.Requests.CRM.CustomerPreference;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Api.Controllers.CRM.CustomerPreference;

[ApiController]
[ApiVersion("2.0")]
[Route(ApiRouteTemplates.Versioned)]
[Produces("application/json")]
public class CustomerPreferenceController(IMediator mediator) : ControllerBase
{
    #region Queries
    [HttpGet("by-customer/{customerId:long}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<CustomerPreferenceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCustomerAsync([FromServices] IOptions<PaginationOptions> paginationOptions,
        [FromRoute] long customerId, [FromQuery] long tenantId, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var settings = paginationOptions.Value;
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = pageSize <= 0 ? settings.DefaultPageSize : pageSize;
        var result = await mediator.Send(
            new GetCustomerPreferencesByCustomerQuery(tenantId, customerId, safePageNumber, safePageSize),
            cancellationToken);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result.Items);
    }
    #endregion

    #region Commands
    [HttpPut]
    [ProducesResponseType(typeof(CustomerPreferenceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpsertAsync([FromBody] UpsertCustomerPreferenceRequest request,
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new UpsertCustomerPreferenceCommand(request), cancellationToken));
    }
    #endregion
}
