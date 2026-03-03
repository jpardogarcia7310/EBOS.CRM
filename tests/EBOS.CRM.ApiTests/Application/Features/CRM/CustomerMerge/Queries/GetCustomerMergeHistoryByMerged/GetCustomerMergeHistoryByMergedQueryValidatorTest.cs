using EBOS.CRM.Application.Features.CRM.CustomerMerge.Queries.GetCustomerMergeHistoryByMerged;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerMerge.Queries.GetCustomerMergeHistoryByMerged;

public class GetCustomerMergeHistoryByMergedQueryValidatorTest
{
    private readonly GetCustomerMergeHistoryByMergedQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var query = new GetCustomerMergeHistoryByMergedQuery(1, 10, 1, 50);
        var result = await _validator.TestValidateAsync(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidTenantId_Fails(long tenantId)
    {
        var query = new GetCustomerMergeHistoryByMergedQuery(tenantId, 10, 1, 50);
        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.TenantId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidMergedCustomerId_Fails(long mergedCustomerId)
    {
        var query = new GetCustomerMergeHistoryByMergedQuery(1, mergedCustomerId, 1, 50);
        var result = await _validator.TestValidateAsync(query);
        result.ShouldHaveValidationErrorFor(x => x.MergedCustomerId);
    }
}
