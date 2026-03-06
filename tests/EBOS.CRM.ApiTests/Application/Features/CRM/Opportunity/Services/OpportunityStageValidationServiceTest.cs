using EBOS.CRM.Application.Features.CRM.Opportunity.Services;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Opportunity.Services;

public class OpportunityStageValidationServiceTest
{
    [Fact]
    public async Task EnsureStageAvailableAsync_WhenStageMissing_ThrowsValidation()
    {
        var repository = new Mock<IOpportunityStageRepository>();
        repository.Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.OpportunityStage?)null);

        var service = new OpportunityStageValidationService(repository.Object);

        var ex = await Assert.ThrowsAsync<DomainValidationException>(() =>
            service.EnsureStageAvailableAsync(1, 999, CancellationToken.None));
        Assert.Equal("DOMAIN_VALIDATION_OPPORTUNITY_STAGE_NOT_FOUND", ex.Code);
    }

    [Fact]
    public async Task EnsureStageAvailableAsync_WhenTenantMismatch_ThrowsConflict()
    {
        var repository = new Mock<IOpportunityStageRepository>();
        repository.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new global::EBOS.CRM.Domain.Entities.CRM.OpportunityStage { Id = 10, TenantId = 2, Name = "Qualified" });

        var service = new OpportunityStageValidationService(repository.Object);

        var ex = await Assert.ThrowsAsync<DomainConflictException>(() =>
            service.EnsureStageAvailableAsync(1, 10, CancellationToken.None));
        Assert.Equal("DOMAIN_CONFLICT_OPPORTUNITY_STAGE_TENANT_MISMATCH", ex.Code);
    }

    [Fact]
    public async Task EnsureStageAvailableAsync_WhenStageDisabled_ThrowsRuleViolation()
    {
        var repository = new Mock<IOpportunityStageRepository>();
        repository.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new global::EBOS.CRM.Domain.Entities.CRM.OpportunityStage
            {
                Id = 10,
                TenantId = 1,
                Name = "Qualified",
                Erased = true
            });

        var service = new OpportunityStageValidationService(repository.Object);

        var ex = await Assert.ThrowsAsync<DomainRuleViolationException>(() =>
            service.EnsureStageAvailableAsync(1, 10, CancellationToken.None));
        Assert.Equal("DOMAIN_RULE_OPPORTUNITY_STAGE_DISABLED", ex.Code);
    }

    [Fact]
    public async Task EnsureStageAvailableAsync_WhenRepositoryTimeout_ThrowsTransientDeterministicCode()
    {
        var repository = new Mock<IOpportunityStageRepository>();
        repository.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException("stage lookup timeout"));

        var service = new OpportunityStageValidationService(repository.Object);

        var ex = await Assert.ThrowsAsync<TransientDomainFailureException>(() =>
            service.EnsureStageAvailableAsync(1, 10, CancellationToken.None));
        Assert.Equal("DOMAIN_TRANSIENT_OPPORTUNITY_STAGE_RESOLUTION", ex.Code);
    }
}
