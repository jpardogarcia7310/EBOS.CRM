using EBOS.CRM.Application.Features.CRM.BankInformation.Queries.GetBankInformationById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BankInformation.Queries.GetBankInformationById;

public class GetBankInformationByIdQueryValidatorTest
{
    private readonly GetBankInformationByIdQueryValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidId_Fails(long id)
    {
        var query = new GetBankInformationByIdQuery(id);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}




