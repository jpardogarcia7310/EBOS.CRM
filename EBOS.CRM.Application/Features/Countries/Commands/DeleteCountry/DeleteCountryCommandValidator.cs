using FluentValidation;

namespace EBOS.CRM.Application.Features.Countries.Commands.DeleteCountry;

public sealed class DeleteCountryCommandValidator : AbstractValidator<DeleteCountryCommand>
{
    public DeleteCountryCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}