using EBOS.CRM.Application.Features.CRM.Opportunity.Commands.AddOpportunity;
using EBOS.CRM.Contracts.Requests.CRM.Opportunity;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Opportunity.Commands.AddOpportunity;

public class AddOpportunityCommandHandlerTest
{
    [Fact]
    public async Task Handle_ValidRequest_PersistsAndReturnsResponse()
    {
        var repository = new Mock<IOpportunityRepository>();
        var auditService = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var mapper = new Mock<IMapper>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        auditService.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));
        mapper.Setup(x => x.Map<global::EBOS.CRM.Domain.Entities.CRM.Opportunity>(It.IsAny<AddOpportunityRequest>()))
            .Returns(new global::EBOS.CRM.Domain.Entities.CRM.Opportunity { Id = 1, TenantId = 1, Name = "Opp A", StageId = 2, OwnerUserId = 3, CustomerId = 4, Amount = 100m, Probability = 0.5m });
        mapper.Setup(x => x.Map<OpportunityResponse>(It.IsAny<global::EBOS.CRM.Domain.Entities.CRM.Opportunity>()))
            .Returns(new OpportunityResponse(1, 1, "Opp A", 2, 3, 4, null, 100m, 0.5m, null, null, null, true));

        var handler = new AddOpportunityCommandHandler(repository.Object, auditService.Object, currentUser.Object, mapper.Object);
        var result = await handler.Handle(new AddOpportunityCommand(new AddOpportunityRequest(1, "Opp A", 2, 3, 4, null, 100m, 0.5m, null, null)), CancellationToken.None);

        Assert.NotNull(result);
        repository.Verify(x => x.AddAsync(It.IsAny<global::EBOS.CRM.Domain.Entities.CRM.Opportunity>(), It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
