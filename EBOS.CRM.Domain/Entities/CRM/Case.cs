using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class Case : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Status { get; set; } = null!;
    public string Priority { get; set; } = null!;
    public long OwnerUserId { get; set; }
    public long QueueId { get; set; }
    public Queue Queue { get; set; } = null!;
    public long SlaId { get; set; }
    public Sla Sla { get; set; } = null!;
    public DateTime? DueAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }
}
