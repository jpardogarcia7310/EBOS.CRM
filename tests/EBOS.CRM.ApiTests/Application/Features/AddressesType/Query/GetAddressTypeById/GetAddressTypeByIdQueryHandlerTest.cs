using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Features.AddressesType.Query.GetAddressTypeById;
using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.AddressesType.Query.GetAddressTypeById;

public class GetAddressTypeByIdQueryHandlerTest
{
    private readonly Mock<IAddressTypeRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAddressTypeByIdQueryHandler _handler;

    public GetAddressTypeByIdQueryHandlerTest()
    {
        _repositoryMock = new Mock<IAddressTypeRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAddressTypeByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsMappedDto()
    {
        var entity = new AddressType
        {
            Id = 1,
            Code = "HOME",
            Description = "Home",
            Category = "Shipping",
            AllowsMultiple = true,
            RequiresPrimary = false
        };
        var dto = new AddressTypeResponse(1, "HOME", "Home", "Shipping", true, false);

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<AddressTypeResponse>(entity)).Returns(dto);

        var query = new GetAddressTypeByIdQuery(1);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(dto.Id, result.Id);
        Assert.Equal(dto.Code, result.Code);
        _repositoryMock.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<AddressTypeResponse>(entity), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingId_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddressType?)null);
        var query = new GetAddressTypeByIdQuery(99);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
        _mapperMock.Verify(m => m.Map<AddressTypeResponse>(It.IsAny<AddressType>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_PropagatesException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));
        var query = new GetAddressTypeByIdQuery(1);

        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CancellationRequested_ThrowsOperationCanceled()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var query = new GetAddressTypeByIdQuery(1);

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(query, cts.Token));
    }

    [Fact]
    public async Task Handle_MapperConfigurationInvalid_ThrowsMappingException()
    {
        var entity = new AddressType { Id = 1, Code = "HOME", Description = "Home" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<AddressTypeResponse>(entity))
            .Throws(new InvalidOperationException("Mapping failed"));

        var query = new GetAddressTypeByIdQuery(1);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_RepositoryCalledOnce_WithCorrectIdAndToken()
    {
        var query = new GetAddressTypeByIdQuery(42);
        _repositoryMock.Setup(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddressType?)null);

        await _handler.Handle(query, CancellationToken.None);

        _repositoryMock.Verify(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NullEntity_DoesNotCallMapper()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddressType?)null);
        var query = new GetAddressTypeByIdQuery(1);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
        _mapperMock.Verify(m => m.Map<AddressTypeResponse>(It.IsAny<AddressType>()), Times.Never);
    }
}


