using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.CheckCaseSla;

public class CheckCaseSlaQueryValidator : AbstractValidator<CheckCaseSlaQuery>
{
    public CheckCaseSlaQueryValidator()
    {
        RuleFor(x => x.SlaRequest).NotNull();

        When(x => x.SlaRequest != null, () =>
        {
            RuleFor(x => x.SlaRequest.TenantId).GreaterThan(0);
            RuleFor(x => x.SlaRequest.CaseId).GreaterThan(0);
        });
    }
}
