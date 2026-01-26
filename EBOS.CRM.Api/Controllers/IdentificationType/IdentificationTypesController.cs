using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Features.IdentificationType.Query.GetAllIdentificationType;
using EBOS.CRM.Application.Features.IdentificationType.Query.GetIdentificationTypeByIdQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EBOS.CRM.Api.Controllers.IdentificationType;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class IdentificationTypesController(IMediator mediator) : ControllerBase
{
    #region Queries

    [HttpGet("{id:long}")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(typeof(IdentificationTypeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(new GetIdentificationTypeByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"IdentificationType with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(dto);
    }

    [HttpGet]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(typeof(ICollection<IdentificationTypeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetAllIdentificationTypeQuery(), cancellationToken));
    }

    #endregion
}