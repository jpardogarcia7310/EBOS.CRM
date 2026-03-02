using EBOS.CRM.Application.Features.CRM.BranchOffice.Queries.GetBranchOfficeById;
using FluentValidation.TestHelper;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BranchOffice.Queries.GetBranchOfficeById;

public class GetBranchOfficeByIdQueryValidatorTest
{
    private readonly GetBranchOfficeByIdQueryValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_InvalidId_Fails(long id)
    {
        var query = new GetBranchOfficeByIdQuery(id);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}




