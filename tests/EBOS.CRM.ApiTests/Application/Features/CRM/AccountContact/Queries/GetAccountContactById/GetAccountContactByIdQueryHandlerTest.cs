using EBOS.CRM.Application.Features.CRM.AccountContact.Queries.GetAccountContactById;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;
using MapsterMapper;
using Moq;
using CRMAccountContact = EBOS.CRM.Domain.Entities.CRM.AccountContact;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountContact.Queries.GetAccountContactById;

public class GetAccountContactByIdQueryHandlerTest
{
    [Fact]
    public async Task Handle_EntityExists_ReturnsMappedResponse()
    {
        var repository = new Mock<IAccountContactRepository>();
        var tenantContext = new Mock<ITenantContext>();
        var mapper = new Mock<IMapper>();

        tenantContext.SetupGet(x => x.TenantId).Returns(1);
        var entity = CRMAccountContact.Create(1, 20, 30, false, DateTime.UtcNow, null, 1);
        entity.Id = 10;
        repository.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        mapper.Setup(x => x.Map<AccountContactResponse>(entity))
            .Returns(new AccountContactResponse(10, 1, 20, 30, false, entity.StartAt, entity.EndAt, true));

        var handler = new GetAccountContactByIdQueryHandler(repository.Object, tenantContext.Object, mapper.Object);
        var result = await handler.Handle(new GetAccountContactByIdQuery(10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
    }

    [Fact]
    public async Task Handle_TenantMismatch_Throws()
    {
        var repository = new Mock<IAccountContactRepository>();
        var tenantContext = new Mock<ITenantContext>();
        var mapper = new Mock<IMapper>();

        tenantContext.SetupGet(x => x.TenantId).Returns(2);
        var entity = CRMAccountContact.Create(1, 20, 30, false, DateTime.UtcNow, null, 1);
        repository.Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var handler = new GetAccountContactByIdQueryHandler(repository.Object, tenantContext.Object, mapper.Object);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new GetAccountContactByIdQuery(10), CancellationToken.None));
    }
}
