using EBOS.CRM.Application.Contracts.Requests.CRM.CreditAccount;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.AddCreditAccount;
using EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.DeleteCreditAccount;
using EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.UpdateCreditAccount;
using EBOS.CRM.Application.Features.CRM.CreditAccount.Queries.GetCreditAccountById;
using EBOS.CRM.Application.Features.CRM.CreditAccount.Queries.GetAllCreditAccounts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EBOS.CRM.Application.Contracts.Requests.Common;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Api.Controllers.CRM.CreditAccount;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class CreditAccountController(IMediator mediator) : ControllerBase
{
    #region Commands
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CreditAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAsync([FromBody] AddCreditAccountRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new AddCreditAccountCommand(request), cancellationToken));
    }

    [HttpPut("{id:long}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CreditAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync([FromRoute] long id, [FromBody] UpdateCreditAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new UpdateCreditAccountCommand(id, request), cancellationToken);
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
    [ProducesResponseType(StatusCodes.Status200OK)]
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

        return Ok();
    }
    #endregion

    #region Queries
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(CreditAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
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
    [ProducesResponseType(typeof(PagedResponse<CreditAccountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAsync([FromQuery] PagedQueryRequest query, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetAllCreditAccountsQuery(query), cancellationToken));
    }

    #endregion
}




