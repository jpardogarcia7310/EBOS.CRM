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
        var entities = new List<global::EBOS.CRM.Domain.Entities.CRM.CreditTransaction> { new() };

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities.Count);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<CreditTransactionResponse>>(entities))
            .Returns(new List<CreditTransactionResponse>());

        var result = await handler.Handle(new GetAllCreditTransactionsQuery(), CancellationToken.None);

        Assert.NotNull(result);
        _repositoryMock.Verify(r => r.CountAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<IReadOnlyCollection<CreditTransactionResponse>>(entities), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCanceled_ThrowsOperationCanceled()
    {
        var handler = new GetAllCreditTransactionsQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(new GetAllCreditTransactionsQuery(), cts.Token));
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_PropagatesException()
    {
        var handler = new GetAllCreditTransactionsQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("db error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new GetAllCreditTransactionsQuery(), CancellationToken.None));
    }
}
