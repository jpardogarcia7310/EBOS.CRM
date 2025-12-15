using EBOS.CRM.Application.Features.Countries.Queries.GetCountryById;
using FluentValidation;

namespace EBOS.CRM.Application.Features.Statuses.Queries.GetStatusById;

public class GetStatusByIdQueryValidator : AbstractValidator<GetStatusByIdQuery>
{
    public GetStatusByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El identificador debe ser un número entero positivo mayor que 0.")
            .WithErrorCode("VAL_ID_POSITIVE");
    }
}