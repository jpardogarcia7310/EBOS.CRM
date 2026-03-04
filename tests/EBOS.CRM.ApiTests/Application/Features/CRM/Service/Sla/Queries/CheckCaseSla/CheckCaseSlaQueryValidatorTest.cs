using EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.CheckCaseSla;
using EBOS.CRM.Contracts.Requests.CRM.Service.Sla;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Sla.Queries.CheckCaseSla;

public class CheckCaseSlaQueryValidatorTest
{
    private readonly CheckCaseSlaQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var result = await _validator.TestValidateAsync(
            new CheckCaseSlaQuery(new CheckCaseSlaRequest(1, 10, DateTime.UtcNow)));
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_NullRequest_Fails()
    {
        var result = await _validator.TestValidateAsync(new CheckCaseSlaQuery(null!));
        Assert.False(result.IsValid);
    }
}
