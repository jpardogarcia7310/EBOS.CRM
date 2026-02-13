using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.CreditAccount.Queries.GetAllCreditAccounts;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CreditAccount.Queries.GetAllCreditAccounts;

public class GetAllCreditAccountsQueryHandlerTest
{
    private readonly Mock<ICreditAccountRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_ReturnsList()
    {
        var handler = new GetAllCreditAccountsQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entities = new List<global::EBOS.CRM.Domain.Entities.CRM.CreditAccount> { new() };

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities.Count);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<CreditAccountResponse>>(entities))
            .Returns(new List<CreditAccountResponse>());

        var result = await handler.Handle(new GetAllCreditAccountsQuery(), CancellationToken.None);

        Assert.NotNull(result);
    }
}
