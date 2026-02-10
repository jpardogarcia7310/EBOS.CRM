using EBOS.CRM.Api.Constants;
using EBOS.CRM.Application.Contracts.Requests.CRM.Opportunity;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.Opportunity.Commands.AddOpportunity;
using EBOS.CRM.Application.Features.CRM.Opportunity.Commands.CloseOpportunity;
using EBOS.CRM.Application.Features.CRM.Opportunity.Commands.PatchOpportunityStage;
using EBOS.CRM.Application.Features.CRM.Opportunity.Commands.UpdateOpportunity;
using EBOS.CRM.Application.Features.CRM.Opportunity.Queries.GetAllOpportunities;
using EBOS.CRM.Application.Features.CRM.Opportunity.Queries.GetOpportunityById;
using EBOS.CRM.Api.Options;
using MediatR;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Api.Controllers.CRM.Opportunity;

[ApiController]
[ApiVersion("2.0")]
[Route(ApiRouteTemplates.Versioned)]
[Produces("application/json")]
public class OpportunityController(IMediator mediator) : ControllerBase
{
    #region Commands
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(OpportunityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAsync([FromBody] AddOpportunityRequest request,
        CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new AddOpportunityCommand(request), cancellationToken));
    }

    [HttpPut("{id:long}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(OpportunityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync([FromRoute] long id, [FromBody] UpdateOpportunityRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new UpdateOpportunityCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Opportunity with id {id} not found."));
        }

        return Ok(dto);
    }

    [HttpPost("{id:long}/close")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(OpportunityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CloseAsync([FromRoute] long id, [FromBody] CloseOpportunityRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new CloseOpportunityCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Opportunity with id {id} not found."));
        }

        return Ok(dto);
    }

    [HttpPost("{id:long}/stage")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(OpportunityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchStageAsync([FromRoute] long id, [FromBody] PatchOpportunityStageRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new PatchOpportunityStageCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Opportunity with id {id} not found."));
        }

        return Ok(dto);
    }
    #endregion

    #region Queries
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(OpportunityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(new GetOpportunityByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Opportunity with id {id} not found."));
        }

        return Ok(dto);
    }

    /// <summary>
    /// Returns all resources (paginated).
    /// </summary>
    /// <param name="paginationOptions">Pagination settings.</param>
    /// <param name="pageNumber">1-based page number.</param>
    /// <param name="pageSize">Page size (must be &lt;= configured max).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">List of resources. Adds X-Total-Count header.</response>
    /// <response code="400">Invalid pageSize.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<OpportunityResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllAsync([FromServices] IOptions<PaginationOptions> paginationOptions,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var settings = paginationOptions.Value;
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = pageSize <= 0 ? settings.DefaultPageSize : pageSize;
        var result = await mediator.Send(new GetAllOpportunitiesQuery(safePageNumber, safePageSize), cancellationToken);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result.Items);
    }
    #endregion
}
