using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EBOS.CRM.Application.Contracts.Requests.CRM.CorporateCustomer;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.CorporateCustomer.Commands.AddCorporateCustomer;
using EBOS.CRM.Application.Features.CRM.CorporateCustomer.Commands.DeleteCorporateCustomer;
using EBOS.CRM.Application.Features.CRM.CorporateCustomer.Commands.UpdateCorporateCustomer;
using EBOS.CRM.Application.Features.CRM.CorporateCustomer.Queries.GetCorporateCustomerById;
using EBOS.CRM.Application.Features.CRM.CorporateCustomer.Queries.GetAllCorporateCustomers;
using MediatR;
using EBOS.CRM.Api.Options;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Localization;
using EBOS.CRM.Api.Resources;

namespace EBOS.CRM.Api.Controllers.CRM.CorporateCustomer;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class CorporateCustomerController(IMediator mediator, IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    #region Commands
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CorporateCustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAsync([FromBody] AddCorporateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new AddCorporateCustomerCommand(request), cancellationToken));
    }

    [HttpPut("{id:long}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CorporateCustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync([FromRoute] long id, [FromBody] UpdateCorporateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new UpdateCorporateCustomerCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"CorporateCustomer with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(dto);
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync([FromRoute] long id, CancellationToken cancellationToken = default)
    {
        var deleted = await mediator.Send(new DeleteCorporateCustomerCommand(id), cancellationToken);
        if (!deleted)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"CorporateCustomer with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok();
    }
    #endregion

    #region Queries
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(CorporateCustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(new GetCorporateCustomerByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"CorporateCustomer with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(dto);
    }
    /// <summary>
    /// Returns all resources (paginated).
    /// </summary>
    /// <param name="paginationOptions">Pagination settings.</param>
    /// <param name="pageNumber">1-based page number.</param>
    /// <param name="pageSize">Page size (must be &lt;= configured max).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">List of resources. Adds X-Total-Count header.</response>
    /// <response code="400">Invalid pageSize.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CorporateCustomerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllAsync([FromServices] IOptions<PaginationOptions> paginationOptions, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var settings = paginationOptions.Value;
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = pageSize <= 0 ? settings.DefaultPageSize : pageSize;
        if (safePageSize > settings.MaxPageSize)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid pageSize",
                Detail = localizer["InvalidPageSize", settings.MaxPageSize],
                Status = StatusCodes.Status400BadRequest
            });
        }

        var result = await mediator.Send(new GetAllCorporateCustomersQuery(safePageNumber, safePageSize), cancellationToken);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result.Items);
    }

    #endregion
}
























