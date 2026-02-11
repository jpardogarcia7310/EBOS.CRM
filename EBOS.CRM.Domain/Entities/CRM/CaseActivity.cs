using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class CaseActivity : ErasableEntity, ITenantScopedEntity
{
    public const string StatusOpen = "Open";
    public const string StatusInProgress = "InProgress";
    public const string StatusCompleted = "Completed";
    public const string StatusCancelled = "Cancelled";

    public long TenantId { get; set; }
    public long CaseId { get; set; }
    public Case Case { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }
}
