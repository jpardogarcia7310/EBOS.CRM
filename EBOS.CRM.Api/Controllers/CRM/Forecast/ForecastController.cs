using EBOS.CRM.Api.Constants;
using EBOS.CRM.Application.Contracts.Requests.CRM.Forecast;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.Forecast.Queries.GetForecastSummary;
using MediatR;

namespace EBOS.CRM.Api.Controllers.CRM.Forecast;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}")]
[Produces("application/json")]
public class ForecastController(IMediator mediator) : ControllerBase
{
    [HttpGet("forecast")]
    [ProducesResponseType(typeof(ForecastSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSummaryAsync([FromQuery] GetForecastRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetForecastSummaryQuery(request), cancellationToken);
        return Ok(result);
    }
}
