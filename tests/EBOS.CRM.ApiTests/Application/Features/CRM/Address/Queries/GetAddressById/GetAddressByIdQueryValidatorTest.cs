using EBOS.CRM.Application.Features.CRM.Address.Queries.GetAddressById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Address.Queries.GetAddressById;

public class GetAddressByIdQueryValidatorTest
{
    private readonly GetAddressByIdQueryValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidId_Fails(long id)
    {
        var query = new GetAddressByIdQuery(id);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}




