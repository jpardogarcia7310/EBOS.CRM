using EBOS.CRM.Application.Features.CRM.Address.Queries.GetAddressById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Address.Queries.GetAddressById;

public class GetAddressByIdQueryValidatorTest
{
    private readonly GetAddressByIdQueryValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidId_Fails(long id)
    {
        var query = new GetAddressByIdQuery(id);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}


