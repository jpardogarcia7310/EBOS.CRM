using EBOS.CRM.Api.Constants;
using EBOS.CRM.Api.Options;
using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Application.Features.EBOS.TenantConfiguration.Queries.GetAllTenantConfigurations;
using EBOS.CRM.Application.Features.EBOS.TenantConfiguration.Queries.GetTenantConfigurationById;
using MediatR;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Api.Controllers.EBOS.TenantConfiguration;

[ApiController]
[ApiVersion("2.0")]
[Route(ApiRouteTemplates.Versioned)]
[Produces("application/json")]
public class TenantConfigurationController(IMediator mediator) : ControllerBase
{
    #region Queries
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(TenantConfigurationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(new GetTenantConfigurationByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            return NotFound(ProblemDetailsFactory.CreateProblemDetails(HttpContext, statusCode: StatusCodes.Status404NotFound,
                title: ProblemDetailsDefaults.NotFoundTitle, detail: $"TenantConfiguration with id {id} not found."));
        }

        return Ok(dto);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<TenantConfigurationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllAsync([FromServices] IOptions<PaginationOptions> paginationOptions,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var settings = paginationOptions.Value;
        var safePageNumber = Math.Max(1, pageNumber);
        var safePageSize = pageSize <= 0 ? settings.DefaultPageSize : pageSize;
        var result = await mediator.Send(new GetAllTenantConfigurationsQuery(safePageNumber, safePageSize), cancellationToken);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result.Items);
    }
    #endregion
}
