using EBOS.CRM.Application.Features.CRM.AccountContactRole.Queries.GetAccountContactRoleById;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;
using MapsterMapper;
using Moq;
using CRMAccountContactRole = EBOS.CRM.Domain.Entities.CRM.AccountContactRole;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountContactRole.Queries.GetAccountContactRoleById;

public class GetAccountContactRoleByIdQueryHandlerTest
{
    [Fact]
    public async Task Handle_WhenFound_ReturnsMappedResponse()
    {
        var repository = new Mock<IAccountContactRoleRepository>();
        var tenantContext = new Mock<ITenantContext>();
        var mapper = new Mock<IMapper>();

        tenantContext.SetupGet(x => x.TenantId).Returns(1);
        var entity = CRMAccountContactRole.Create(1, 10, "OWNER", false, DateTime.UtcNow, null);
        entity.Id = 5;
        repository.Setup(x => x.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        mapper.Setup(x => x.Map<AccountContactRoleResponse>(entity))
            .Returns(new AccountContactRoleResponse(5, 1, 10, "OWNER", false, entity.ValidFrom, entity.ValidTo, true));

        var handler = new GetAccountContactRoleByIdQueryHandler(repository.Object, tenantContext.Object, mapper.Object);
        var result = await handler.Handle(new GetAccountContactRoleByIdQuery(5), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(5, result.Id);
    }

    [Fact]
    public async Task Handle_TenantMismatch_Throws()
    {
        var repository = new Mock<IAccountContactRoleRepository>();
        var tenantContext = new Mock<ITenantContext>();
        var mapper = new Mock<IMapper>();

        tenantContext.SetupGet(x => x.TenantId).Returns(2);
        var entity = CRMAccountContactRole.Create(1, 10, "OWNER", false, DateTime.UtcNow, null);
        repository.Setup(x => x.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var handler = new GetAccountContactRoleByIdQueryHandler(repository.Object, tenantContext.Object, mapper.Object);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new GetAccountContactRoleByIdQuery(5), CancellationToken.None));
    }
}
