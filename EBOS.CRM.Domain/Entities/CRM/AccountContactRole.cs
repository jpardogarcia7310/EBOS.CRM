using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class AccountContactRole : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; private set; }
    public long AccountContactId { get; private set; }
    public AccountContact AccountContact { get; private set; } = null!;
    public string RoleCode { get; private set; } = null!;
    public bool IsPrimary { get; private set; }
    public DateTime ValidFrom { get; private set; }
    public DateTime? ValidTo { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();
    long ITenantScopedEntity.TenantId { get => TenantId; set => TenantId = value; }

    private AccountContactRole()
    {
    }

    public static AccountContactRole Create(long tenantId, long accountContactId, string roleCode, bool isPrimary,
        DateTime validFrom, DateTime? validTo)
    {
        ValidateIdentity(tenantId, accountContactId, roleCode);

        var entity = new AccountContactRole
        {
            TenantId = tenantId,
            AccountContactId = accountContactId,
            RoleCode = roleCode.Trim().ToUpperInvariant(),
            Erased = false
        };

        entity.Activate(validFrom);
        if (validTo.HasValue)
        {
            entity.Deactivate(validTo.Value);
        }

        entity.SetPrimary(isPrimary);
        return entity;
    }

    public void Update(long tenantId, long accountContactId, string roleCode, bool isPrimary, DateTime validFrom,
        DateTime? validTo)
    {
        ValidateIdentity(tenantId, accountContactId, roleCode);

        TenantId = tenantId;
        AccountContactId = accountContactId;
        RoleCode = roleCode.Trim().ToUpperInvariant();

        Activate(validFrom);
        if (validTo.HasValue)
        {
            Deactivate(validTo.Value);
        }

        SetPrimary(isPrimary);
    }

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

    public void ReassignAccountContact(long accountContactId)
    {
        if (accountContactId <= 0)
        {
            throw new InvalidOperationException("AccountContactId must be a positive value.");
        }

        AccountContactId = accountContactId;
    }

    private static void ValidateIdentity(long tenantId, long accountContactId, string roleCode)
    {
        if (tenantId <= 0)
        {
            throw new InvalidOperationException("TenantId must be a positive value.");
        }

        if (accountContactId <= 0)
        {
            throw new InvalidOperationException("AccountContactId must be a positive value.");
        }

        if (string.IsNullOrWhiteSpace(roleCode))
        {
            throw new InvalidOperationException("RoleCode is required.");
        }
    }
}

