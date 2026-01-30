using EBOS.CRM.Application.Contracts.Requests.CRM;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.AddCreditAccount;
using EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.DeleteCreditAccount;
using EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.PatchCreditAccount;
using EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.UpdateCreditAccount;
using EBOS.CRM.Application.Features.CRM.CreditAccount.Queries.GetAllCreditAccounts;
using EBOS.CRM.Application.Features.CRM.CreditAccount.Queries.GetCreditAccountById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EBOS.CRM.Api.Controllers.CRM.CreditAccount;

[ApiController]
[ApiVersion("3.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class CreditAccountController(IMediator mediator) : ControllerBase
{
    #region Commands
    [HttpPost]
    [ProducesResponseType(typeof(CreditAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAsync([FromBody] AddCreditAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new AddCreditAccountCommand(request), cancellationToken));
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(CreditAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync([FromRoute] long id,
        [FromBody] UpdateCreditAccountRequest request,
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

        var dto = await mediator.Send(new UpdateCreditAccountCommand(request), cancellationToken);
        if (dto is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"CreditAccount with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(dto);
    }

    [HttpPatch("{id:long}")]
    [ProducesResponseType(typeof(CreditAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchAsync([FromRoute] long id,
        [FromBody] PatchCreditAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new PatchCreditAccountCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"CreditAccount with id {id} not found.",
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
        var deleted = await mediator.Send(new DeleteCreditAccountCommand(id), cancellationToken);
        if (!deleted)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"CreditAccount with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return NoContent();
    }
    #endregion

    #region Queries
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(CreditAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(new GetCreditAccountByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"CreditAccount with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(dto);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ICollection<CreditAccountResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetAllCreditAccountsQuery(), cancellationToken));
    }
    #endregion
}
