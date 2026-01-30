namespace EBOS.CRM.Infrastructure.Services.Audit;

public sealed class AuditServiceOptions
{
    public const string SectionName = "AuditService";

    public string BaseUrl { get; set; } = "http://192.168.1.222:5000";
    public bool Enabled { get; set; } = true;
    public int TimeoutSeconds { get; set; } = 10;
    public int RetryCount { get; set; } = 3;
}
