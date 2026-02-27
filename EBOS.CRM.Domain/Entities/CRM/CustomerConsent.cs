using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class CustomerConsent : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; set; }
    public long CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;
    public string ConsentType { get; private set; } = null!;
    public bool Granted { get; private set; }
    public DateTime GrantedAt { get; private set; }
    public string Source { get; private set; } = null!;
    public DateTime? ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    private CustomerConsent()
    {
    }

    public static CustomerConsent Create(long tenantId, long customerId, string consentType, bool granted,
        DateTime grantedAt, string source, DateTime? expiresAt)
    {
        ValidateCommonInputs(tenantId, customerId, consentType, source);
        ValidateGrantEvent(granted, grantedAt, expiresAt);

        return new CustomerConsent
        {
            TenantId = tenantId,
            CustomerId = customerId,
            ConsentType = consentType,
            Granted = granted,
            GrantedAt = grantedAt,
            Source = source,
            ExpiresAt = expiresAt,
            RevokedAt = null
        };
    }

    public static CustomerConsent CreateRevoked(long tenantId, long customerId, string consentType, DateTime revokedAt,
        string source, DateTime? expiresAt)
    {
        ValidateCommonInputs(tenantId, customerId, consentType, source);
        ValidateRevocationEvent(revokedAt, expiresAt);

        return new CustomerConsent
        {
            TenantId = tenantId,
            CustomerId = customerId,
            ConsentType = consentType,
            Granted = false,
            GrantedAt = revokedAt,
            Source = source,
            ExpiresAt = expiresAt,
            RevokedAt = revokedAt
        };
    }

    public void Revoke(DateTime revokedAt)
    {
        throw new InvalidOperationException(
            "CustomerConsent is append-only. Use CreateRevoked to register a revocation event.");
    }

    public void AssignCustomer(long customerId)
    {
        if (customerId <= 0)
        {
            throw new InvalidOperationException("CustomerId must be a positive value.");
        }

        CustomerId = customerId;
    }

    private static void ValidateCommonInputs(long tenantId, long customerId, string consentType, string source)
    {
        if (tenantId <= 0)
        {
            throw new InvalidOperationException("TenantId must be a positive value.");
        }

        if (customerId <= 0)
        {
            throw new InvalidOperationException("CustomerId must be a positive value.");
        }

        if (string.IsNullOrWhiteSpace(consentType))
        {
            throw new InvalidOperationException("ConsentType is required.");
        }

        if (string.IsNullOrWhiteSpace(source))
        {
            throw new InvalidOperationException("Source is required.");
        }
    }

    private static void ValidateGrantEvent(bool granted, DateTime grantedAt, DateTime? expiresAt)
    {
        if (expiresAt.HasValue && expiresAt.Value < grantedAt)
        {
            throw new InvalidOperationException("ExpiresAt cannot be earlier than GrantedAt.");
        }

        if (!granted)
        {
            if (!expiresAt.HasValue)
            {
                throw new InvalidOperationException("ExpiresAt is required when Granted is false.");
            }

            if (expiresAt.Value != grantedAt)
            {
                throw new InvalidOperationException("ExpiresAt must match GrantedAt when Granted is false.");
            }
        }
    }

    private static void ValidateRevocationEvent(DateTime revokedAt, DateTime? expiresAt)
    {
        if (!expiresAt.HasValue)
        {
            throw new InvalidOperationException("ExpiresAt is required for revocation events.");
        }

        if (expiresAt.Value != revokedAt)
        {
            throw new InvalidOperationException("ExpiresAt must match RevokedAt for revocation events.");
        }
    }
}
