using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Features.AddressesType.Query.GetAddressTypeById;
using EBOS.CRM.Application.Features.AddressesType.Query.GetAllAddressesType;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EBOS.CRM.Application.Contracts.Requests.Common;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Api.Controllers.AddressType;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class AddressTypeController(IMediator mediator) : ControllerBase
{
    #region Queries

    /// <summary>
    /// Returns an address type by its identifier.
    /// </summary>
    /// <example>
    /// GET /api/v2/AddressType/1
    /// </example>
    /// <response code="200">Address type found.</response>
    /// <response code="404">Address type not found.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(AddressTypeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(new GetAddressTypeByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"AddressType with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(dto);
    }

    /// <summary>
    /// Returns all address types.
    /// </summary>
    /// <example>
    /// GET /api/v2/AddressType
    /// </example>
    /// <response code="200">List of address types.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<AddressTypeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAsync([FromQuery] PagedQueryRequest query, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetAllAddressesTypeQuery(query), cancellationToken));
    }

    #endregion
}


