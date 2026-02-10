using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.OpportunityStage.Queries.GetOpportunityStageById;

public class GetOpportunityStageByIdQueryValidator : AbstractValidator<GetOpportunityStageByIdQuery>
{
    public GetOpportunityStageByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
