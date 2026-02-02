using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Features.Statuses.Queries.GetAllStatuses;
using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Interfaces.Repositories;
using MapsterMapper;
using Moq;
using EBOS.CRM.Domain.Primitives.Paging;
using EBOS.CRM.Application.Contracts.Requests.Common;

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
            new() { Id = 1, Description = "Active" }
        };
        var dtos = new List<StatusResponse>
        {
            new(1, "Active")
        };

        _repositoryMock.Setup(r => r.GetPagedAsync(It.IsAny<PagedQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Status>(statuses, 1, 50, statuses.Count, statuses.Count == 0 ? 0 : 1, null, null, null));
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<StatusResponse>>(statuses)).Returns(dtos);

        var query = new GetAllStatusesQuery(new PagedQueryRequest());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Active", result.Items.First().Description);
        _repositoryMock.Verify(r => r.GetPagedAsync(It.IsAny<PagedQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _mapperMock.Verify(m => m.Map<IReadOnlyCollection<StatusResponse>>(statuses), Times.Once);
    }

    [Fact]
    public async Task Handle_NoCountries_ReturnsEmptyEnumerable()
    {
        // Arrange
        var statuses = new List<Status>();
        var dtos = new List<StatusResponse>();

        _repositoryMock.Setup(r => r.GetPagedAsync(It.IsAny<PagedQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Status>(statuses, 1, 50, statuses.Count, statuses.Count == 0 ? 0 : 1, null, null, null));
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<StatusResponse>>(statuses)).Returns(dtos);

        var query = new GetAllStatusesQuery(new PagedQueryRequest());

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
        _repositoryMock.Setup(r => r.GetPagedAsync(It.IsAny<PagedQuery>(), It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new Exception("DB error"));
        var query = new GetAllStatusesQuery(new PagedQueryRequest());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CancellationRequested_ThrowsOperationCanceled()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var query = new GetAllStatusesQuery(new PagedQueryRequest());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(query, cts.Token));
    }

    [Fact]
    public async Task Handle_MapperConfigurationInvalid_ThrowsMappingException()
    {
        // Arrange
        var statuses = new List<Status> { new() { Id = 1, Description = "Active" } };
        _repositoryMock.Setup(r => r.GetPagedAsync(It.IsAny<PagedQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Status>(statuses, 1, 50, statuses.Count, statuses.Count == 0 ? 0 : 1, null, null, null));

        // We simulated that the Mapster mapper failed
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<StatusResponse>>(statuses))
                   .Throws(new InvalidOperationException("Mapping failed"));

        var query = new GetAllStatusesQuery(new PagedQueryRequest());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NullEntityProperty_MapsGracefully()
    {
        // Arrange
        var statuses = new List<Status> { new() { Id = 1, Description = null! } };
        var dtos = new List<StatusResponse> { new(1, null!) };

        _repositoryMock.Setup(r => r.GetPagedAsync(It.IsAny<PagedQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Status>(statuses, 1, 50, statuses.Count, statuses.Count == 0 ? 0 : 1, null, null, null));
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<StatusResponse>>(statuses)).Returns(dtos);

        var query = new GetAllStatusesQuery(new PagedQueryRequest());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Null(result.Items.First().Description);
    }

    [Fact]
    public async Task Handle_MapperCalledWithCorrectSourceType()
    {
        // Arrange
        var statuses = new List<Status>();
        _repositoryMock.Setup(r => r.GetPagedAsync(It.IsAny<PagedQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Status>(statuses, 1, 50, statuses.Count, statuses.Count == 0 ? 0 : 1, null, null, null));

        var query = new GetAllStatusesQuery(new PagedQueryRequest());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mapperMock.Verify(m => m.Map<IReadOnlyCollection<StatusResponse>>(statuses), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryCalledOnce_WithCancellationToken()
    {
        // Arrange
        var query = new GetAllStatusesQuery(new PagedQueryRequest());
        var statuses = new List<Status>();
        _repositoryMock.Setup(r => r.GetPagedAsync(It.IsAny<PagedQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Status>(statuses, 1, 50, statuses.Count, statuses.Count == 0 ? 0 : 1, null, null, null));

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetPagedAsync(It.IsAny<PagedQuery>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}






