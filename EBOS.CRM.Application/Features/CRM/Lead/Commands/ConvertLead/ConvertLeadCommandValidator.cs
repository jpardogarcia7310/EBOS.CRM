using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Lead.Commands.ConvertLead;

public class ConvertLeadCommandValidator : AbstractValidator<ConvertLeadCommand>
{
    public ConvertLeadCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.LeadRequest).NotNull();

        When(x => x.LeadRequest != null, () =>
        {
            RuleFor(x => x.LeadRequest.CustomerId).GreaterThan(0);
            RuleFor(x => x.LeadRequest.StageId).GreaterThan(0);
            RuleFor(x => x.LeadRequest.OpportunityName)
                .NotEmpty().MaximumLength(200);
            RuleFor(x => x.LeadRequest.Amount)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.LeadRequest.Probability)
                .InclusiveBetween(0, 1);
            RuleFor(x => x.LeadRequest.Notes)
                .MaximumLength(2000);
        });
    }
}
