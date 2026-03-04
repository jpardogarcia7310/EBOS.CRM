using EBOS.CRM.Application.Features.EBOS.TenantUsageMetric.Queries.GetTenantUsageMetricById;
using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using Moq;
using TenantUsageMetricEntity = EBOS.CRM.Domain.Entities.EBOS.TenantUsageMetric;

namespace EBOS.CRM.ApiTests.Application.Features.EBOS.TenantUsageMetric.Queries.GetTenantUsageMetricById;

public class GetTenantUsageMetricByIdQueryHandlerTest
{
    private readonly Mock<ITenantUsageMetricRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetTenantUsageMetricByIdQueryHandler _handler;

    public GetTenantUsageMetricByIdQueryHandlerTest()
    {
        _repositoryMock = new Mock<ITenantUsageMetricRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetTenantUsageMetricByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsMappedDto()
    {
        var entity = new TenantUsageMetricEntity
        {
            Id = 1,
            TenantId = 1,
            Metric = "api.calls",
            Value = 250,
            Unit = "count",
            PeriodStart = DateTime.UtcNow.AddDays(-7),
            PeriodEnd = DateTime.UtcNow,
            Source = "gateway"
        };
        var dto = new TenantUsageMetricResponse(
            entity.Id, entity.TenantId, entity.Metric, entity.Value, entity.Unit, entity.PeriodStart, entity.PeriodEnd, entity.Source, true);

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<TenantUsageMetricResponse>(entity)).Returns(dto);

        var query = new GetTenantUsageMetricByIdQuery(1);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(dto.Id, result.Id);
        Assert.Equal(dto.Metric, result.Metric);
        _repositoryMock.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<TenantUsageMetricResponse>(entity), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingId_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantUsageMetricEntity?)null);
        var query = new GetTenantUsageMetricByIdQuery(99);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
        _mapperMock.Verify(m => m.Map<TenantUsageMetricResponse>(It.IsAny<TenantUsageMetricEntity>()), Times.Never);
    }
}
