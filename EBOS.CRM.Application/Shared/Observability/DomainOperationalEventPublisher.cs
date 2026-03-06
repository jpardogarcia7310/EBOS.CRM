using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Events;
using EBOS.CRM.Domain.Interfaces.Services;

namespace EBOS.CRM.Application.Shared.Observability;

public sealed class DomainOperationalEventPublisher(
    IAuditService auditService,
    ICurrentUserContext currentUser) : IDomainOperationalEventPublisher
{
    public async Task PublishAsync(
        string aggregate,
        long registerId,
        IReadOnlyCollection<DomainOperationalEvent> events,
        CancellationToken cancellationToken)
    {
        if (events.Count == 0)
        {
            return;
        }

        foreach (var domainEvent in events)
        {
            var payload = new
            {
                domainEvent.Name,
                category = domainEvent.Category.ToString(),
                domainEvent.OccurredAtUtc,
                domainEvent.Evidence
            };

            await auditService.InsertAuditAsync(new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Update,
                Entity: $"{aggregate}.DomainOperationalEvent",
                RegisterId: registerId,
                OldValues: null,
                NewValues: AuditSerialization.Serialize(payload),
                CorrelationId: currentUser.CorrelationId), cancellationToken);
        }
    }
}
