using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.OpportunityStage.Commands.UpdateOpportunityStage;

public class UpdateOpportunityStageCommandValidator : AbstractValidator<UpdateOpportunityStageCommand>
{
    public UpdateOpportunityStageCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.StageRequest).NotNull();

        When(x => x.StageRequest != null, () =>
        {
            RuleFor(x => x.StageRequest.Id).GreaterThan(0);
            RuleFor(x => x.StageRequest.Name)
                .NotEmpty().MaximumLength(100);
            RuleFor(x => x.StageRequest.Order)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.StageRequest.DefaultProbability)
                .InclusiveBetween(0, 1);
        });
    }
}
