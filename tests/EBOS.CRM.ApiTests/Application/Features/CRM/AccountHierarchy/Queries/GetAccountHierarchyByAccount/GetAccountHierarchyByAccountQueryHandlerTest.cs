using EBOS.CRM.Application.Features.CRM.AccountHierarchy.Queries.GetAccountHierarchyByAccount;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;
using CRMAccountHierarchy = EBOS.CRM.Domain.Entities.CRM.AccountHierarchy;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.AccountHierarchy.Queries.GetAccountHierarchyByAccount;

public class GetAccountHierarchyByAccountQueryHandlerTest
{
    [Fact]
    public async Task Handle_ReturnsPagedResult()
    {
        var repository = new Mock<IAccountHierarchyRepository>();
        var mapper = new Mock<IMapper>();

        var page = new List<CRMAccountHierarchy> { CRMAccountHierarchy.Create(1, 10, 20, "HOLDING", DateTime.UtcNow) };
        repository.Setup(x => x.GetByAccountPagedAsync(1, 10, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(page);
        repository.Setup(x => x.CountByAccountAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        mapper.Setup(x => x.Map<IReadOnlyCollection<AccountHierarchyResponse>>(page))
            .Returns(new[] { new AccountHierarchyResponse(1, 1, 10, 20, "HOLDING", DateTime.UtcNow, null, true, true) });

        var handler = new GetAccountHierarchyByAccountQueryHandler(repository.Object, mapper.Object);
        var result = await handler.Handle(new GetAccountHierarchyByAccountQuery(1, 10, 1, 10), CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal(1, result.Total);
    }
}
