using EBOS.CRM.Application.Features.CRM.OpportunityStage.Commands.AddOpportunityStage;
using EBOS.CRM.Contracts.Requests.CRM.OpportunityStage;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.OpportunityStage.Commands.AddOpportunityStage;

public class AddOpportunityStageCommandHandlerTest
{
    [Fact]
    public async Task Handle_ValidRequest_PersistsAndReturnsResponse()
    {
        var repository = new Mock<IOpportunityStageRepository>();
        var auditService = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var mapper = new Mock<IMapper>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        auditService.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));
        mapper.Setup(x => x.Map<global::EBOS.CRM.Domain.Entities.CRM.OpportunityStage>(It.IsAny<AddOpportunityStageRequest>()))
            .Returns(new global::EBOS.CRM.Domain.Entities.CRM.OpportunityStage { Id = 1, TenantId = 1, Name = "Qualified", Order = 1, DefaultProbability = 0.3m });
        mapper.Setup(x => x.Map<OpportunityStageResponse>(It.IsAny<global::EBOS.CRM.Domain.Entities.CRM.OpportunityStage>()))
            .Returns(new OpportunityStageResponse(1, 1, "Qualified", 1, 0.3m, false, false, true));

        var handler = new AddOpportunityStageCommandHandler(repository.Object, auditService.Object, currentUser.Object, mapper.Object);
        var result = await handler.Handle(new AddOpportunityStageCommand(new AddOpportunityStageRequest(1, "Qualified", 1, 0.3m, false, false)), CancellationToken.None);

        Assert.NotNull(result);
        repository.Verify(x => x.AddAsync(It.IsAny<global::EBOS.CRM.Domain.Entities.CRM.OpportunityStage>(), It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
