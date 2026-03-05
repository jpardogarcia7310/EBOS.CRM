namespace EBOS.CRM.Domain.Events;

public sealed record DomainOperationalEvent(
    string Name,
    DomainOperationalEventCategory Category,
    DateTime OccurredAtUtc,
    IReadOnlyDictionary<string, string> Evidence);
