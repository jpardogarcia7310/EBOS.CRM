using EBOS.CRM.Application.Features.CRM.IndividualCustomer.Queries.GetIndividualCustomerById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.IndividualCustomer.Queries.GetIndividualCustomerById;

public class GetIndividualCustomerByIdQueryValidatorTest
{
    private readonly GetIndividualCustomerByIdQueryValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_InvalidId_Fails(long id)
    {
        var query = new GetIndividualCustomerByIdQuery(id);

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}