using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Exceptions;
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
            throw new DomainValidationException("TenantId must be a positive value.", "DOMAIN_VALIDATION_TENANT_ID_POSITIVE");
        }

        if (customerId <= 0)
        {
            throw new DomainValidationException("CustomerId must be a positive value.", "DOMAIN_VALIDATION_CUSTOMER_ID_POSITIVE");
        }

        if (channelId <= 0)
        {
            throw new DomainValidationException("ChannelId must be a positive value.", "DOMAIN_VALIDATION_CHANNEL_ID_POSITIVE");
        }

        if (updatedBy <= 0)
        {
            throw new DomainValidationException("UpdatedBy must be a positive value.", "DOMAIN_VALIDATION_UPDATED_BY_POSITIVE");
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
            throw new DomainValidationException("UpdatedBy must be a positive value.", "DOMAIN_VALIDATION_UPDATED_BY_POSITIVE");
        }

        Preferred = preferred;
        UpdatedAt = updatedAt;
        UpdatedBy = updatedBy;
    }

    public void ReassignCustomer(long customerId)
    {
        if (customerId <= 0)
        {
            throw new DomainValidationException("CustomerId must be a positive value.", "DOMAIN_VALIDATION_CUSTOMER_ID_POSITIVE");
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

