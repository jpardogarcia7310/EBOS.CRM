using EBOS.CRM.Application.Features.Countries.Dtos;
using EBOS.CRM.Application.Features.Countries.Queries.GetAllCountries;
using EBOS.CRM.Application.Features.Statuses.Dtos;
using EBOS.CRM.Application.Features.Statuses.Queries.GetAllStatuses;
using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.Statuses.Queries.GetAllStatuses;

public class GetAllEstadosQueryHandlerTest
{
    private readonly Mock<IEstadoRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllEstadosQueryHandler _handler;

    public GetAllEstadosQueryHandlerTest()
    {
        _repositoryMock = new Mock<IEstadoRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllEstadosQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_CountriesExist_ReturnsMappedDtos()
    {
        // Arrange
        var statuses = new List<Estado>
        {
            new() { Id = 1, Description = "Activo" }
        };
        var dtos = new List<EstadoResponseDto>
        {
            new(1, "Activo")
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(statuses);
        _mapperMock.Setup(m => m.Map<IEnumerable<EstadoResponseDto>>(statuses)).Returns(dtos);

        var query = new GetAllEstadosQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Activo", result.First().Description);
        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<EstadoResponseDto>>(statuses), Times.Once);
    }

    [Fact]
    public async Task Handle_NoCountries_ReturnsEmptyEnumerable()
    {
        // Arrange
        var statuses = new List<Estado>();
        var dtos = new List<EstadoResponseDto>();

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(statuses);
        _mapperMock.Setup(m => m.Map<IEnumerable<EstadoResponseDto>>(statuses)).Returns(dtos);

        var query = new GetAllEstadosQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_PropagatesException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new Exception("DB error"));
        var query = new GetAllEstadosQuery();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CancellationRequested_ThrowsOperationCanceled()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var query = new GetAllEstadosQuery();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(query, cts.Token));
    }

    [Fact]
    public async Task Handle_MapperConfigurationInvalid_ThrowsMappingException()
    {
        // Arrange
        var statuses = new List<Estado> { new() { Id = 1, Description = "Activo" } };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(statuses);

        // Simulamos que el mapper de Mapster falla
        _mapperMock.Setup(m => m.Map<IEnumerable<EstadoResponseDto>>(statuses))
                   .Throws(new InvalidOperationException("Mapping failed"));

        var query = new GetAllEstadosQuery();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NullEntityProperty_MapsGracefully()
    {
        // Arrange
        var statuses = new List<Estado> { new() { Id = 1, Description = null! } };
        var dtos = new List<EstadoResponseDto> { new(1, null!) };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(statuses);
        _mapperMock.Setup(m => m.Map<IEnumerable<EstadoResponseDto>>(statuses)).Returns(dtos);

        var query = new GetAllEstadosQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Null(result.First().Description);
    }

    [Fact]
    public async Task Handle_MapperCalledWithCorrectSourceType()
    {
        // Arrange
        var statuses = new List<Estado>();
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(statuses);

        var query = new GetAllEstadosQuery();

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mapperMock.Verify(m => m.Map<IEnumerable<EstadoResponseDto>>(statuses), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryCalledOnce_WithCancellationToken()
    {
        // Arrange
        var query = new GetAllEstadosQuery();
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}