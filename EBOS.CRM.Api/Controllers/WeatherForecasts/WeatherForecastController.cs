using EBOS.CRM.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EBOS.CRM.Api.Controllers.WeatherForecasts;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class WeatherForecastController(CrmDbContext db, IMediator mediator) : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];
    private readonly CrmDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    #region Queries
    [HttpGet]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(typeof(ICollection<WeatherForecast>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        // Use Random.Shared (thread-safe) for non-cryptographic randomness
        var rng = Random.Shared;
        var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            // Date-only values
            Date = DateTime.UtcNow.Date.AddDays(index),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)]
        }).ToArray();

        return Ok(result);
    }

    [HttpGet("{date:datetime}")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(typeof(WeatherForecast), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByDate([FromRoute] DateTime date, CancellationToken cancellationToken)
    {
        // Deterministic temperature and summary based on date to avoid creating Random per request
        var daySeed = date.Date.ToBinary();
        var temp = ((int)(unchecked(daySeed) % 76)) - 20; // approximate range -20..55
        var summaryIndex = Math.Abs((int)(daySeed % Summaries.Length));
        var forecast = new WeatherForecast
        {
            Date = date.Date,
            TemperatureC = temp,
            Summary = Summaries[summaryIndex]
        };
        if (date.Date < DateTime.UtcNow.Date)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"No forecast available for {date:yyyy-MM-dd}.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(forecast);
    }

    #endregion

    #region Commands (persist via CrmDbContext and notify via MediatR)
    [HttpPost]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(typeof(WeatherForecast), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] WeatherForecast request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest(new ValidationProblemDetails { Title = "Invalid payload" });
        }
        if (string.IsNullOrWhiteSpace(request.Summary))
        {
            ModelState.AddModelError(nameof(request.Summary), "Summary is required");
            return ValidationProblem(ModelState);
        }
        // Normalize date to date-only
        request.Date = request.Date == default ? DateTime.UtcNow.Date : request.Date.Date;
        // Persist using CrmDbContext
        await _db.Set<WeatherForecast>().AddAsync(request, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        // Publish a notification (handlers can react if registered)
        await _mediator.Publish(new WeatherForecastCreatedNotification(request.Date, request.TemperatureC, request.Summary), cancellationToken);
        // Return Created pointing to GetByDate (v1.0 used for location)
        var location = Url.Action(nameof(GetByDate), new { version = "1.0", date = request.Date.ToString("o") });

        return Created(location ?? string.Empty, request);
    }

    [HttpPut("{id:long}")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(typeof(WeatherForecast), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] long id, [FromBody] WeatherForecast request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest(new ValidationProblemDetails { Title = "Invalid payload" });
        }
        // Minimal validation
        if (string.IsNullOrWhiteSpace(request.Summary))
        {
            ModelState.AddModelError(nameof(request.Summary), "Summary is required");
            return ValidationProblem(ModelState);
        }
        var entity = await _db.Set<WeatherForecast>().FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"WeatherForecast with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }
        // Apply updates (normalize date to date-only if provided)
        entity.Date = request.Date == default ? entity.Date : request.Date.Date;
        entity.TemperatureC = request.TemperatureC;
        entity.Summary = request.Summary;
        _db.Set<WeatherForecast>().Update(entity);
        await _db.SaveChangesAsync(cancellationToken);
        await _mediator.Publish(new WeatherForecastUpdatedNotification(entity.Id, entity.Date, entity.TemperatureC, entity.Summary), cancellationToken);

        return Ok(entity);
    }

    [HttpDelete("{id:long}")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken cancellationToken)
    {
        var entity = await _db.Set<WeatherForecast>().FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"WeatherForecast with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        _db.Set<WeatherForecast>().Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        await _mediator.Publish(new WeatherForecastDeletedNotification(id), cancellationToken);

        return NoContent();
    }

    #endregion

    #region Notifications (simple MediatR notifications published by controller)

    public record WeatherForecastCreatedNotification(DateTime Date, int TemperatureC, string Summary) : INotification;
    public record WeatherForecastUpdatedNotification(long Id, DateTime Date, int TemperatureC, string Summary) : INotification;
    public record WeatherForecastDeletedNotification(long Id) : INotification;

    #endregion
}