using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Commands.UpdateAccountContact;

public class UpdateAccountContactCommandValidator : AbstractValidator<UpdateAccountContactCommand>
{
    public UpdateAccountContactCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.AccountContactRequest).NotNull();
        RuleFor(x => x.AccountContactRequest.TenantId).GreaterThan(0);
        RuleFor(x => x.AccountContactRequest.CorporateCustomerId).GreaterThan(0);
        RuleFor(x => x.AccountContactRequest.IndividualCustomerId).GreaterThan(0);
        RuleFor(x => x.AccountContactRequest.StartAt).NotEmpty();
        RuleFor(x => x.AccountContactRequest.EndAt)
            .Must((request, endAt) => !endAt.HasValue || endAt.Value >= request.AccountContactRequest.StartAt)
            .WithMessage("EndAt cannot be earlier than StartAt.");
    }
}
