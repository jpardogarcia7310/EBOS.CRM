using EBOS.CRM.Domain.Events;

namespace EBOS.CRM.Application.Shared.Observability;

public interface IDomainOperationalEventPublisher
{
    Task PublishAsync(
        string aggregate,
        long registerId,
        IReadOnlyCollection<DomainOperationalEvent> events,
        CancellationToken cancellationToken);
}
