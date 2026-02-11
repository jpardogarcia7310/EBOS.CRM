using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Service.Queue.Commands.AddQueue;

public class AddQueueCommandValidator : AbstractValidator<AddQueueCommand>
{
    public AddQueueCommandValidator()
    {
        RuleFor(x => x.QueueRequest).NotNull();

        When(x => x.QueueRequest != null, () =>
        {
            RuleFor(x => x.QueueRequest.TenantId).GreaterThan(0);
            RuleFor(x => x.QueueRequest.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.QueueRequest.Code).NotEmpty().MaximumLength(50);
        });
    }
}
