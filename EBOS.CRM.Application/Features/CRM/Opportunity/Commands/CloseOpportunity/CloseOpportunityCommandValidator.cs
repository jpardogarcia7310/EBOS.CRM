using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Opportunity.Commands.CloseOpportunity;

public class CloseOpportunityCommandValidator : AbstractValidator<CloseOpportunityCommand>
{
    public CloseOpportunityCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.OpportunityRequest).NotNull();

        When(x => x.OpportunityRequest != null, () =>
        {
            RuleFor(x => x.OpportunityRequest.StageId).GreaterThan(0);
            RuleFor(x => x.OpportunityRequest.CloseReason)
                .MaximumLength(500);
        });
    }
}
