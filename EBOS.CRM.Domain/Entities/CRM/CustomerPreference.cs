using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class CustomerPreference : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; private set; }
    public long CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;
    public long ChannelId { get; private set; }
    public ChannelType Channel { get; private set; } = null!;
    public bool Preferred { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public long UpdatedBy { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    long ITenantScopedEntity.TenantId
    {
        get => TenantId;
        set => TenantId = value;
    }

    private CustomerPreference()
    {
    }

    public static CustomerPreference Create(long tenantId, long customerId, long channelId, bool preferred,
        DateTime updatedAt, long updatedBy)
    {
        if (tenantId <= 0)
        {
            throw new InvalidOperationException("TenantId must be a positive value.");
        }

        if (customerId <= 0)
        {
            throw new InvalidOperationException("CustomerId must be a positive value.");
        }

        if (channelId <= 0)
        {
            throw new InvalidOperationException("ChannelId must be a positive value.");
        }

        if (updatedBy <= 0)
        {
            throw new InvalidOperationException("UpdatedBy must be a positive value.");
        }

        return new CustomerPreference
        {
            TenantId = tenantId,
            CustomerId = customerId,
            ChannelId = channelId,
            Preferred = preferred,
            UpdatedAt = updatedAt,
            UpdatedBy = updatedBy
        };
    }

    public void UpdatePreference(bool preferred, DateTime updatedAt, long updatedBy)
    {
        if (updatedBy <= 0)
        {
            throw new InvalidOperationException("UpdatedBy must be a positive value.");
        }

        Preferred = preferred;
        UpdatedAt = updatedAt;
        UpdatedBy = updatedBy;
    }

    public void ReassignCustomer(long customerId)
    {
        if (customerId <= 0)
        {
            throw new InvalidOperationException("CustomerId must be a positive value.");
        }

        if (customerId == CustomerId)
        {
            return;
        }

        CustomerId = customerId;
    }

    public void MergeFrom(CustomerPreference source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        UpdatePreference(source.Preferred, source.UpdatedAt, source.UpdatedBy);
    }
}

