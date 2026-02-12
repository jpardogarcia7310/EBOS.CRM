using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.CreditTransaction.Queries.GetAllCreditTransactions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CreditTransaction.Queries.GetAllCreditTransactions;

public class GetAllCreditTransactionsQueryHandlerTest
{
    private readonly Mock<ICreditTransactionRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_ReturnsList()
    {
        var handler = new GetAllCreditTransactionsQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entities = new List<EBOS.CRM.Domain.Entities.CRM.CreditTransaction> { new() };
        var dtos = new List<CreditTransactionResponse>();

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities.Count);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<CreditTransactionResponse>>(entities))
            .Returns(dtos);

        var result = await handler.Handle(new GetAllCreditTransactionsQuery(), CancellationToken.None);

        Assert.NotNull(result);
    }
}
