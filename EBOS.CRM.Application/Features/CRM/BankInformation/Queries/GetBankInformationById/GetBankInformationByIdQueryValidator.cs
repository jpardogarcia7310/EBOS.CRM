using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.BankInformation.Queries.GetBankInformationById;

public class GetBankInformationByIdQueryValidator : AbstractValidator<GetBankInformationByIdQuery>
{
    public GetBankInformationByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
