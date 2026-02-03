using EBOS.CRM.Application.Contracts.Requests.CRM.Address;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.Address.Commands.AddAddress;
using EBOS.CRM.Application.Features.CRM.Address.Commands.DeleteAddress;
using EBOS.CRM.Application.Features.CRM.Address.Commands.UpdateAddress;
using EBOS.CRM.Application.Features.CRM.Address.Queries.GetAddressById;
using EBOS.CRM.Application.Features.CRM.Address.Queries.GetAllAddresses;
using MediatR;
using EBOS.CRM.Api.Options;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Localization;
using EBOS.CRM.Api.Resources;

namespace EBOS.CRM.Api.Controllers.CRM.Address;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class AddressController(IMediator mediator, IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    /// <summary>
    /// Creates a new address.
    /// </summary>
    /// <param name="request">Address payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Address created.</response>
    /// <response code="400">Invalid request.</response>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(AddressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAsync([FromBody] AddAddressRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new AddAddressCommand(request), cancellationToken));
    }

    /// <summary>
    /// Updates an address by id.
    /// </summary>
    /// <param name="id">Address id.</param>
    /// <param name="request">Address payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Address updated.</response>
    /// <response code="404">Address not found.</response>
    [HttpPut("{id:long}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(AddressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync([FromRoute] long id, [FromBody] UpdateAddressRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new UpdateAddressCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"Address with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(dto);
    }

    /// <summary>
    /// Deletes an address by id.
    /// </summary>
    /// <param name="id">Address id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Address deleted.</response>
    /// <response code="404">Address not found.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync([FromRoute] long id, CancellationToken cancellationToken = default)
    {
        var deleted = await mediator.Send(new DeleteAddressCommand(id), cancellationToken);
        if (!deleted)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"Address with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok();
    }

    /// <summary>
    /// Gets an address by id.
    /// </summary>
    /// <param name="id">Address id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Address found.</response>
    /// <response code="400">Invalid id.</response>
    /// <response code="404">Address not found.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(AddressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(new GetAddressByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"Address with id {id} not found.",
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
    [ProducesResponseType(typeof(IReadOnlyCollection<AddressResponse>), StatusCodes.Status200OK)]
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

        var result = await mediator.Send(new GetAllAddressesQuery(safePageNumber, safePageSize), cancellationToken);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result.Items);
    }
}






















