namespace EBOS.CRM.Domain.Events;

public sealed record DomainOperationalEventDefinition(
    string Name,
    DomainOperationalEventCategory Category,
    string Description,
    string AnalyticsUsage);
