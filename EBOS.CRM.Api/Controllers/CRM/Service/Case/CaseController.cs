using EBOS.CRM.Api.Constants;
using EBOS.CRM.Application.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AssignCaseSla;
using MediatR;

namespace EBOS.CRM.Api.Controllers.CRM.Service.Case;

[ApiController]
[ApiVersion("2.0")]
[Route(ApiRouteTemplates.Versioned)]
[Produces("application/json")]
public class CaseController(IMediator mediator) : ControllerBase
{
    [HttpPatch("{id:long}/sla")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignSlaAsync([FromRoute] long id,
        [FromBody] AssignCaseSlaRequest request, CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new AssignCaseSlaCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Case with id {id} not found."));
        }

        return Ok(dto);
    }
}
