

using FluentValidation;


namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Queries.GetBranchOfficeById;

public class GetBranchOfficeByIdQueryValidator : AbstractValidator<GetBranchOfficeByIdQuery>
{
    public GetBranchOfficeByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}




