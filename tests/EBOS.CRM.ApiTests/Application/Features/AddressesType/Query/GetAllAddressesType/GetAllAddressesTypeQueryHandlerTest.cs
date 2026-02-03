using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Features.AddressesType.Query.GetAllAddressesType;
using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.AddressesType.Query.GetAllAddressesType;

public class GetAllAddressesTypeQueryHandlerTest
{
    private readonly Mock<IAddressTypeRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllAddressesTypeQueryHandler _handler;

    public GetAllAddressesTypeQueryHandlerTest()
    {
        _repositoryMock = new Mock<IAddressTypeRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllAddressesTypeQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_EntitiesExist_ReturnsMappedDtos()
    {
        var entities = new List<AddressType>
        {
            new()
            {
                Id = 1,
                Code = "HOME",
                Description = "Home",
                Category = "Shipping",
                AllowsMultiple = true,
                RequiresPrimary = false
            }
        };
        var dtos = new List<AddressTypeResponse>
        {
            new(1, "HOME", "Home", "Shipping", true, false)
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<AddressTypeResponse>>(entities)).Returns(dtos);

        var query = new GetAllAddressesTypeQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("HOME", result.First().Code);
        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<IReadOnlyCollection<AddressTypeResponse>>(entities), Times.Once);
    }

    [Fact]
    public async Task Handle_NoEntities_ReturnsEmptyEnumerable()
    {
        var entities = new List<AddressType>();
        var dtos = new List<AddressTypeResponse>();

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<AddressTypeResponse>>(entities)).Returns(dtos);

        var query = new GetAllAddressesTypeQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_PropagatesException()
    {
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));
        var query = new GetAllAddressesTypeQuery();

        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CancellationRequested_ThrowsOperationCanceled()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var query = new GetAllAddressesTypeQuery();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(query, cts.Token));
    }

    [Fact]
    public async Task Handle_MapperConfigurationInvalid_ThrowsMappingException()
    {
        var entities = new List<AddressType> { new() { Id = 1, Code = "HOME" } };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<AddressTypeResponse>>(entities))
            .Throws(new InvalidOperationException("Mapping failed"));

        var query = new GetAllAddressesTypeQuery();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NullEntityProperty_MapsGracefully()
    {
        var entities = new List<AddressType> { new() { Id = 1, Code = null! } };
        var dtos = new List<AddressTypeResponse> { new(1, null!, "Home", "Shipping", true, false) };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<AddressTypeResponse>>(entities)).Returns(dtos);

        var query = new GetAllAddressesTypeQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Null(result.First().Code);
    }

    [Fact]
    public async Task Handle_MapperCalledWithCorrectSourceType()
    {
        var entities = new List<AddressType>();
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var query = new GetAllAddressesTypeQuery();

        await _handler.Handle(query, CancellationToken.None);

        _mapperMock.Verify(m => m.Map<IReadOnlyCollection<AddressTypeResponse>>(entities), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryCalledOnce_WithCancellationToken()
    {
        var query = new GetAllAddressesTypeQuery();
        var entities = new List<AddressType>();
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        await _handler.Handle(query, CancellationToken.None);

        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}











