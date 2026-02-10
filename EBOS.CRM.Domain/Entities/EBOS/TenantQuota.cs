using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class TenantQuota : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; set; }
    public string Metric { get; set; } = null!;
    public decimal Limit { get; set; }
    public string? Unit { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}
