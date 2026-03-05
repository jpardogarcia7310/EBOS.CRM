using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class CustomerMergeHistory : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; private set; }
    public long WinnerCustomerId { get; private set; }
    public long MergedCustomerId { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public long CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }

    long ITenantScopedEntity.TenantId
    {
        get => TenantId;
        set => TenantId = value;
    }

    private CustomerMergeHistory()
    {
    }

    public static CustomerMergeHistory Create(long tenantId, long winnerCustomerId, long mergedCustomerId,
        string reason, long createdBy, DateTime? createdAt = null)
    {
        if (tenantId <= 0)
        {
            throw new DomainValidationException("TenantId must be a positive value.", "DOMAIN_VALIDATION_TENANT_ID_POSITIVE");
        }

        if (winnerCustomerId <= 0 || mergedCustomerId <= 0)
        {
            throw new DomainValidationException("Customer ids must be positive values.", "DOMAIN_VALIDATION_CUSTOMER_IDS_POSITIVE");
        }

        if (winnerCustomerId == mergedCustomerId)
        {
            throw new DomainRuleViolationException("WinnerCustomerId and MergedCustomerId must be different.", "DOMAIN_RULE_VIOLATION_MERGE_SAME_CUSTOMER");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new DomainValidationException("Reason is required.", "DOMAIN_VALIDATION_MERGE_REASON_REQUIRED");
        }

        if (createdBy <= 0)
        {
            throw new DomainValidationException("CreatedBy must be a positive value.", "DOMAIN_VALIDATION_CREATED_BY_POSITIVE");
        }

        return new CustomerMergeHistory
        {
            TenantId = tenantId,
            WinnerCustomerId = winnerCustomerId,
            MergedCustomerId = mergedCustomerId,
            Reason = reason.Trim(),
            CreatedBy = createdBy,
            CreatedAt = createdAt ?? DateTime.UtcNow
        };
    }

    public bool MatchesMergeIntent(long tenantId, long winnerCustomerId, long mergedCustomerId)
    {
        return TenantId == tenantId
               && WinnerCustomerId == winnerCustomerId
               && MergedCustomerId == mergedCustomerId;
    }
}
