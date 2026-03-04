using EBOS.CRM.Application.Features.CRM.AccountContact.Queries.GetAccountContactsByAccount;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;
using CRMAccountContact = EBOS.CRM.Domain.Entities.CRM.AccountContact;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountContact.Queries.GetAccountContactsByAccount;

public class GetAccountContactsByAccountQueryHandlerTest
{
    [Fact]
    public async Task Handle_ReturnsPagedResult()
    {
        var repository = new Mock<IAccountContactRepository>();
        var mapper = new Mock<IMapper>();

        var entities = new List<CRMAccountContact> { CRMAccountContact.Create(1, 20, 30, false, DateTime.UtcNow, null, 1) };
        repository.Setup(x => x.GetByCorporateCustomerPagedAsync(1, 20, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        repository.Setup(x => x.CountByCorporateCustomerAsync(1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        mapper.Setup(x => x.Map<IReadOnlyCollection<AccountContactResponse>>(entities))
            .Returns(new[] { new AccountContactResponse(1, 1, 20, 30, false, DateTime.UtcNow, null, true) });

        var handler = new GetAccountContactsByAccountQueryHandler(repository.Object, mapper.Object);
        var result = await handler.Handle(new GetAccountContactsByAccountQuery(1, 20, 1, 10), CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal(1, result.Total);
    }
}
