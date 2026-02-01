using EBOS.CRM.Application.Contracts.Requests.CRM;
using EBOS.CRM.Application.Contracts.Requests.CRM.Address;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.Address.Commands.AddAddress;
using EBOS.CRM.Application.Features.CRM.Address.Commands.DeleteAddress;
using EBOS.CRM.Application.Features.CRM.Address.Commands.UpdateAddress;
using EBOS.CRM.Application.Features.CRM.Address.Queries.GetAddressById;
using EBOS.CRM.Application.Features.CRM.Address.Queries.GetAllAddresses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EBOS.CRM.Application.Contracts.Requests.Common;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Api.Controllers.CRM.Address;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class AddressController(IMediator mediator) : ControllerBase
{
    #region Commands
    /// <summary>
    /// Creates a new address.
    /// </summary>
    /// <example>
    /// POST /api/v2/Address
    /// {
    ///   "street": "Main St",
    ///   "externalNumber": "123",
    ///   "city": "Quito",
    ///   "stateOrProvince": "Pichincha",
    ///   "postalCode": "EC17001",
    ///   "customerId": 1,
    ///   "countryId": 1,
    ///   "addressTypeId": 1
    /// }
    /// </example>
    /// <response code="200">Address created.</response>
    /// <response code="400">Validation error.</response>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(AddressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAsync([FromBody] AddAddressRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new AddAddressCommand(request), cancellationToken));
    }

    /// <summary>
    /// Updates an existing address.
    /// </summary>
    /// <example>
    /// PUT /api/v2/Address/1
    /// {
    ///   "street": "Main St",
    ///   "externalNumber": "123",
    ///   "city": "Quito",
    ///   "stateOrProvince": "Pichincha",
    ///   "postalCode": "EC17001",
    ///   "countryId": 1,
    ///   "addressTypeId": 1
    /// }
    /// </example>
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
    /// Deletes an address by its identifier.
    /// </summary>
    /// <example>
    /// DELETE /api/v2/Address/1
    /// </example>
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
    #endregion

    #region Queries
    /// <summary>
    /// Returns an address by its identifier.
    /// </summary>
    /// <example>
    /// GET /api/v2/Address/1
    /// </example>
    /// <response code="200">Address found.</response>
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
    /// Returns all addresses.
    /// </summary>
    /// <example>
    /// GET /api/v2/Address
    /// </example>
    /// <response code="200">List of addresses.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<AddressResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAsync([FromQuery] PagedQueryRequest query, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetAllAddressQuery(query), cancellationToken));
    }

    #endregion
}


