using EBOS.CRM.Application.Contracts.Responses.CRM;
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
        var entities = new List<EBOS.CRM.Domain.Entities.CRM.CreditAccount> { new() };
        var dtos = new List<CreditAccountResponse>();

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<CreditAccountResponse>>(entities))
            .Returns(dtos);

        var result = await handler.Handle(new GetAllCreditAccountsQuery(), CancellationToken.None);

        Assert.NotNull(result);
    }
}







