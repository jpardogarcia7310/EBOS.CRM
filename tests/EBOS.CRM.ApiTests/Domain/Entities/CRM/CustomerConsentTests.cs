using EBOS.CRM.Domain.Entities.CRM;
using FluentAssertions;

namespace EBOS.CRM.ApiTests.Domain.Entities.CRM;

public class CustomerConsentTests
{
    [Fact]
    public void Revoke_ShouldThrow_WhenConsentIsAppendOnly()
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

        var act = () => consent.Revoke(revokedAt);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*append-only*");
    }

    [Fact]
    public void CreateRevoked_ShouldCreateExplicitRevocationEvent()
    {
        var revokedAt = new DateTime(2025, 12, 2, 12, 0, 0, DateTimeKind.Utc);

        var consent = CustomerConsent.CreateRevoked(
            tenantId: 10,
            customerId: 20,
            consentType: "marketing",
            revokedAt: revokedAt,
            source: "portal",
            expiresAt: revokedAt);

        consent.Granted.Should().BeFalse();
        consent.GrantedAt.Should().Be(revokedAt);
        consent.RevokedAt.Should().Be(revokedAt);
        consent.ExpiresAt.Should().Be(revokedAt);
    }
}
