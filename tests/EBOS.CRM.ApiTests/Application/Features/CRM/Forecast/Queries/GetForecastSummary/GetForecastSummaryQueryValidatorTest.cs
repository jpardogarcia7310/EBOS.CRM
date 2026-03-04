using EBOS.CRM.Application.Features.CRM.Forecast.Queries.GetForecastSummary;
using EBOS.CRM.Contracts.Requests.CRM.Forecast;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Forecast.Queries.GetForecastSummary;

public class GetForecastSummaryQueryValidatorTest
{
    private readonly GetForecastSummaryQueryValidator _validator = new();

    [Fact]
    public async Task Validate_NullForecastRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(new GetForecastSummaryQuery(null!));
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(
            new GetForecastSummaryQuery(new GetForecastRequest(1, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow, null, null)));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidTenant_Fails()
    {
        var result = await _validator.TestValidateAsync(
            new GetForecastSummaryQuery(new GetForecastRequest(0, null, null, null, null)));
        Assert.False(result.IsValid);
    }
}
