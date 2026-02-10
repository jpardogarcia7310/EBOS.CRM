using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Opportunity.Commands.UpdateOpportunity;

public class UpdateOpportunityCommandValidator : AbstractValidator<UpdateOpportunityCommand>
{
    public UpdateOpportunityCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.OpportunityRequest).NotNull();

        When(x => x.OpportunityRequest != null, () =>
        {
            RuleFor(x => x.OpportunityRequest.Id).GreaterThan(0);
            RuleFor(x => x.OpportunityRequest.Name)
                .NotEmpty().MaximumLength(200);
            RuleFor(x => x.OpportunityRequest.StageId)
                .GreaterThan(0);
            RuleFor(x => x.OpportunityRequest.OwnerUserId)
                .GreaterThan(0);
            RuleFor(x => x.OpportunityRequest.CustomerId)
                .GreaterThan(0);
            RuleFor(x => x.OpportunityRequest.Amount)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.OpportunityRequest.Probability)
                .InclusiveBetween(0, 1);
            RuleFor(x => x.OpportunityRequest.Source)
                .MaximumLength(100);
            RuleFor(x => x.OpportunityRequest.CloseReason)
                .MaximumLength(500);
        });
    }
}
