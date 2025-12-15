using FluentValidation;

namespace EBOS.CRM.Application.Features.TaxRegimes.Queries.GetTaxRegimeById;

public class GetTaxRegimeByIdQueryValidator : AbstractValidator<GetTaxRegimeByIdQuery>
{
    public GetTaxRegimeByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El identificador debe ser un número entero positivo mayor que 0.")
            .WithErrorCode("VAL_ID_POSITIVE");
    }
}