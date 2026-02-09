using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Features.EBOS.AddressesType.Query.GetAllAddressesType;
using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Interfaces.Repositories;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
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

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AddressType>());
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
    }

    [Fact]
    public async Task Handle_AddressTypesExist_ReturnsMappedDtos()
    {
        // Arrange
        var entities = new List<AddressType> { new() { Id = 1, Code = "HOME" } };
        var dtos = new List<AddressTypeResponse> { new(1, "HOME", "Home", null, false, false) };

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities.Count);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<AddressTypeResponse>>(entities))
            .Returns(dtos);

        var query = new GetAllAddressesTypeQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("HOME", result.Items.First().Code);
    }

    [Fact]
    public async Task Handle_NoAddressTypes_ReturnsEmptyEnumerable()
    {
        // Arrange
        var entities = new List<AddressType>();
        var dtos = new List<AddressTypeResponse>();

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<AddressTypeResponse>>(entities))
            .Returns(dtos);

        var query = new GetAllAddressesTypeQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_PropagatesException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Fail"));

        var query = new GetAllAddressesTypeQuery();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NullEntityProperty_MapsGracefully()
    {
        // Arrange
        var entities = new List<AddressType> { new() { Id = 1, Code = null! } };
        var dtos = new List<AddressTypeResponse> { new(1, null!, null!, null, false, false) };

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities.Count);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<AddressTypeResponse>>(entities))
            .Returns(dtos);

        var query = new GetAllAddressesTypeQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Null(result.Items.First().Code);
    }
}

