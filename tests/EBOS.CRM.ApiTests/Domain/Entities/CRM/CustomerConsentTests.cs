using EBOS.CRM.Domain.Entities.CRM;
using FluentAssertions;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class CustomerConsentTests
{
    [Fact]
    public void Revoke_ShouldOnlyChange_GrantedAndRevokedAt()
    {
        var grantedAt = new DateTime(2025, 12, 1, 10, 30, 0, DateTimeKind.Utc);
        var revokedAt = new DateTime(2025, 12, 2, 12, 0, 0, DateTimeKind.Utc);
        var expiresAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var consent = CustomerConsent.Create(
            tenantId: 10,
            customerId: 20,
            consentType: "marketing",
            granted: true,
            grantedAt: grantedAt,
            source: "portal",
            expiresAt: expiresAt);

        var beforeTenantId = consent.TenantId;
        var beforeCustomerId = consent.CustomerId;
        var beforeConsentType = consent.ConsentType;
        var beforeGrantedAt = consent.GrantedAt;
        var beforeSource = consent.Source;
        var beforeExpiresAt = consent.ExpiresAt;

        consent.Revoke(revokedAt);

        consent.Granted.Should().BeFalse();
        consent.RevokedAt.Should().Be(revokedAt);

        consent.TenantId.Should().Be(beforeTenantId);
        consent.CustomerId.Should().Be(beforeCustomerId);
        consent.ConsentType.Should().Be(beforeConsentType);
        consent.GrantedAt.Should().Be(beforeGrantedAt);
        consent.Source.Should().Be(beforeSource);
        consent.ExpiresAt.Should().Be(beforeExpiresAt);
    }
}
