using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class AccountContact : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; set; }
    public long CorporateCustomerId { get; set; }
    public CorporateCustomer CorporateCustomer { get; set; } = null!;
    public long IndividualCustomerId { get; set; }
    public IndividualCustomer IndividualCustomer { get; set; } = null!;
    public bool IsPrimary { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }

    public ICollection<AccountContactRole> Roles { get; set; } = new List<AccountContactRole>();

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
        IsPrimary = isPrimary;
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
