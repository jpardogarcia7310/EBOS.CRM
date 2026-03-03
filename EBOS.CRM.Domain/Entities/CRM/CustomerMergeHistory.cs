using EBOS.Core.Primitives;
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
            throw new InvalidOperationException("TenantId must be a positive value.");
        }

        if (winnerCustomerId <= 0 || mergedCustomerId <= 0)
        {
            throw new InvalidOperationException("Customer ids must be positive values.");
        }

        if (winnerCustomerId == mergedCustomerId)
        {
            throw new InvalidOperationException("WinnerCustomerId and MergedCustomerId must be different.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new InvalidOperationException("Reason is required.");
        }

        if (createdBy <= 0)
        {
            throw new InvalidOperationException("CreatedBy must be a positive value.");
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
}
