using EBOS.CRM.Application.Contracts.Requests.CRM;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.AddTaxInformation;
using EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.DeleteTaxInformation;
using EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.PatchTaxInformation;
using EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.UpdateTaxInformation;
using EBOS.CRM.Application.Features.CRM.TaxInformation.Queries.GetAllTaxInformation;
using EBOS.CRM.Application.Features.CRM.TaxInformation.Queries.GetTaxInformationById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EBOS.CRM.Api.Controllers.CRM.TaxInformation;

[ApiController]
[ApiVersion("3.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class TaxInformationController(IMediator mediator) : ControllerBase
{
    #region Commands
    [HttpPost]
    [ProducesResponseType(typeof(TaxInformationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAsync([FromBody] AddTaxInformationRequest request,
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new AddTaxInformationCommand(request), cancellationToken));
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(TaxInformationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync([FromRoute] long id,
        [FromBody] UpdateTaxInformationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id != request.Id)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid request",
                Detail = "Route id does not match body id.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var dto = await mediator.Send(new UpdateTaxInformationCommand(request), cancellationToken);
        if (dto is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"TaxInformation with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(dto);
    }

    [HttpPatch("{id:long}")]
    [ProducesResponseType(typeof(TaxInformationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchAsync([FromRoute] long id,
        [FromBody] PatchTaxInformationRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new PatchTaxInformationCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"TaxInformation with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(dto);
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync([FromRoute] long id, CancellationToken cancellationToken = default)
    {
        var deleted = await mediator.Send(new DeleteTaxInformationCommand(id), cancellationToken);
        if (!deleted)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"TaxInformation with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return NoContent();
    }
    #endregion

    #region Queries
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(TaxInformationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(new GetTaxInformationByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"TaxInformation with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(dto);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ICollection<TaxInformationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetAllTaxInformationQuery(), cancellationToken));
    }
    #endregion
}
