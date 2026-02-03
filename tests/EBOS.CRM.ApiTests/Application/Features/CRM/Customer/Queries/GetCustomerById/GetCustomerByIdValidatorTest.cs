using EBOS.CRM.Application.Features.CRM.Customer.Queries.GetCustomerById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Customer.Queries.GetCustomerById;

public class GetCustomerByIdQueryValidatorTest
{
    private readonly GetCustomerByIdQueryValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidId_Fails(long id)
    {
        var query = new GetCustomerByIdQuery(id);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
