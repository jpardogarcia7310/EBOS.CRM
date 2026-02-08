namespace EBOS.CRM.Application.Options;

public sealed class TenantIsolationOptions
{
    public const string SectionName = "TenantIsolation";

    public int MinTraversalDepth { get; init; } = 1;
    public int MaxTraversalDepth { get; init; } = 50;
    public int TraversalDepth { get; init; } = 10;
}
