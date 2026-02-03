using EBOS.CRM.Application.Features.CRM.TaxInformation.Queries.GetTaxInformationById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.TaxInformation.Queries.GetTaxInformationById;

public class GetTaxInformationByIdQueryValidatorTest
{
    private readonly GetTaxInformationByIdQueryValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidId_Fails(long id)
    {
        var query = new GetTaxInformationByIdQuery(id);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
