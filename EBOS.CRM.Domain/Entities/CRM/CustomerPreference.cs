using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class CustomerPreference : ErasableEntity, ITenantScopedEntity
{
    public const string ChannelEmail = "Email";
    public const string ChannelSms = "SMS";
    public const string ChannelPhone = "Phone";

    public long TenantId { get; set; }
    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public string Channel { get; set; } = null!;
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
