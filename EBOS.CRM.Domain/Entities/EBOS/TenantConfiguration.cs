using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.EBOS;

public class TenantConfiguration : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; set; }
    public string Key { get; set; } = null!;
    public string ValueJson { get; set; } = null!;
    public DateTime UpdatedAt { get; set; }
    public long UpdatedBy { get; set; }
}
