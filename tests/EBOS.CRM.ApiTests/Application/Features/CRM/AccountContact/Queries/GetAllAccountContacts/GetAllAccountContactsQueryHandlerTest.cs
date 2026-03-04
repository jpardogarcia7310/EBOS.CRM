using EBOS.CRM.Application.Features.CRM.AccountContact.Queries.GetAllAccountContacts;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;
using MapsterMapper;
using Moq;
using CRMAccountContact = EBOS.CRM.Domain.Entities.CRM.AccountContact;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountContact.Queries.GetAllAccountContacts;

public class GetAllAccountContactsQueryHandlerTest
{
    [Fact]
    public async Task Handle_TenantAligned_ReturnsPagedResult()
    {
        var repository = new Mock<IAccountContactRepository>();
        var tenantContext = new Mock<ITenantContext>();
        var mapper = new Mock<IMapper>();

        tenantContext.SetupGet(x => x.TenantId).Returns(1);
        var entities = new List<CRMAccountContact> { CRMAccountContact.Create(1, 20, 30, false, DateTime.UtcNow, null, 1) };
        repository.Setup(x => x.GetAllPagedAsync(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(entities);
        mapper.Setup(x => x.Map<IReadOnlyCollection<AccountContactResponse>>(entities))
            .Returns(new[] { new AccountContactResponse(1, 1, 20, 30, false, DateTime.UtcNow, null, true) });

        var handler = new GetAllAccountContactsQueryHandler(repository.Object, tenantContext.Object, mapper.Object);
        var result = await handler.Handle(new GetAllAccountContactsQuery(1, 1, 10), CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal(1, result.Total);
    }

    [Fact]
    public async Task Handle_TenantMismatch_Throws()
    {
        var repository = new Mock<IAccountContactRepository>();
        var tenantContext = new Mock<ITenantContext>();
        var mapper = new Mock<IMapper>();

        tenantContext.SetupGet(x => x.TenantId).Returns(1);
        var entities = new List<CRMAccountContact> { CRMAccountContact.Create(2, 20, 30, false, DateTime.UtcNow, null, 1) };
        repository.Setup(x => x.GetAllPagedAsync(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(entities);

        var handler = new GetAllAccountContactsQueryHandler(repository.Object, tenantContext.Object, mapper.Object);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new GetAllAccountContactsQuery(1, 1, 10), CancellationToken.None));
    }
}
