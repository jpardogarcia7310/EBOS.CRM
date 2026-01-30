using EBOS.CRM.Application.Contracts.Requests.CRM;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.AddBranchOffice;
using EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.DeleteBranchOffice;
using EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.PatchBranchOffice;
using EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.UpdateBranchOffice;
using EBOS.CRM.Application.Features.CRM.BranchOffice.Queries.GetAllBranchOffices;
using EBOS.CRM.Application.Features.CRM.BranchOffice.Queries.GetBranchOfficeById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EBOS.CRM.Api.Controllers.CRM.BranchOffice;

[ApiController]
[ApiVersion("3.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class BranchOfficeController(IMediator mediator) : ControllerBase
{
    #region Commands
    [HttpPost]
    [ProducesResponseType(typeof(BranchOfficeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAsync([FromBody] AddBranchOfficeRequest request,
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new AddBranchOfficeCommand(request), cancellationToken));
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(BranchOfficeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync([FromRoute] long id,
        [FromBody] UpdateBranchOfficeRequest request,
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

        var dto = await mediator.Send(new UpdateBranchOfficeCommand(request), cancellationToken);
        if (dto is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"BranchOffice with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(dto);
    }

    [HttpPatch("{id:long}")]
    [ProducesResponseType(typeof(BranchOfficeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchAsync([FromRoute] long id,
        [FromBody] PatchBranchOfficeRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new PatchBranchOfficeCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"BranchOffice with id {id} not found.",
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
        var deleted = await mediator.Send(new DeleteBranchOfficeCommand(id), cancellationToken);
        if (!deleted)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"BranchOffice with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return NoContent();
    }
    #endregion

    #region Queries
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(BranchOfficeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(new GetBranchOfficeByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"BranchOffice with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(dto);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ICollection<BranchOfficeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetAllBranchOfficesQuery(), cancellationToken));
    }
    #endregion
}
