using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Lead.Commands.QualifyLead;

public class QualifyLeadCommandValidator : AbstractValidator<QualifyLeadCommand>
{
    public QualifyLeadCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.LeadRequest).NotNull();

        When(x => x.LeadRequest != null, () =>
        {
            RuleFor(x => x.LeadRequest.Notes)
                .MaximumLength(2000);
        });
    }
}
