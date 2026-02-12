using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Application.Features.EBOS.TenantUsageMetric.Queries.GetAllTenantUsageMetrics;
using TenantUsageMetricEntity = EBOS.CRM.Domain.Entities.EBOS.TenantUsageMetric;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.TenantUsageMetric.Queries.GetAllTenantUsageMetrics;

public class GetAllTenantUsageMetricsQueryHandlerTest
{
    private readonly Mock<ITenantUsageMetricRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllTenantUsageMetricsQueryHandler _handler;

    public GetAllTenantUsageMetricsQueryHandlerTest()
    {
        _repositoryMock = new Mock<ITenantUsageMetricRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllTenantUsageMetricsQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ItemsExist_ReturnsMappedDtos()
    {
        var entities = new List<TenantUsageMetricEntity>
        {
            new()
            {
                Id = 1,
                TenantId = 1,
                Metric = "api.calls",
                Value = 250,
                Unit = "count",
                PeriodStart = DateTime.UtcNow.AddDays(-7),
                PeriodEnd = DateTime.UtcNow,
                Source = "gateway"
            }
        };
        var dtos = new List<TenantUsageMetricResponse>
        {
            new(1, 1, "api.calls", 250, "count", entities[0].PeriodStart, entities[0].PeriodEnd, "gateway", true)
        };

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities.Count);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<TenantUsageMetricResponse>>(entities))
            .Returns(dtos);

        var query = new GetAllTenantUsageMetricsQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("api.calls", result.Items.First().Metric);
    }
}
