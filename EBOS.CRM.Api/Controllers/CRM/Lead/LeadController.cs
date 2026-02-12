using EBOS.CRM.Api.Constants;
using EBOS.CRM.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.Lead.Commands.AddLead;
using EBOS.CRM.Application.Features.CRM.Lead.Commands.ConvertLead;
using EBOS.CRM.Application.Features.CRM.Lead.Commands.DisqualifyLead;
using EBOS.CRM.Application.Features.CRM.Lead.Commands.QualifyLead;
using EBOS.CRM.Application.Features.CRM.Lead.Commands.UpdateLead;
using EBOS.CRM.Application.Features.CRM.Lead.Queries.CheckLeadDebtor;
using EBOS.CRM.Application.Features.CRM.Lead.Queries.GetAllLeads;
using EBOS.CRM.Application.Features.CRM.Lead.Queries.GetLeadById;
using EBOS.CRM.Api.Options;
using MediatR;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Api.Controllers.CRM.Lead;

[ApiController]
[ApiVersion("2.0")]
[Route(ApiRouteTemplates.Versioned)]
[Produces("application/json")]
public class LeadController(IMediator mediator) : ControllerBase
{
    #region Commands
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(LeadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAsync([FromBody] AddLeadRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new AddLeadCommand(request), cancellationToken));
    }

    [HttpPut("{id:long}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(LeadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync([FromRoute] long id, [FromBody] UpdateLeadRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new UpdateLeadCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Lead with id {id} not found."));
        }

        return Ok(dto);
    }

    [HttpPost("{id:long}/qualify")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(LeadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> QualifyAsync([FromRoute] long id, [FromBody] QualifyLeadRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new QualifyLeadCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Lead with id {id} not found."));
        }

        return Ok(dto);
    }

    [HttpPost("{id:long}/disqualify")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(LeadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DisqualifyAsync([FromRoute] long id, [FromBody] DisqualifyLeadRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new DisqualifyLeadCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Lead with id {id} not found."));
        }

        return Ok(dto);
    }

    [HttpPost("{id:long}/convert")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(OpportunityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConvertAsync([FromRoute] long id, [FromBody] ConvertLeadRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new ConvertLeadCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Lead with id {id} not found."));
        }

        return Ok(dto);
    }

    [HttpGet("{id:long}/conversion")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(LeadConversionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetConversionAsync([FromRoute] long id,
        CancellationToken cancellationToken = default)
    {
        var lead = await mediator.Send(new GetLeadByIdQuery(id), cancellationToken);
        if (lead is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Lead with id {id} not found."));
        }

        return Ok(new LeadConversionResponse(lead.Id, lead.ConvertedOpportunityId, lead.Status));
    }

    [HttpPost("debtor-check")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(LeadDebtorCheckResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckDebtorAsync([FromBody] LeadDebtorCheckRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new CheckLeadDebtorQuery(request), cancellationToken);
        return Ok(result);
    }
    #endregion

    #region Queries
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(LeadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(new GetLeadByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle,
                detail: $"Lead with id {id} not found."));
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
    [ProducesResponseType(typeof(IReadOnlyCollection<LeadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllAsync([FromServices] IOptions<PaginationOptions> paginationOptions,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var settings = paginationOptions.Value;
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = pageSize <= 0 ? settings.DefaultPageSize : pageSize;
        var result = await mediator.Send(new GetAllLeadsQuery(safePageNumber, safePageSize), cancellationToken);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result.Items);
    }
    #endregion
}
