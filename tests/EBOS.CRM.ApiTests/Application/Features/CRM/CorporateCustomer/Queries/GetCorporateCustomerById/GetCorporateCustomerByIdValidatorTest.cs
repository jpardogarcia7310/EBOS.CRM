using EBOS.CRM.Application.Features.CRM.CorporateCustomer.Queries.GetCorporateCustomerById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CorporateCustomer.Queries.GetCorporateCustomerById;

public class GetCorporateCustomerByIdQueryValidatorTest
{
    private readonly GetCorporateCustomerByIdQueryValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidId_Fails(long id)
    {
        var query = new GetCorporateCustomerByIdQuery(id);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}




