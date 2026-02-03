using EBOS.CRM.Application.Features.CRM.BankInformation.Queries.GetBankInformationById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BankInformation.Queries.GetBankInformationById;

public class GetBankInformationByIdQueryValidatorTest
{
    private readonly GetBankInformationByIdQueryValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidId_Fails(long id)
    {
        var query = new GetBankInformationByIdQuery(id);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
