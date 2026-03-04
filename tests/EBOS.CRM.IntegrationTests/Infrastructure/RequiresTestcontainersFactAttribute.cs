namespace EBOS.CRM.IntegrationTests.Infrastructure;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class RequiresTestcontainersFactAttribute : FactAttribute
{
    public RequiresTestcontainersFactAttribute()
    {
        if (!string.Equals(Environment.GetEnvironmentVariable("USE_TESTCONTAINERS"), "true",
                StringComparison.OrdinalIgnoreCase))
        {
            Skip = "Skipped: set USE_TESTCONTAINERS=true to run this SQL Server/Testcontainers test.";
        }
    }
}
