using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Queries.GetAllBranchOfficeAddresses;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.BranchOfficeAddress.Queries.GetAllBranchOfficeAddresses;

public class GetAllBranchOfficeAddressesQueryHandlerTest
{
    private readonly Mock<IBranchOfficeAddressRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    [Fact]
    public async Task Handle_ReturnsList()
    {
        var handler = new GetAllBranchOfficeAddressesQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        var entities = new List<global::EBOS.CRM.Domain.Entities.CRM.BranchOfficeAddress> { new() };

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities.Count);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<BranchOfficeAddressResponse>>(entities))
            .Returns(new List<BranchOfficeAddressResponse>());

        var result = await handler.Handle(new GetAllBranchOfficeAddressesQuery(), CancellationToken.None);

        Assert.NotNull(result);
        _repositoryMock.Verify(r => r.CountAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<IReadOnlyCollection<BranchOfficeAddressResponse>>(entities), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCanceled_ThrowsOperationCanceled()
    {
        var handler = new GetAllBranchOfficeAddressesQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(new GetAllBranchOfficeAddressesQuery(), cts.Token));
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_PropagatesException()
    {
        var handler = new GetAllBranchOfficeAddressesQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("db error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new GetAllBranchOfficeAddressesQuery(), CancellationToken.None));
    }
}

