using EBOS.Core.Primitives;

namespace EBOS.CRM.Domain.Entities.EBOS;

public class AuditOutboxMessage : BaseEntity
{
    public string Operation { get; set; } = null!;
    public string Payload { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime NextAttemptAt { get; set; }
    public int AttemptCount { get; set; }
    public string? LastError { get; set; }
    public DateTime? ProcessedAt { get; set; }
}
