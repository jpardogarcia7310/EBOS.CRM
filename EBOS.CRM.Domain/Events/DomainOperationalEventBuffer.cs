namespace EBOS.CRM.Domain.Events;

public sealed class DomainOperationalEventBuffer
{
    private readonly List<DomainOperationalEvent> _events = [];

    public IReadOnlyCollection<DomainOperationalEvent> Peek()
        => _events.AsReadOnly();

    public IReadOnlyCollection<DomainOperationalEvent> Dequeue()
    {
        var snapshot = _events.ToArray();
        _events.Clear();
        return snapshot;
    }

    public void Emit(string eventName, IReadOnlyDictionary<string, string>? evidence = null)
    {
        var category = DomainOperationalEventCatalog.Classify(eventName);
        _events.Add(new DomainOperationalEvent(
            Name: eventName,
            Category: category,
            OccurredAtUtc: DateTime.UtcNow,
            Evidence: evidence ?? new Dictionary<string, string>(StringComparer.Ordinal)));
    }
}
