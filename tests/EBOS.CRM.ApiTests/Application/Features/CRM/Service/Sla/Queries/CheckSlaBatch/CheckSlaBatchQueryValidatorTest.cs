using EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.CheckSlaBatch;
using EBOS.CRM.Contracts.Requests.CRM.Service.Sla;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Sla.Queries.CheckSlaBatch;

public class CheckSlaBatchQueryValidatorTest
{
    private readonly CheckSlaBatchQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(
            new CheckSlaBatchQuery(new CheckSlaBatchRequest(1, DateTime.UtcNow, 1, 10)));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_InvalidTenant_Fails()
    {
        var result = await _validator.TestValidateAsync(
            new CheckSlaBatchQuery(new CheckSlaBatchRequest(0, DateTime.UtcNow, 1, 10)));
        Assert.False(result.IsValid);
    }
}
