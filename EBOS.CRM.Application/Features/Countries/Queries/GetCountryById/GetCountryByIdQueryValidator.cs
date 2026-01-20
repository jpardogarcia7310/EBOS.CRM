using FluentValidation;

namespace EBOS.CRM.Application.Features.Countries.Queries.GetCountryById;

public class GetCountryByIdQueryValidator : AbstractValidator<GetCountryByIdQuery>
{
    public GetCountryByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El identificador debe ser un número entero positivo mayor que 0.")
            .WithErrorCode("VAL_ID_POSITIVE");
    }
}