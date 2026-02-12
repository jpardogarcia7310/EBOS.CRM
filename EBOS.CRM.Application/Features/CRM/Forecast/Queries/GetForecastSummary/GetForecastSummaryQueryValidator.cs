using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Forecast.Queries.GetForecastSummary;

public sealed class GetForecastSummaryQueryValidator : AbstractValidator<GetForecastSummaryQuery>
{
    public GetForecastSummaryQueryValidator()
    {
        RuleFor(x => x.ForecastRequest).NotNull();

        When(x => x.ForecastRequest != null, () =>
        {
            RuleFor(x => x.ForecastRequest.TenantId).GreaterThan(0);
        });
    }
}
