using EBOS.CRM.Application.Features.CRM.Opportunity.Commands.UpdateOpportunity;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.CRM.Opportunity;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Opportunity.Commands.UpdateOpportunity;

public class UpdateOpportunityCommandHandlerTest
{
    [Fact]
    public async Task Handle_WhenNotFound_ReturnsNull()
    {
        var repository = new Mock<IOpportunityRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var mapper = new Mock<IMapper>();
        var stageValidation = new Mock<IOpportunityStageValidationService>();
        repository.Setup(x => x.GetByIdAsync(404, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.Opportunity?)null);

        var handler = new UpdateOpportunityCommandHandler(repository.Object, audit.Object, currentUser.Object, mapper.Object, stageValidation.Object);
        var req = new UpdateOpportunityRequest(404, 1, "Opp", 2, 3, 4, null, 100m, 0.5m, null, null, null);
        var result = await handler.Handle(new UpdateOpportunityCommand(404, req), CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenValid_UpdatesAndReturnsDto()
    {
        var repository = new Mock<IOpportunityRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var mapper = new Mock<IMapper>();
        var stageValidation = new Mock<IOpportunityStageValidationService>();
        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        audit.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var entity = new global::EBOS.CRM.Domain.Entities.CRM.Opportunity { Id = 1, TenantId = 1, Name = "Opp", StageId = 1, OwnerUserId = 2, CustomerId = 3, Amount = 100m, Probability = 0.3m };
        repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        stageValidation.Setup(x => x.EnsureStageAvailableAsync(1, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new global::EBOS.CRM.Domain.Entities.CRM.OpportunityStage { Id = 2, TenantId = 1, Name = "Qualified" });
        mapper.Setup(x => x.Map(It.IsAny<UpdateOpportunityRequest>(), entity)).Callback(() => entity.Name = "Opp-Updated");
        mapper.Setup(x => x.Map<OpportunityResponse>(entity))
            .Returns(new OpportunityResponse(1, 1, "Opp-Updated", 2, 2, 3, null, 100m, 0.5m, null, null, null, true));

        var handler = new UpdateOpportunityCommandHandler(repository.Object, audit.Object, currentUser.Object, mapper.Object, stageValidation.Object);
        var req = new UpdateOpportunityRequest(1, 1, "Opp-Updated", 2, 2, 3, null, 100m, 0.5m, null, null, null);
        var result = await handler.Handle(new UpdateOpportunityCommand(1, req), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Opp-Updated", result!.Name);
        repository.Verify(x => x.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }
}
