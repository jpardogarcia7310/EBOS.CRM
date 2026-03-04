using EBOS.CRM.Application.Features.CRM.CustomerAddress.Queries.GetCustomerAddressById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CustomerAddress.Queries.GetCustomerAddressById;

public class GetCustomerAddressByIdQueryValidatorTest
{
    private readonly GetCustomerAddressByIdQueryValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidId_Fails(long id)
    {
        var query = new GetCustomerAddressByIdQuery(id);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}




