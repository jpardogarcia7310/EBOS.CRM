using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class AccountContactRole : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; set; }
    public long AccountContactId { get; set; }
    public AccountContact AccountContact { get; set; } = null!;
    public string RoleCode { get; set; } = null!;
    public bool IsPrimary { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public void Activate(DateTime validFrom)
    {
        ValidFrom = validFrom;
        ValidTo = null;
    }

    public void Deactivate(DateTime validTo)
    {
        if (validTo < ValidFrom)
        {
            throw new InvalidOperationException("ValidTo cannot be earlier than ValidFrom.");
        }

        ValidTo = validTo;
        IsPrimary = false;
    }

    public void SetPrimary(bool isPrimary)
    {
        if (isPrimary && ValidTo.HasValue)
        {
            throw new InvalidOperationException("Cannot set primary role when role is not active.");
        }

        IsPrimary = isPrimary;
    }
}

