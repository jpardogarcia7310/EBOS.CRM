using EBOS.CRM.Application.Features.EBOS.IdentificationType.Query.GetAllIdentificationType;
using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using Moq;
using IdentificationTypeEntity = EBOS.CRM.Domain.Entities.EBOS.IdentificationType;

namespace EBOS.CRM.ApiTests.Application.Features.EBOS.IdentificationType.Query.GetAllIdentificationType;

public class GetAllIdentificationTypeQueryHandlerTest
{
    private readonly Mock<IIdentificationTypeRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllIdentificationTypeQueryHandler _handler;

    public GetAllIdentificationTypeQueryHandlerTest()
    {
        _repositoryMock = new Mock<IIdentificationTypeRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllIdentificationTypeQueryHandler(_repositoryMock.Object, _mapperMock.Object);

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<IdentificationTypeEntity>());
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
    }

    [Fact]
    public async Task Handle_IdentificationTypesExist_ReturnsMappedDtos()
    {
        // Arrange
        var entities = new List<IdentificationTypeEntity> { new() { Id = 1, Code = "DNI" } };
        var dtos = new List<IdentificationTypeResponse> { new(1, "DNI", "Documento") };

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities.Count);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<IdentificationTypeResponse>>(entities))
            .Returns(dtos);

        var query = new GetAllIdentificationTypeQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("DNI", result.Items.First().Code);
    }

    [Fact]
    public async Task Handle_NoIdentificationTypes_ReturnsEmptyEnumerable()
    {
        // Arrange
        var entities = new List<IdentificationTypeEntity>();

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<IdentificationTypeResponse>>(entities))
            .Returns(new List<IdentificationTypeResponse>());

        var query = new GetAllIdentificationTypeQuery();

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

        var query = new GetAllIdentificationTypeQuery();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NullEntityProperty_MapsGracefully()
    {
        // Arrange
        var entities = new List<IdentificationTypeEntity> { new() { Id = 1, Code = null! } };
        var dtos = new List<IdentificationTypeResponse> { new(1, null!, null!) };

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities.Count);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<IdentificationTypeResponse>>(entities))
            .Returns(dtos);

        var query = new GetAllIdentificationTypeQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Null(result.Items.First().Code);
    }
}
