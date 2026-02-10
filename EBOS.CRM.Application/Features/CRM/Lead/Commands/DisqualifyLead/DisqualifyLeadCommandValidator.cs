using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Lead.Commands.DisqualifyLead;

public class DisqualifyLeadCommandValidator : AbstractValidator<DisqualifyLeadCommand>
{
    public DisqualifyLeadCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.LeadRequest).NotNull();

        When(x => x.LeadRequest != null, () =>
        {
            RuleFor(x => x.LeadRequest.Reason)
                .NotEmpty().MaximumLength(2000);
        });
    }
}
