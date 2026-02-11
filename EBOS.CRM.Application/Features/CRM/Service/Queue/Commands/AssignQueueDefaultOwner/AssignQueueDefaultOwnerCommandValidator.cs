using FluentValidation;

namespace EBOS.CRM.Application.Features.CRM.Service.Queue.Commands.AssignQueueDefaultOwner;

public class AssignQueueDefaultOwnerCommandValidator : AbstractValidator<AssignQueueDefaultOwnerCommand>
{
    public AssignQueueDefaultOwnerCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.QueueRequest).NotNull();

        When(x => x.QueueRequest != null, () =>
        {
            RuleFor(x => x.QueueRequest.TenantId).GreaterThan(0);
        });
    }
}
