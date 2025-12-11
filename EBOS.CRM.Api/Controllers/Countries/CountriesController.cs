using EBOS.CRM.Api.Controllers.Countries.Requests;
using EBOS.CRM.Application.Features.Countries.Commands.AddCountry;
using EBOS.CRM.Application.Features.Countries.Commands.DeleteCountry;
using EBOS.CRM.Application.Features.Countries.Commands.UpdateCountry;
using EBOS.CRM.Application.Features.Countries.Dtos;
using EBOS.CRM.Application.Features.Countries.Queries.GetAllCountries;
using EBOS.CRM.Application.Features.Countries.Queries.GetCountryById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EBOS.CRM.Api.Controllers.Countries;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class CountriesController(IMediator mediator) : ControllerBase
{
    #region Commands
    [HttpPost]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(CountryResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Add([FromBody] AddCountryRQ request, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(
            new AddCountryCommand(
                request.Name, request.Iso31661A2Code, request.Iso31661A3Code,
                request.Iso31661NumCode, request.Domain, request.Currency,
                request.CurrencyCode, request.InternationalPhoneCode),
            cancellationToken
        ));
    }

    [HttpPut("{id:long}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromRoute] long id, [FromBody] UpdateCountryRQ request, CancellationToken cancellationToken)
    {
        var exists = await mediator.Send(new GetCountryByIdQuery(id), cancellationToken);
        if (exists is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"Country with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(await mediator.Send(
            new UpdateCountryCommand(
                id, request.Name, request.Iso31661A2Code, request.Iso31661A3Code, request.Iso31661NumCode,
                request.Domain, request.Currency, request.CurrencyCode, request.InternationalPhoneCode),
            cancellationToken
        ));
    }

    [HttpDelete("{id:long}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken cancellationToken)
    {
        var exists = await mediator.Send(new GetCountryByIdQuery(id), cancellationToken);
        if (exists is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"Country with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        await mediator.Send(new DeleteCountryCommand(id), cancellationToken);
        return NoContent();
    }
    #endregion

    #region Queries
    [HttpGet("{id:long}")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(typeof(CountryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] long id, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(new GetCountryByIdQuery(id), cancellationToken);
        if (dto is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource not found",
                Detail = $"Country with id {id} not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(dto);
    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(typeof(ICollection<CountryResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetAllCountriesQuery(), cancellationToken));
    }
    #endregion
}