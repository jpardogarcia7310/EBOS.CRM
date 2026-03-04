using EBOS.CRM.Application.Features.CRM.CustomerMerge.Commands.MergeCustomers;
using EBOS.CRM.Contracts.Requests.CRM.CustomerMerge;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerMerge.Commands.MergeCustomers;

public class MergeCustomersCommandValidatorTest
{
    private readonly MergeCustomersCommandValidator _validator = new();

    [Fact]
    public async Task Validate_ValidRequest_Passes()
    {
        var command = new MergeCustomersCommand(new MergeCustomersRequest(
            1, 10, new List<long> { 11, 12 }, "dedupe"));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyMergeIds_Fails()
    {
        var command = new MergeCustomersCommand(new MergeCustomersRequest(
            1, 10, Array.Empty<long>(), "dedupe"));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.MergeCustomerIds);
    }

    [Fact]
    public async Task Validate_WinnerInsideMergeList_Fails()
    {
        var command = new MergeCustomersCommand(new MergeCustomersRequest(
            1, 10, new List<long> { 10, 11 }, "dedupe"));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Request);
    }
}


