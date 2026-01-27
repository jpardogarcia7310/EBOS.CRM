using EBOS.CRM.Application.Contracts.Requests.CRM;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.Address.Commands.AddAddress;
using EBOS.CRM.Application.Features.CRM.Address.Queries.GetAddressById;
using EBOS.CRM.Application.Features.CRM.Address.Queries.GetAllAddresses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EBOS.CRM.Api.Controllers.CRM.Address;

[ApiController]
[ApiVersion("2.1")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class AddressController(IMediator mediator) : ControllerBase
{
    #region Commands
    [HttpPost]
    [MapToApiVersion("2.1")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(AddressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAsync([FromBody] AddAddressRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new AddAddressCommand(request), cancellationToken));
    }
    #endregion
    
    #region Queries
    [HttpGet("{id:long}")]
    [MapToApiVersion("2.1")]
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

    [HttpGet]
    [MapToApiVersion("2.1")]
    [ProducesResponseType(typeof(ICollection<AddressResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetAllAddressQuery(), cancellationToken));
    }

    #endregion
}