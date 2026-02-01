using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Queries.GetTaxInformationById;

public class GetTaxInformationByIdQueryValidator : AbstractValidator<GetTaxInformationByIdQuery>
{
    public GetTaxInformationByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
