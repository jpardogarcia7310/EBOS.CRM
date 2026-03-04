using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class Lead : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; set; }
    public string Source { get; set; } = null!;
    public string Status { get; set; } = null!;
    public long OwnerUserId { get; set; }
    public string CompanyName { get; set; } = null!;
    public string ContactName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public decimal? EstimatedValue { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    public long? ConvertedOpportunityId { get; set; }
    public Opportunity? ConvertedOpportunity { get; set; }
}
