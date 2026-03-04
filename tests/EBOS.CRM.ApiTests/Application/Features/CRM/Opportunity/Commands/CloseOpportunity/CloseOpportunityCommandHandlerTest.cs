using EBOS.CRM.Application.Features.CRM.Opportunity.Commands.CloseOpportunity;
using EBOS.CRM.Contracts.Requests.CRM.Opportunity;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Opportunity.Commands.CloseOpportunity;

public class CloseOpportunityCommandHandlerTest
{
    [Fact]
    public async Task Handle_WhenNotFound_ReturnsNull()
    {
        var repository = new Mock<IOpportunityRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        repository.Setup(x => x.GetByIdAsync(404, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.Opportunity?)null);

        var handler = new CloseOpportunityCommandHandler(repository.Object, audit.Object, currentUser.Object);
        var result = await handler.Handle(new CloseOpportunityCommand(404, new CloseOpportunityRequest(1, 2, true, "won")), CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenValid_ClosesOpportunity()
    {
        var repository = new Mock<IOpportunityRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        audit.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var entity = new global::EBOS.CRM.Domain.Entities.CRM.Opportunity { Id = 1, TenantId = 1, Name = "Opp", StageId = 1, OwnerUserId = 2, CustomerId = 3, Amount = 100m, Probability = 0.3m };
        repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var handler = new CloseOpportunityCommandHandler(repository.Object, audit.Object, currentUser.Object);
        var result = await handler.Handle(new CloseOpportunityCommand(1, new CloseOpportunityRequest(1, 9, true, "won")), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(9, result.StageId);
        Assert.Equal(1m, result.Probability);
        repository.Verify(x => x.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }
}
