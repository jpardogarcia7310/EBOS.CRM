using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Service.Queue.Commands.ToggleQueue;

public class ToggleQueueCommandValidator : AbstractValidator<ToggleQueueCommand>
{
    public ToggleQueueCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.QueueRequest).NotNull();

        When(x => x.QueueRequest != null, () =>
        {
            RuleFor(x => x.QueueRequest.TenantId).GreaterThan(0);
        });
    }
}
