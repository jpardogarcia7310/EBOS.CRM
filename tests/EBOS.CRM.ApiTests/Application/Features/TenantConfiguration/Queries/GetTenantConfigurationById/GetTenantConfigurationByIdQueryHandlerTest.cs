using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Application.Features.EBOS.TenantConfiguration.Queries.GetTenantConfigurationById;
using TenantConfigurationEntity = EBOS.CRM.Domain.Entities.EBOS.TenantConfiguration;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.TenantConfiguration.Queries.GetTenantConfigurationById;

public class GetTenantConfigurationByIdQueryHandlerTest
{
    private readonly Mock<ITenantConfigurationRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetTenantConfigurationByIdQueryHandler _handler;

    public GetTenantConfigurationByIdQueryHandlerTest()
    {
        _repositoryMock = new Mock<ITenantConfigurationRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetTenantConfigurationByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsMappedDto()
    {
        var entity = new TenantConfigurationEntity
        {
            Id = 1,
            TenantId = 1,
            Key = "limits.maxUsers",
            ValueJson = "{\"value\":25}",
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = 10
        };
        var dto = new TenantConfigurationResponse(
            entity.Id, entity.TenantId, entity.Key, entity.ValueJson, true);

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<TenantConfigurationResponse>(entity)).Returns(dto);

        var query = new GetTenantConfigurationByIdQuery(1);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(dto.Id, result.Id);
        Assert.Equal(dto.Key, result.Key);
        _repositoryMock.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<TenantConfigurationResponse>(entity), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingId_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantConfigurationEntity?)null);
        var query = new GetTenantConfigurationByIdQuery(99);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
        _mapperMock.Verify(m => m.Map<TenantConfigurationResponse>(It.IsAny<TenantConfigurationEntity>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RepositoryThrows_PropagatesException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));
        var query = new GetTenantConfigurationByIdQuery(1);

        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CancellationRequested_ThrowsOperationCanceled()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var query = new GetTenantConfigurationByIdQuery(1);

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(query, cts.Token));
    }
}
