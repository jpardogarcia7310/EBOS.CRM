using EBOS.CRM.Application.Features.CRM.CustomerMerge.Queries.GetCustomerMergeHistoryByWinner;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerMerge.Queries.GetCustomerMergeHistoryByWinner;

public class GetCustomerMergeHistoryByWinnerQueryValidatorTest
{
    private readonly GetCustomerMergeHistoryByWinnerQueryValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var query = new GetCustomerMergeHistoryByWinnerQuery(1, 10, 1, 50);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidTenantId_Fails(long tenantId)
    {
        var query = new GetCustomerMergeHistoryByWinnerQuery(tenantId, 10, 1, 50);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.TenantId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidWinnerCustomerId_Fails(long winnerCustomerId)
    {
        var query = new GetCustomerMergeHistoryByWinnerQuery(1, winnerCustomerId, 1, 50);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.WinnerCustomerId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidPageNumber_Fails(int pageNumber)
    {
        var query = new GetCustomerMergeHistoryByWinnerQuery(1, 10, pageNumber, 50);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidPageSize_Fails(int pageSize)
    {
        var query = new GetCustomerMergeHistoryByWinnerQuery(1, 10, 1, pageSize);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}
