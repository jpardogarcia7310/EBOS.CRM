using EBOS.CRM.Application.Features.CRM.AccountHierarchy.Commands.EndAccountHierarchy;
using EBOS.CRM.Contracts.Requests.CRM.AccountHierarchy;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using Moq;
using CRMAccountHierarchy = EBOS.CRM.Domain.Entities.CRM.AccountHierarchy;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountHierarchy.Commands.EndAccountHierarchy;

public class EndAccountHierarchyCommandHandlerTest
{
    [Fact]
    public async Task Handle_WhenFound_EndsRelationAndReturnsResponse()
    {
        var repository = new Mock<IAccountHierarchyRepository>();
        var auditService = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var mapper = new Mock<IMapper>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        auditService.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var entity = CRMAccountHierarchy.Create(1, 10, 20, "HOLDING", DateTime.UtcNow.AddDays(-5));
        entity.Id = 7;
        repository.Setup(x => x.GetByIdAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        mapper.Setup(x => x.Map<AccountHierarchyResponse>(entity))
            .Returns(new AccountHierarchyResponse(entity.Id, entity.TenantId, entity.ParentCorporateCustomerId,
                entity.ChildCorporateCustomerId, entity.RelationType, entity.ValidFrom, DateTime.UtcNow, false, true));

        var handler = new EndAccountHierarchyCommandHandler(repository.Object, auditService.Object, currentUser.Object, mapper.Object);
        var result = await handler.Handle(new EndAccountHierarchyCommand(7, new EndAccountHierarchyRequest(1, DateTime.UtcNow)),
            CancellationToken.None);

        Assert.NotNull(result);
        repository.Verify(x => x.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
