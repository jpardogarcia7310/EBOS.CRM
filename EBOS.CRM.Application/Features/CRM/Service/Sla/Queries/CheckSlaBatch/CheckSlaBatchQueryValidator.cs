using EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.CheckSlaBatch;
using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.CheckSlaBatch;

public sealed class CheckSlaBatchQueryValidator : AbstractValidator<CheckSlaBatchQuery>
{
    public CheckSlaBatchQueryValidator()
    {
        RuleFor(x => x.Request).NotNull();
        When(x => x.Request is not null, () =>
        {
            RuleFor(x => x.Request.TenantId).GreaterThan(0);
            RuleFor(x => x.Request.PageNumber).GreaterThan(0);
            RuleFor(x => x.Request.PageSize).GreaterThan(0).LessThanOrEqualTo(500);
        });
    }
}
