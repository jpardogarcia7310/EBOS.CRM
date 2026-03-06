using EBOS.CRM.Application.Features.CRM.Quote.Services;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Quote.Services;

public class QuoteOpportunityValidationServiceTest
{
    [Fact]
    public async Task EnsureOpportunityAvailableAsync_WhenOpportunityMissing_ThrowsValidation()
    {
        var repository = new Mock<IOpportunityRepository>();
        repository.Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.Opportunity?)null);

        var service = new QuoteOpportunityValidationService(repository.Object);

        var ex = await Assert.ThrowsAsync<DomainValidationException>(() =>
            service.EnsureOpportunityAvailableAsync(1, 999, CancellationToken.None));
        Assert.Equal("DOMAIN_VALIDATION_QUOTE_OPPORTUNITY_NOT_FOUND", ex.Code);
    }

    [Fact]
    public async Task EnsureOpportunityAvailableAsync_WhenTenantMismatch_ThrowsConflict()
    {
        var repository = new Mock<IOpportunityRepository>();
        repository.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new global::EBOS.CRM.Domain.Entities.CRM.Opportunity { Id = 10, TenantId = 2, Name = "Opp", StageId = 1, OwnerUserId = 1, CustomerId = 1, Amount = 0m, Probability = 0m });

        var service = new QuoteOpportunityValidationService(repository.Object);

        var ex = await Assert.ThrowsAsync<DomainConflictException>(() =>
            service.EnsureOpportunityAvailableAsync(1, 10, CancellationToken.None));
        Assert.Equal("DOMAIN_CONFLICT_QUOTE_OPPORTUNITY_TENANT_MISMATCH", ex.Code);
    }

    [Fact]
    public async Task EnsureOpportunityAvailableAsync_WhenOpportunityDeleted_ThrowsRuleViolation()
    {
        var repository = new Mock<IOpportunityRepository>();
        repository.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new global::EBOS.CRM.Domain.Entities.CRM.Opportunity
            {
                Id = 10,
                TenantId = 1,
                Name = "Opp",
                StageId = 1,
                OwnerUserId = 1,
                CustomerId = 1,
                Amount = 0m,
                Probability = 0m,
                Erased = true
            });

        var service = new QuoteOpportunityValidationService(repository.Object);

        var ex = await Assert.ThrowsAsync<DomainRuleViolationException>(() =>
            service.EnsureOpportunityAvailableAsync(1, 10, CancellationToken.None));
        Assert.Equal("DOMAIN_RULE_QUOTE_OPPORTUNITY_DISABLED", ex.Code);
    }

    [Fact]
    public async Task EnsureOpportunityAvailableAsync_WhenRepositoryTimeout_ThrowsTransientDeterministicCode()
    {
        var repository = new Mock<IOpportunityRepository>();
        repository.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException("opportunity timeout"));

        var service = new QuoteOpportunityValidationService(repository.Object);

        var ex = await Assert.ThrowsAsync<TransientDomainFailureException>(() =>
            service.EnsureOpportunityAvailableAsync(1, 10, CancellationToken.None));
        Assert.Equal("DOMAIN_TRANSIENT_QUOTE_OPPORTUNITY_RESOLUTION", ex.Code);
    }
}
