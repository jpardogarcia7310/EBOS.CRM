using EBOS.Core.Primitives;
using EBOS.CRM.Domain.Exceptions;
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
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

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
        throw new DomainRuleViolationException(
            "CustomerConsent is append-only. Use CreateRevoked to register a revocation event.",
            "DOMAIN_RULE_VIOLATION_CUSTOMER_CONSENT_APPEND_ONLY");
    }

    public void AssignCustomer(long customerId)
    {
        if (customerId <= 0)
        {
            throw new DomainValidationException("CustomerId must be a positive value.", "DOMAIN_VALIDATION_CUSTOMER_ID_POSITIVE");
        }

        CustomerId = customerId;
    }

    private static void ValidateCommonInputs(long tenantId, long customerId, string consentType, string source)
    {
        if (tenantId <= 0)
        {
            throw new DomainValidationException("TenantId must be a positive value.", "DOMAIN_VALIDATION_TENANT_ID_POSITIVE");
        }

        if (customerId <= 0)
        {
            throw new DomainValidationException("CustomerId must be a positive value.", "DOMAIN_VALIDATION_CUSTOMER_ID_POSITIVE");
        }

        if (string.IsNullOrWhiteSpace(consentType))
        {
            throw new DomainValidationException("ConsentType is required.", "DOMAIN_VALIDATION_CONSENT_TYPE_REQUIRED");
        }

        if (string.IsNullOrWhiteSpace(source))
        {
            throw new DomainValidationException("Source is required.", "DOMAIN_VALIDATION_SOURCE_REQUIRED");
        }
    }

    private static void ValidateGrantEvent(bool granted, DateTime grantedAt, DateTime? expiresAt)
    {
        if (expiresAt.HasValue && expiresAt.Value < grantedAt)
        {
            throw new DomainValidationException("ExpiresAt cannot be earlier than GrantedAt.", "DOMAIN_VALIDATION_EXPIRES_AT_RANGE");
        }

        if (!granted)
        {
            if (!expiresAt.HasValue)
            {
                throw new DomainValidationException("ExpiresAt is required when Granted is false.", "DOMAIN_VALIDATION_EXPIRES_AT_REQUIRED_WHEN_NOT_GRANTED");
            }

            if (expiresAt.Value != grantedAt)
            {
                throw new DomainValidationException("ExpiresAt must match GrantedAt when Granted is false.", "DOMAIN_VALIDATION_EXPIRES_AT_MATCH_GRANTED_AT");
            }
        }
    }

    private static void ValidateRevocationEvent(DateTime revokedAt, DateTime? expiresAt)
    {
        if (!expiresAt.HasValue)
        {
            throw new DomainValidationException("ExpiresAt is required for revocation events.", "DOMAIN_VALIDATION_EXPIRES_AT_REQUIRED_FOR_REVOCATION");
        }

        if (expiresAt.Value != revokedAt)
        {
            throw new DomainValidationException("ExpiresAt must match RevokedAt for revocation events.", "DOMAIN_VALIDATION_EXPIRES_AT_MATCH_REVOKED_AT");
        }
    }
}

