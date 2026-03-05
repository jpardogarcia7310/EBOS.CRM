using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Exceptions;

namespace EBOS.CRM.ApiTests.Domain;

public class DomainRuleViolationInvariantTests
{
    [Fact]
    public void Case_StatusTransition_WithExplicitPrecondition_ThrowsDomainRuleViolation()
    {
        var entity = new Case
        {
            TenantId = 1,
            Title = "Case",
            Status = Case.StatusOpen,
            Priority = Case.PriorityLow,
            OwnerUserId = 1,
            QueueId = 1,
            SlaId = 1,
            CreatedAt = DateTime.UtcNow
        };

        var ex = Assert.Throws<DomainRuleViolationException>(() => entity.SetStatus(Case.StatusReopened));
        Assert.Equal(DomainErrorTaxonomyType.DomainRuleViolation, ex.TaxonomyType);
        Assert.Equal("DOMAIN_RULE_VIOLATION_CASE_STATUS_TRANSITION", ex.Code);
    }

    [Fact]
    public void CustomerPrivacyRequest_CompleteWithoutInProgress_WithExplicitPrecondition_ThrowsDomainRuleViolation()
    {
        var request = CustomerPrivacyRequest.Create(
            tenantId: 1,
            customerId: 10,
            requestType: CustomerPrivacyRequest.TypeAnonymize,
            requestedBy: 12,
            reason: "gdpr",
            correlationId: "corr");

        var ex = Assert.Throws<DomainRuleViolationException>(() => request.MarkCompleted(processedBy: 12));
        Assert.Equal(DomainErrorTaxonomyType.DomainRuleViolation, ex.TaxonomyType);
        Assert.Equal("DOMAIN_RULE_VIOLATION_PRIVACY_REQUEST_TRANSITION_COMPLETED", ex.Code);
    }

    [Fact]
    public void CustomerConsent_AppendOnlyInvariant_WithExplicitPrecondition_ThrowsDomainRuleViolation()
    {
        var consent = CustomerConsent.Create(
            tenantId: 1,
            customerId: 10,
            consentType: "MARKETING_EMAIL",
            granted: true,
            grantedAt: DateTime.UtcNow,
            source: "portal",
            expiresAt: DateTime.UtcNow.AddDays(10));

        var ex = Assert.Throws<DomainRuleViolationException>(() => consent.Revoke(DateTime.UtcNow));
        Assert.Equal(DomainErrorTaxonomyType.DomainRuleViolation, ex.TaxonomyType);
        Assert.Equal("DOMAIN_RULE_VIOLATION_CUSTOMER_CONSENT_APPEND_ONLY", ex.Code);
    }
}
