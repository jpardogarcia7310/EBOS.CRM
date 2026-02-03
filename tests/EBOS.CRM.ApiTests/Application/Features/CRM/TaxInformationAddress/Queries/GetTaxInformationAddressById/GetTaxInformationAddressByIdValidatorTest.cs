using EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Queries.GetTaxInformationAddressById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.TaxInformationAddress.Queries.GetTaxInformationAddressById;

public class GetTaxInformationAddressByIdQueryValidatorTest
{
    private readonly GetTaxInformationAddressByIdQueryValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidId_Fails(long id)
    {
        var query = new GetTaxInformationAddressByIdQuery(id);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}


