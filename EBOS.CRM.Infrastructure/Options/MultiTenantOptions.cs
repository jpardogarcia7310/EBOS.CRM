namespace EBOS.CRM.Infrastructure.Options;

public sealed class MultiTenantOptions
{
    public const string SectionName = "MultiTenant";

    public MultiTenantStrategy Strategy { get; init; } = MultiTenantStrategy.Shared;
    public string SchemaPrefix { get; init; } = "Tenant_";
    public string[] SchemaTargets { get; init; } = ["CRM"];
    public string? ConnectionStringTemplate { get; init; }
}

public enum MultiTenantStrategy
{
    Shared,
    Schema,
    Database
}
