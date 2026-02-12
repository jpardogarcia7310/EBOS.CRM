using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;

namespace EBOS.CRM.Domain.Entities.CRM;

public class CustomerConsent : ErasableEntity, ITenantScopedEntity
{
    public long TenantId { get; set; }
    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public string ConsentType { get; set; } = null!;
    public bool Granted { get; set; }
    public DateTime GrantedAt { get; set; }
    public string Source { get; set; } = null!;
    public DateTime? ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public void Grant(DateTime grantedAt, string source, DateTime? expiresAt)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            throw new InvalidOperationException("Source is required.");
        }

        Granted = true;
        GrantedAt = grantedAt;
        Source = source;
        ExpiresAt = expiresAt;
        RevokedAt = null;
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
