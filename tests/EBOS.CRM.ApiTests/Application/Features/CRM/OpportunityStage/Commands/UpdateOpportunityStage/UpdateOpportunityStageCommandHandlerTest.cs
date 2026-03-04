using EBOS.CRM.Application.Features.CRM.OpportunityStage.Commands.UpdateOpportunityStage;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.CRM.OpportunityStage;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.OpportunityStage.Commands.UpdateOpportunityStage;

public class UpdateOpportunityStageCommandHandlerTest
{
    [Fact]
    public async Task Handle_WhenNotFound_ReturnsNull()
    {
        var repository = new Mock<IOpportunityStageRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var mapper = new Mock<IMapper>();
        repository.Setup(x => x.GetByIdAsync(404, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.OpportunityStage?)null);

        var handler = new UpdateOpportunityStageCommandHandler(repository.Object, audit.Object, currentUser.Object, mapper.Object);
        var req = new UpdateOpportunityStageRequest(404, 1, "Qualified", 1, 0.3m, false, false);
        var result = await handler.Handle(new UpdateOpportunityStageCommand(404, req), CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenValid_UpdatesAndReturnsDto()
    {
        var repository = new Mock<IOpportunityStageRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var mapper = new Mock<IMapper>();
        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        audit.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var entity = new global::EBOS.CRM.Domain.Entities.CRM.OpportunityStage { Id = 1, TenantId = 1, Name = "Qualified", Order = 1, DefaultProbability = 0.3m };
        repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        mapper.Setup(x => x.Map(It.IsAny<UpdateOpportunityStageRequest>(), entity)).Callback(() => entity.Name = "Negotiation");
        mapper.Setup(x => x.Map<OpportunityStageResponse>(entity))
            .Returns(new OpportunityStageResponse(1, 1, "Negotiation", 2, 0.5m, false, false, true));

        var handler = new UpdateOpportunityStageCommandHandler(repository.Object, audit.Object, currentUser.Object, mapper.Object);
        var req = new UpdateOpportunityStageRequest(1, 1, "Negotiation", 2, 0.5m, false, false);
        var result = await handler.Handle(new UpdateOpportunityStageCommand(1, req), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Negotiation", result!.Name);
        repository.Verify(x => x.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }
}
