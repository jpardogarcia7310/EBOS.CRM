using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class CustomerPreference : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; set; }
    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public long ChannelId { get; set; }
    public ChannelType Channel { get; set; } = null!;
    public bool Preferred { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long UpdatedBy { get; set; }

    public void UpdatePreference(bool preferred, DateTime updatedAt, long updatedBy)
    {
        Preferred = preferred;
        UpdatedAt = updatedAt;
        UpdatedBy = updatedBy;
    }
}
