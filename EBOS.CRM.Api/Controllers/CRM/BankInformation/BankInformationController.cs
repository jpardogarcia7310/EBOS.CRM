using EBOS.CRM.Api.Constants;
using EBOS.CRM.Application.Contracts.Requests.CRM.BankInformation;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.BankInformation.Commands.AddBankInformation;
using EBOS.CRM.Application.Features.CRM.BankInformation.Commands.DeleteBankInformation;
using EBOS.CRM.Application.Features.CRM.BankInformation.Commands.UpdateBankInformation;
using EBOS.CRM.Application.Features.CRM.BankInformation.Queries.GetBankInformationById;
using EBOS.CRM.Application.Features.CRM.BankInformation.Queries.GetAllBankInformations;
using MediatR;
using EBOS.CRM.Api.Options;
using Microsoft.Extensions.Options;
namespace EBOS.CRM.Api.Controllers.CRM.BankInformation;
[ApiController]
[ApiVersion("2.0")]
[Route(ApiRouteTemplates.Versioned)]
[Produces("application/json")]
public class BankInformationController(IMediator mediator) : ControllerBase
{
    #region Commands
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BankInformationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAsync([FromBody] AddBankInformationRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new AddBankInformationCommand(request), cancellationToken));
    }
    [HttpPut("{id:long}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BankInformationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync([FromRoute] long id, [FromBody] UpdateBankInformationRequest request,
        CancellationToken cancellationToken = default)
    {
        var dto = await mediator.Send(new UpdateBankInformationCommand(id, request), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext, statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle, detail: $"BankInformation with id {id} not found."));
        }
        return Ok(dto);
    }
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync([FromRoute] long id, CancellationToken cancellationToken = default)
    {
        var deleted = await mediator.Send(new DeleteBankInformationCommand(id), cancellationToken);
        if (!deleted)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext, statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle, detail: $"BankInformation with id {id} not found."));
        }
        return Ok();
    }
    #endregion
    #region Queries
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(BankInformationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(new GetBankInformationByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext, statusCode: StatusCodes.Status404NotFound, title: ProblemDetailsDefaults.NotFoundTitle, detail: $"BankInformation with id {id} not found."));
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
    [ProducesResponseType(typeof(IReadOnlyCollection<BankInformationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllAsync([FromServices] IOptions<PaginationOptions> paginationOptions, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var settings = paginationOptions.Value;
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = pageSize <= 0 ? settings.DefaultPageSize : pageSize;
        var result = await mediator.Send(new GetAllBankInformationsQuery(safePageNumber, safePageSize), cancellationToken);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result.Items);
    }
    #endregion
}









