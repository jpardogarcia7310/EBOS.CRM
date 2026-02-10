using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Opportunity.Commands.PatchOpportunityStage;

public class PatchOpportunityStageCommandValidator : AbstractValidator<PatchOpportunityStageCommand>
{
    public PatchOpportunityStageCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.OpportunityRequest).NotNull();

        When(x => x.OpportunityRequest != null, () =>
        {
            RuleFor(x => x.OpportunityRequest.StageId).GreaterThan(0);
            RuleFor(x => x.OpportunityRequest.Probability)
                .InclusiveBetween(0, 1)
                .When(x => x.OpportunityRequest.Probability.HasValue);
        });
    }
}
