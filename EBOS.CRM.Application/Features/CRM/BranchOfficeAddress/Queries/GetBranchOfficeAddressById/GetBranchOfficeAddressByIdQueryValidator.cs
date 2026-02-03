

using FluentValidation;


namespace EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Queries.GetBranchOfficeAddressById;

public class GetBranchOfficeAddressByIdQueryValidator : AbstractValidator<GetBranchOfficeAddressByIdQuery>
{
    public GetBranchOfficeAddressByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}




