using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class AccountContact : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; private set; }
    public long CorporateCustomerId { get; private set; }
    public CorporateCustomer CorporateCustomer { get; private set; } = null!;
    public long IndividualCustomerId { get; private set; }
    public IndividualCustomer IndividualCustomer { get; private set; } = null!;
    public bool IsPrimary { get; private set; }
    public DateTime StartAt { get; private set; }
    public DateTime? EndAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public long CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public long? UpdatedBy { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public ICollection<AccountContactRole> Roles { get; private set; } = new List<AccountContactRole>();

    long ITenantScopedEntity.TenantId
    {
        get => TenantId;
        set => TenantId = value;
    }

    private AccountContact()
    {
    }

    public static AccountContact Create(long tenantId, long corporateCustomerId, long individualCustomerId,
        bool isPrimary, DateTime startAt, DateTime? endAt, long createdBy, DateTime? createdAt = null)
    {
        if (tenantId <= 0)
        {
            throw new InvalidOperationException("TenantId must be a positive value.");
        }

        if (createdBy <= 0)
        {
            throw new InvalidOperationException("CreatedBy must be a positive value.");
        }

        var entity = new AccountContact
        {
            TenantId = tenantId,
            CreatedBy = createdBy,
            CreatedAt = createdAt ?? DateTime.UtcNow
        };

        entity.Assign(corporateCustomerId, individualCustomerId, startAt);
        if (endAt.HasValue)
        {
            entity.Unassign(endAt.Value);
        }

        entity.SetPrimary(isPrimary);
        return entity;
    }

    public void Assign(long corporateCustomerId, long individualCustomerId, DateTime startAt)
    {
        if (corporateCustomerId <= 0)
        {
            throw new InvalidOperationException("CorporateCustomerId must be a positive value.");
        }

        if (individualCustomerId <= 0)
        {
            throw new InvalidOperationException("IndividualCustomerId must be a positive value.");
        }

        CorporateCustomerId = corporateCustomerId;
        IndividualCustomerId = individualCustomerId;
        StartAt = startAt;
        EndAt = null;
    }

    public void ReassignCustomers(long corporateCustomerId, long individualCustomerId)
    {
        if (corporateCustomerId <= 0 || individualCustomerId <= 0)
        {
            throw new InvalidOperationException("Customer ids must be positive values.");
        }

        if (EndAt.HasValue)
        {
            throw new InvalidOperationException("Cannot reassign an unassigned account contact. Assign it first.");
        }

        CorporateCustomerId = corporateCustomerId;
        IndividualCustomerId = individualCustomerId;
    }

    public void Unassign(DateTime endAt)
    {
        if (endAt < StartAt)
        {
            throw new InvalidOperationException("EndAt cannot be earlier than StartAt.");
        }

        EndAt = endAt;
        IsPrimary = false;
    }

    public void SetPrimary(bool isPrimary)
    {
        if (isPrimary && EndAt.HasValue)
        {
            throw new InvalidOperationException("Cannot set as primary when account contact is unassigned.");
        }

        IsPrimary = isPrimary;
    }

    public void Touch(long updatedBy, DateTime? updatedAt = null)
    {
        if (updatedBy <= 0)
        {
            throw new InvalidOperationException("UpdatedBy must be a positive value.");
        }

        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
    }

    public void SetPrimaryRole(long accountContactRoleId)
    {
        if (accountContactRoleId <= 0)
        {
            throw new InvalidOperationException("AccountContactRoleId must be a positive value.");
        }

        var target = Roles.FirstOrDefault(r => r.Id == accountContactRoleId);
        if (target is null)
        {
            throw new InvalidOperationException("AccountContactRole not found.");
        }

        foreach (var role in Roles)
        {
            role.SetPrimary(role.Id == accountContactRoleId);
        }
    }
}
