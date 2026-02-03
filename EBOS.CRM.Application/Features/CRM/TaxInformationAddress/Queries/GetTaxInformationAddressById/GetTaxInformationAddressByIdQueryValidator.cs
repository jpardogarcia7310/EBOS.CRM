

using FluentValidation;


namespace EBOS.CRM.Application.Features.CRM.TaxInformationAddress.Queries.GetTaxInformationAddressById;

public class GetTaxInformationAddressByIdQueryValidator : AbstractValidator<GetTaxInformationAddressByIdQuery>
{
    public GetTaxInformationAddressByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}




