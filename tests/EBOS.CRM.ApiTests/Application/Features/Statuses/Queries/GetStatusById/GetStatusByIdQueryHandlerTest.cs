using EBOS.CRM.Application.Features.Statuses.Dtos;
using EBOS.CRM.Application.Features.Statuses.Queries.GetStatusById;
using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.Statuses.Queries.GetStatusById;

public class GetStatusByIdQueryHandlerTest
{
    private readonly Mock<IStatusRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetStatusByIdQueryHandler _handler;

    public GetStatusByIdQueryHandlerTest()
    {
        _repositoryMock = new Mock<IStatusRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetStatusByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsMappedDto()
    {
        // Arrange
        var status = new Status() { Id = 1, Description = "Activo" };
        var dto = new StatusResponseDto(status.Id, status.Description);

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);
        _mapperMock.Setup(m => m.Map<StatusResponseDto>(status)).Returns(dto);

        var query = new GetStatusByIdQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Id, result.Id);
        Assert.Equal(dto.Description, result.Description);
        _repositoryMock.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()),
            Times.Once);
        _mapperMock.Verify(m => m.Map<StatusResponseDto>(status), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingId_ReturnsNull()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Status?)null);
        var query = new GetStatusByIdQuery(99);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _mapperMock.Verify(m => m.Map<StatusResponseDto>(It.IsAny<Status>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_PropagatesException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new Exception("DB error"));
        var query = new GetStatusByIdQuery(1);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CancellationRequested_ThrowsOperationCanceled()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var query = new GetStatusByIdQuery(1);

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(query, cts.Token));
    }

    [Fact]
    public async Task Handle_MapperConfigurationInvalid_ThrowsMappingException()
    {
        // Arrange
        var country = new Status() { Id = 1, Description = "España" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(country);

        // Simulamos que el mapper de Mapster falla
        _mapperMock.Setup(m => m.Map<StatusResponseDto>(country))
                   .Throws(new InvalidOperationException("Mapping failed"));

        var query = new GetStatusByIdQuery(1);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_RepositoryCalledOnce_WithCorrectIdAndToken()
    {
        // Arrange
        var query = new GetStatusByIdQuery(42);
        _repositoryMock.Setup(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Status?)null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_NullEntity_DoesNotCallMapper()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Status?)null);
        var query = new GetStatusByIdQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _mapperMock.Verify(m => m.Map<StatusResponseDto>(It.IsAny<Status>()), Times.Never);
    }
}