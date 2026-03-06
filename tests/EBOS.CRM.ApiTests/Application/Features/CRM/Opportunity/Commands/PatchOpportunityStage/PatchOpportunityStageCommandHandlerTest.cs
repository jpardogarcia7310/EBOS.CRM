using EBOS.CRM.Application.Features.CRM.Opportunity.Commands.PatchOpportunityStage;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.CRM.Opportunity;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Opportunity.Commands.PatchOpportunityStage;

public class PatchOpportunityStageCommandHandlerTest
{
    [Fact]
    public async Task Handle_WhenNotFound_ReturnsNull()
    {
        var repository = new Mock<IOpportunityRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var stageValidation = new Mock<IOpportunityStageValidationService>();
        repository.Setup(x => x.GetByIdAsync(404, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.Opportunity?)null);

        var handler = new PatchOpportunityStageCommandHandler(repository.Object, audit.Object, currentUser.Object, stageValidation.Object);
        var result = await handler.Handle(new PatchOpportunityStageCommand(404, new PatchOpportunityStageRequest(1, 2, 0.5m)), CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenValid_UpdatesStage()
    {
        var repository = new Mock<IOpportunityRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var stageValidation = new Mock<IOpportunityStageValidationService>();
        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        audit.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var entity = new global::EBOS.CRM.Domain.Entities.CRM.Opportunity { Id = 1, TenantId = 1, Name = "Opp", StageId = 1, OwnerUserId = 2, CustomerId = 3, Amount = 100m, Probability = 0.3m };
        repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        stageValidation.Setup(x => x.EnsureStageAvailableAsync(1, 8, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new global::EBOS.CRM.Domain.Entities.CRM.OpportunityStage { Id = 8, TenantId = 1, Name = "Proposal" });

        var handler = new PatchOpportunityStageCommandHandler(repository.Object, audit.Object, currentUser.Object, stageValidation.Object);
        var result = await handler.Handle(new PatchOpportunityStageCommand(1, new PatchOpportunityStageRequest(1, 8, 0.7m)), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(8, result!.StageId);
        Assert.Equal(0.7m, result.Probability);
        repository.Verify(x => x.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }
}
