using EBOS.CRM.Application.Features.Countries.Dtos;
using EBOS.CRM.Application.Features.Countries.Queries.GetAllCountries;
using EBOS.CRM.Application.Features.Statuses.Dtos;
using EBOS.CRM.Application.Features.Statuses.Queries.GetAllStatuses;
using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.Statuses.Queries.GetAllStatuses;

public class GetAllStatusesQueryHandlerTest
{
    private readonly Mock<IStatusRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllStatusesQueryHandler _handler;

    public GetAllStatusesQueryHandlerTest()
    {
        _repositoryMock = new Mock<IStatusRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllStatusesQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_CountriesExist_ReturnsMappedDtos()
    {
        // Arrange
        var statuses = new List<Status>
        {
            new() { Id = 1, Description = "Activo" }
        };
        var dtos = new List<StatusResponseDto>
        {
            new(1, "Activo")
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(statuses);
        _mapperMock.Setup(m => m.Map<IEnumerable<StatusResponseDto>>(statuses)).Returns(dtos);

        var query = new GetAllStatusesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Activo", result.First().Description);
        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<StatusResponseDto>>(statuses), Times.Once);
    }

    [Fact]
    public async Task Handle_NoCountries_ReturnsEmptyEnumerable()
    {
        // Arrange
        var statuses = new List<Status>();
        var dtos = new List<StatusResponseDto>();

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(statuses);
        _mapperMock.Setup(m => m.Map<IEnumerable<StatusResponseDto>>(statuses)).Returns(dtos);

        var query = new GetAllStatusesQuery();

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
        var query = new GetAllStatusesQuery();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CancellationRequested_ThrowsOperationCanceled()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var query = new GetAllStatusesQuery();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(query, cts.Token));
    }

    [Fact]
    public async Task Handle_MapperConfigurationInvalid_ThrowsMappingException()
    {
        // Arrange
        var statuses = new List<Status> { new() { Id = 1, Description = "Activo" } };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(statuses);

        // Simulamos que el mapper de Mapster falla
        _mapperMock.Setup(m => m.Map<IEnumerable<StatusResponseDto>>(statuses))
                   .Throws(new InvalidOperationException("Mapping failed"));

        var query = new GetAllStatusesQuery();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NullEntityProperty_MapsGracefully()
    {
        // Arrange
        var statuses = new List<Status> { new() { Id = 1, Description = null! } };
        var dtos = new List<StatusResponseDto> { new(1, null!) };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(statuses);
        _mapperMock.Setup(m => m.Map<IEnumerable<StatusResponseDto>>(statuses)).Returns(dtos);

        var query = new GetAllStatusesQuery();

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
        var statuses = new List<Status>();
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(statuses);

        var query = new GetAllStatusesQuery();

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mapperMock.Verify(m => m.Map<IEnumerable<StatusResponseDto>>(statuses), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryCalledOnce_WithCancellationToken()
    {
        // Arrange
        var query = new GetAllStatusesQuery();
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}