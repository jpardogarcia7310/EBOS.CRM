using EBOS.CRM.Application.Features.CRM.AccountHierarchy.Commands.AddAccountHierarchy;
using EBOS.CRM.Contracts.Requests.CRM.AccountHierarchy;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MapsterMapper;
using Moq;
using CRMAccountHierarchy = EBOS.CRM.Domain.Entities.CRM.AccountHierarchy;
using CRMCorporateCustomer = EBOS.CRM.Domain.Entities.CRM.CorporateCustomer;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountHierarchy.Commands.AddAccountHierarchy;

public class AddAccountHierarchyCommandHandlerTest
{
    [Fact]
    public async Task Handle_ValidRequest_PersistsAndReturnsResponse()
    {
        var repository = new Mock<IAccountHierarchyRepository>();
        var corporateRepo = new Mock<ICorporateCustomerRepository>();
        var invariant = new Mock<IAccountHierarchyAcyclicInvariant>();
        var auditService = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var mapper = new Mock<IMapper>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        auditService.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        corporateRepo.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CRMCorporateCustomer { Id = 10, TenantId = 1, LegalName = "P", TaxIdentification = "P1" });
        corporateRepo.Setup(x => x.GetByIdAsync(20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CRMCorporateCustomer { Id = 20, TenantId = 1, LegalName = "C", TaxIdentification = "C1" });

        mapper.Setup(x => x.Map<AccountHierarchyResponse>(It.IsAny<CRMAccountHierarchy>()))
            .Returns((CRMAccountHierarchy e) =>
                new AccountHierarchyResponse(e.Id, e.TenantId, e.ParentCorporateCustomerId, e.ChildCorporateCustomerId,
                    e.RelationType, e.ValidFrom, e.ValidTo, e.IsCurrent, true));

        var handler = new AddAccountHierarchyCommandHandler(
            repository.Object, corporateRepo.Object, invariant.Object, auditService.Object, currentUser.Object, mapper.Object);

        var command = new AddAccountHierarchyCommand(new AddAccountHierarchyRequest(1, 10, 20, "HOLDING", DateTime.UtcNow));
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        repository.Verify(x => x.AddAsync(It.IsAny<CRMAccountHierarchy>(), It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
