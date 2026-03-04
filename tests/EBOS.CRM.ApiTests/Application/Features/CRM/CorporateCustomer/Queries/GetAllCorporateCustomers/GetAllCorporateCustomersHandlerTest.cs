using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.CorporateCustomer.Queries.GetAllCorporateCustomers;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CorporateCustomer.Queries.GetAllCorporateCustomers;

public class GetAllCorporateCustomersQueryHandlerTest
{
    private readonly Mock<ICorporateCustomerRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_ReturnsList()
    {
        var handler = new GetAllCorporateCustomersQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entities = new List<global::EBOS.CRM.Domain.Entities.CRM.CorporateCustomer> { new() };

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities.Count);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<CorporateCustomerResponse>>(entities))
            .Returns(new List<CorporateCustomerResponse>());

        var result = await handler.Handle(new GetAllCorporateCustomersQuery(), CancellationToken.None);

        Assert.NotNull(result);
        _repositoryMock.Verify(r => r.CountAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<IReadOnlyCollection<CorporateCustomerResponse>>(entities), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCanceled_ThrowsOperationCanceled()
    {
        var handler = new GetAllCorporateCustomersQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(new GetAllCorporateCustomersQuery(), cts.Token));
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_PropagatesException()
    {
        var handler = new GetAllCorporateCustomersQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("db error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new GetAllCorporateCustomersQuery(), CancellationToken.None));
    }
}
