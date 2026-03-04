using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Service.Sla.Commands.ToggleSla;

public class ToggleSlaCommandValidator : AbstractValidator<ToggleSlaCommand>
{
    public ToggleSlaCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.SlaRequest).NotNull();

        When(x => x.SlaRequest != null, () =>
        {
            RuleFor(x => x.SlaRequest.TenantId).GreaterThan(0);
        });
    }
}
