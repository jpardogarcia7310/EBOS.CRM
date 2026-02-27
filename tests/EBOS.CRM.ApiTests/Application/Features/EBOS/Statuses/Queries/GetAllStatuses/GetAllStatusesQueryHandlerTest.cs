using EBOS.CRM.Application.Features.EBOS.Statuses.Queries.GetAllStatuses;
using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.EBOS.Statuses.Queries.GetAllStatuses;

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

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status>());
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
    }

    [Fact]
    public async Task Handle_StatusesExist_ReturnsMappedDtos()
    {
        // Arrange
        var entities = new List<Status> { new() { Id = 1, Description = "Active" } };
        var dtos = new List<StatusResponse> { new(1, "Active") };

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities.Count);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<StatusResponse>>(entities))
            .Returns(dtos);

        var query = new GetAllStatusesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Active", result.Items.First().Description);
    }

    [Fact]
    public async Task Handle_NoStatuses_ReturnsEmptyEnumerable()
    {
        // Arrange
        var entities = new List<Status>();

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<StatusResponse>>(entities))
            .Returns(new List<StatusResponse>());

        var query = new GetAllStatusesQuery();

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

        var query = new GetAllStatusesQuery();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NullEntityProperty_MapsGracefully()
    {
        // Arrange
        var entities = new List<Status> { new() { Id = 1, Description = null! } };
        var dtos = new List<StatusResponse> { new(1, null!) };

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities.Count);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<StatusResponse>>(entities))
            .Returns(dtos);

        var query = new GetAllStatusesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Null(result.Items.First().Description);
    }
}

