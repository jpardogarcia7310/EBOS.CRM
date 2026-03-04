using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.OpportunityStage.Commands.AddOpportunityStage;

public class AddOpportunityStageCommandValidator : AbstractValidator<AddOpportunityStageCommand>
{
    public AddOpportunityStageCommandValidator()
    {
        RuleFor(x => x.StageRequest).NotNull();

        When(x => x.StageRequest != null, () =>
        {
            RuleFor(x => x.StageRequest.Name)
                .NotEmpty().MaximumLength(100);
            RuleFor(x => x.StageRequest.Order)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.StageRequest.DefaultProbability)
                .InclusiveBetween(0, 1);
        });
    }
}
