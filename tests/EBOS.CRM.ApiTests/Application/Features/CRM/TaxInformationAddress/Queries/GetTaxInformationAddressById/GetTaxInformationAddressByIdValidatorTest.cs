using EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Queries.GetTaxInformationAddressById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.TaxInformationAddress.Queries.GetTaxInformationAddressById;

public class GetTaxInformationAddressByIdQueryValidatorTest
{
    private readonly GetTaxInformationAddressByIdQueryValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidId_Fails(long id)
    {
        var query = new GetTaxInformationAddressByIdQuery(id);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}




