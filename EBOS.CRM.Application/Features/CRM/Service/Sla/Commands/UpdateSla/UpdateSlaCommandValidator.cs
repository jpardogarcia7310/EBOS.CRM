using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Service.Sla.Commands.UpdateSla;

public class UpdateSlaCommandValidator : AbstractValidator<UpdateSlaCommand>
{
    public UpdateSlaCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.SlaRequest).NotNull();

        When(x => x.SlaRequest != null, () =>
        {
            RuleFor(x => x.SlaRequest.Id).GreaterThan(0);
            RuleFor(x => x.SlaRequest.TenantId).GreaterThan(0);
            RuleFor(x => x.SlaRequest.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.SlaRequest.TargetMinutes).GreaterThan(0);
            RuleFor(x => x.SlaRequest.WarningMinutes)
                .GreaterThanOrEqualTo(0).When(x => x.SlaRequest.WarningMinutes.HasValue);
        });
    }
}
