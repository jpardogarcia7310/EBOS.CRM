using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Domain.Entities.CRM;

public class AccountHierarchy : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; set; }
    public long ParentCorporateCustomerId { get; set; }
    public CorporateCustomer ParentCorporateCustomer { get; set; } = null!;
    public long ChildCorporateCustomerId { get; set; }
    public CorporateCustomer ChildCorporateCustomer { get; set; } = null!;
    public string RelationType { get; set; } = null!;
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public bool IsCurrent { get; set; }

    public async Task AssignParentAsync(long tenantId, long parentCorporateCustomerId, long childCorporateCustomerId,
        string relationType, DateTime validFrom, IAccountHierarchyAcyclicInvariant hierarchyInvariant,
        CancellationToken cancellationToken = default)
    {
        if (hierarchyInvariant is null)
        {
            throw new ArgumentNullException(nameof(hierarchyInvariant));
        }

        await hierarchyInvariant.EnsureNoCycleAsync(tenantId, parentCorporateCustomerId, childCorporateCustomerId,
            cancellationToken);
        AssignParent(parentCorporateCustomerId, childCorporateCustomerId, relationType, validFrom);
    }

    internal void AssignParent(long parentCorporateCustomerId, long childCorporateCustomerId, string relationType, DateTime validFrom)
    {
        if (parentCorporateCustomerId <= 0)
        {
            throw new InvalidOperationException("ParentCorporateCustomerId must be a positive value.");
        }

        if (childCorporateCustomerId <= 0)
        {
            throw new InvalidOperationException("ChildCorporateCustomerId must be a positive value.");
        }

        if (parentCorporateCustomerId == childCorporateCustomerId)
        {
            throw new InvalidOperationException("ParentCorporateCustomerId cannot be the same as ChildCorporateCustomerId.");
        }

        if (string.IsNullOrWhiteSpace(relationType))
        {
            throw new InvalidOperationException("RelationType is required.");
        }

        ParentCorporateCustomerId = parentCorporateCustomerId;
        ChildCorporateCustomerId = childCorporateCustomerId;
        RelationType = relationType;
        ValidFrom = validFrom;
        ValidTo = null;
        IsCurrent = true;
    }

    public void EndRelation(DateTime validTo)
    {
        if (validTo < ValidFrom)
        {
            throw new InvalidOperationException("ValidTo cannot be earlier than ValidFrom.");
        }

        ValidTo = validTo;
        IsCurrent = false;
    }
}
