using EBOS.CRM.Application.Features.CRM.AccountHierarchy.Queries.GetAccountHierarchyById;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;
using MapsterMapper;
using Moq;
using CRMAccountHierarchy = EBOS.CRM.Domain.Entities.CRM.AccountHierarchy;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountHierarchy.Queries.GetAccountHierarchyById;

public class GetAccountHierarchyByIdQueryHandlerTest
{
    [Fact]
    public async Task Handle_WhenFound_ReturnsMappedResponse()
    {
        var repository = new Mock<IAccountHierarchyRepository>();
        var tenant = new Mock<ITenantContext>();
        var mapper = new Mock<IMapper>();

        tenant.SetupGet(x => x.TenantId).Returns(1);
        var entity = CRMAccountHierarchy.Create(1, 10, 20, "HOLDING", DateTime.UtcNow);
        entity.Id = 3;
        repository.Setup(x => x.GetByIdAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        mapper.Setup(x => x.Map<AccountHierarchyResponse>(entity))
            .Returns(new AccountHierarchyResponse(3, 1, 10, 20, "HOLDING", entity.ValidFrom, null, true, true));

        var handler = new GetAccountHierarchyByIdQueryHandler(repository.Object, tenant.Object, mapper.Object);
        var result = await handler.Handle(new GetAccountHierarchyByIdQuery(3), CancellationToken.None);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_TenantMismatch_Throws()
    {
        var repository = new Mock<IAccountHierarchyRepository>();
        var tenant = new Mock<ITenantContext>();
        var mapper = new Mock<IMapper>();

        tenant.SetupGet(x => x.TenantId).Returns(2);
        repository.Setup(x => x.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CRMAccountHierarchy.Create(1, 10, 20, "HOLDING", DateTime.UtcNow));

        var handler = new GetAccountHierarchyByIdQueryHandler(repository.Object, tenant.Object, mapper.Object);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new GetAccountHierarchyByIdQuery(3), CancellationToken.None));
    }
}
