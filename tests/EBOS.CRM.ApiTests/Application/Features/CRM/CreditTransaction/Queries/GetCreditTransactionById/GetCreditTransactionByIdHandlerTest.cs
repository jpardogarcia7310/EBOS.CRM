using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.CreditTransaction.Queries.GetCreditTransactionById;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CreditTransaction.Queries.GetCreditTransactionById;

public class GetCreditTransactionByIdQueryHandlerTest
{
    private readonly Mock<ICreditTransactionRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_WhenFound_Maps()
    {
        var handler = new GetCreditTransactionByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entity = new global::EBOS.CRM.Domain.Entities.CRM.CreditTransaction();

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<CreditTransactionResponse>(entity))
            .Returns((CreditTransactionResponse)null!);

        await handler.Handle(new GetCreditTransactionByIdQuery(1), CancellationToken.None);

        _mapperMock.Verify(m => m.Map<CreditTransactionResponse>(entity), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ReturnsNull()
    {
        var handler = new GetCreditTransactionByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        _repositoryMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.CreditTransaction?)null);

        var result = await handler.Handle(new GetCreditTransactionByIdQuery(99), CancellationToken.None);

        Assert.Null(result);
        _mapperMock.Verify(m => m.Map<CreditTransactionResponse>(It.IsAny<global::EBOS.CRM.Domain.Entities.CRM.CreditTransaction>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCanceled_ThrowsOperationCanceled()
    {
        var handler = new GetCreditTransactionByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(new GetCreditTransactionByIdQuery(1), cts.Token));
    }
}


