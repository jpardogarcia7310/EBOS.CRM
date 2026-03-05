using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Domain.Entities.CRM;

public class AccountHierarchy : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; private set; }
    public long ParentCorporateCustomerId { get; private set; }
    public CorporateCustomer ParentCorporateCustomer { get; private set; } = null!;
    public long ChildCorporateCustomerId { get; private set; }
    public CorporateCustomer ChildCorporateCustomer { get; private set; } = null!;
    public string RelationType { get; private set; } = null!;
    public DateTime ValidFrom { get; private set; }
    public DateTime? ValidTo { get; private set; }
    public bool IsCurrent { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();
    long ITenantScopedEntity.TenantId { get => TenantId; set => TenantId = value; }

    private AccountHierarchy()
    {
    }

    public static AccountHierarchy Create(long tenantId, long parentCorporateCustomerId, long childCorporateCustomerId,
        string relationType, DateTime validFrom)
    {
        var entity = new AccountHierarchy
        {
            TenantId = tenantId,
            Erased = false
        };

        entity.AssignParent(parentCorporateCustomerId, childCorporateCustomerId, relationType, validFrom);
        return entity;
    }

    public async Task AssignParentAsync(long tenantId, long parentCorporateCustomerId, long childCorporateCustomerId,
        string relationType, DateTime validFrom, IAccountHierarchyAcyclicInvariant hierarchyInvariant,
        CancellationToken cancellationToken = default)
    {
        if (hierarchyInvariant is null)
        {
            throw new ArgumentNullException(nameof(hierarchyInvariant));
        }

        if (tenantId <= 0)
        {
            throw new DomainValidationException("TenantId must be a positive value.", "DOMAIN_VALIDATION_TENANT_ID_POSITIVE");
        }

        TenantId = tenantId;
        await hierarchyInvariant.EnsureNoCycleAsync(tenantId, parentCorporateCustomerId, childCorporateCustomerId,
            cancellationToken);
        AssignParent(parentCorporateCustomerId, childCorporateCustomerId, relationType, validFrom);
    }

    internal void AssignParent(long parentCorporateCustomerId, long childCorporateCustomerId, string relationType, DateTime validFrom)
    {
        if (parentCorporateCustomerId <= 0)
        {
            throw new DomainValidationException("ParentCorporateCustomerId must be a positive value.", "DOMAIN_VALIDATION_PARENT_CORPORATE_CUSTOMER_ID_POSITIVE");
        }

        if (childCorporateCustomerId <= 0)
        {
            throw new DomainValidationException("ChildCorporateCustomerId must be a positive value.", "DOMAIN_VALIDATION_CHILD_CORPORATE_CUSTOMER_ID_POSITIVE");
        }

        if (parentCorporateCustomerId == childCorporateCustomerId)
        {
            throw new DomainRuleViolationException("ParentCorporateCustomerId cannot be the same as ChildCorporateCustomerId.", "DOMAIN_RULE_VIOLATION_HIERARCHY_SELF_REFERENCE");
        }

        if (string.IsNullOrWhiteSpace(relationType))
        {
            throw new DomainValidationException("RelationType is required.", "DOMAIN_VALIDATION_RELATION_TYPE_REQUIRED");
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
            throw new DomainValidationException("ValidTo cannot be earlier than ValidFrom.", "DOMAIN_VALIDATION_VALID_TO_RANGE");
        }

        ValidTo = validTo;
        IsCurrent = false;
    }
}

