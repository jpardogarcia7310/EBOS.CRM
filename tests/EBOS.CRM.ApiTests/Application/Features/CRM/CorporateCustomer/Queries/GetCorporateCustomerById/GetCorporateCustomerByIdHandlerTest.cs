using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.CorporateCustomer.Queries.GetCorporateCustomerById;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.CorporateCustomer.Queries.GetCorporateCustomerById;

public class GetCorporateCustomerByIdQueryHandlerTest
{
    private readonly Mock<ICorporateCustomerRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_WhenFound_Maps()
    {
        var handler = new GetCorporateCustomerByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entity = new global::EBOS.CRM.Domain.Entities.CRM.CorporateCustomer();

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<CorporateCustomerResponse>(entity))
            .Returns((CorporateCustomerResponse)null!);

        await handler.Handle(new GetCorporateCustomerByIdQuery(1), CancellationToken.None);

        _mapperMock.Verify(m => m.Map<CorporateCustomerResponse>(entity), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ReturnsNull()
    {
        var handler = new GetCorporateCustomerByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        _repositoryMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.CorporateCustomer?)null);

        var result = await handler.Handle(new GetCorporateCustomerByIdQuery(99), CancellationToken.None);

        Assert.Null(result);
        _mapperMock.Verify(m => m.Map<CorporateCustomerResponse>(It.IsAny<global::EBOS.CRM.Domain.Entities.CRM.CorporateCustomer>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCanceled_ThrowsOperationCanceled()
    {
        var handler = new GetCorporateCustomerByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(new GetCorporateCustomerByIdQuery(1), cts.Token));
    }
}


