using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class TenantUsageMetric : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; set; }
    public string Metric { get; set; } = null!;
    public decimal Value { get; set; }
    public string? Unit { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public string? Source { get; set; }
}
