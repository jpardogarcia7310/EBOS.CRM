using EBOS.CRM.Api.Constants;
using EBOS.CRM.Api.Options;
using EBOS.CRM.Application.Features.CRM.CustomerMerge.Commands.MergeCustomers;
using EBOS.CRM.Application.Features.CRM.CustomerMerge.Queries.FindCustomerDuplicates;
using EBOS.CRM.Application.Features.CRM.CustomerMerge.Queries.GetCustomerMergeHistoryByMerged;
using EBOS.CRM.Application.Features.CRM.CustomerMerge.Queries.GetCustomerMergeHistoryByWinner;
using EBOS.CRM.Contracts.Requests.CRM.CustomerMerge;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Api.Controllers.CRM.CustomerMerge;

[ApiController]
[ApiVersion("2.0")]
[Route(ApiRouteTemplates.Versioned)]
[Produces("application/json")]
[Authorize(Policy = "ApiUser")]
public class CustomerMergeController(IMediator mediator) : ControllerBase
{
    #region Queries
    [HttpGet("duplicates")]
    [Authorize(Policy = PolicyKeys.Crm.CustomerRead)]
    [ProducesResponseType(typeof(IReadOnlyCollection<CustomerDuplicateCandidateResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> FindDuplicatesAsync([FromServices] IOptions<PaginationOptions> paginationOptions,
        [FromQuery] long tenantId, [FromQuery] string? email, [FromQuery] string? phone, [FromQuery] string? taxId,
        [FromQuery] string? idNumber, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var settings = paginationOptions.Value;
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = pageSize <= 0 ? settings.DefaultPageSize : pageSize;

        var request = new FindCustomerDuplicatesRequest(tenantId, email, phone, taxId, idNumber);
        var result = await mediator.Send(new FindCustomerDuplicatesQuery(request, safePageNumber, safePageSize),
            cancellationToken);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result.Items);
    }

    [HttpGet("history/by-winner/{winnerCustomerId:long}")]
    [Authorize(Policy = PolicyKeys.Crm.CustomerRead)]
    [ProducesResponseType(typeof(IReadOnlyCollection<CustomerMergeHistoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMergeHistoryByWinnerAsync([FromServices] IOptions<PaginationOptions> paginationOptions,
        [FromRoute] long winnerCustomerId, [FromQuery] long tenantId, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var settings = paginationOptions.Value;
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = pageSize <= 0 ? settings.DefaultPageSize : pageSize;

        var result = await mediator.Send(
            new GetCustomerMergeHistoryByWinnerQuery(tenantId, winnerCustomerId, safePageNumber, safePageSize),
            cancellationToken);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result.Items);
    }

    [HttpGet("history/by-merged/{mergedCustomerId:long}")]
    [Authorize(Policy = PolicyKeys.Crm.CustomerRead)]
    [ProducesResponseType(typeof(IReadOnlyCollection<CustomerMergeHistoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMergeHistoryByMergedAsync([FromServices] IOptions<PaginationOptions> paginationOptions,
        [FromRoute] long mergedCustomerId, [FromQuery] long tenantId, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var settings = paginationOptions.Value;
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = pageSize <= 0 ? settings.DefaultPageSize : pageSize;

        var result = await mediator.Send(
            new GetCustomerMergeHistoryByMergedQuery(tenantId, mergedCustomerId, safePageNumber, safePageSize),
            cancellationToken);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result.Items);
    }
    #endregion

    #region Commands
    [HttpPost("merge")]
    [Authorize(Policy = PolicyKeys.Crm.CustomerUpdate)]
    [ProducesResponseType(typeof(CustomerMergeResultResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> MergeAsync([FromBody] MergeCustomersRequest request,
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new MergeCustomersCommand(request), cancellationToken));
    }
    #endregion
}
