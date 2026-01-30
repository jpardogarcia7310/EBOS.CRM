using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Features.IdentificationType.Query.GetAllIdentificationType;
using EBOS.CRM.Application.Features.IdentificationType.Query.GetIdentificationTypeByIdQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EBOS.CRM.Api.Controllers.IdentificationType;

[ApiController]
[ApiVersion("2.0")]
[ApiVersion("3.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class IdentificationTypeController(IMediator mediator) : ControllerBase
{
    #region Queries
    /// <summary>
    /// Returns an identification type by its identifier.
    /// </summary>
    /// <example>
    /// GET /api/v2/IdentificationType/1
    /// </example>
    /// <response code="200">Identification type found.</response>
    /// <response code="404">Identification type not found.</response>
    [HttpGet("{id:long}")]
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

    /// <summary>
    /// Returns all identification types.
    /// </summary>
    /// <example>
    /// GET /api/v2/IdentificationType
    /// </example>
    /// <response code="200">List of identification types.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ICollection<IdentificationTypeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetAllIdentificationTypeQuery(), cancellationToken));
    }

    #endregion
}
