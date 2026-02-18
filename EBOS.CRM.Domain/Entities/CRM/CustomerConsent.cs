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
        if (string.IsNullOrWhiteSpace(source))
        {
            throw new InvalidOperationException("Source is required.");
        }

        if (string.IsNullOrWhiteSpace(consentType))
        {
            throw new InvalidOperationException("ConsentType is required.");
        }

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

    public void Revoke(DateTime revokedAt)
    {
        if (!Granted)
        {
            throw new InvalidOperationException("Consent is not granted.");
        }

        if (revokedAt < GrantedAt)
        {
            throw new InvalidOperationException("RevokedAt cannot be earlier than GrantedAt.");
        }

        RevokedAt = revokedAt;
        Granted = false;
    }
}
