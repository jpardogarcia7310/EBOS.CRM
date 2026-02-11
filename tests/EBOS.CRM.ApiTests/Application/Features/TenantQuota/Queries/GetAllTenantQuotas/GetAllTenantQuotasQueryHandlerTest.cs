using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Contracts.Responses.EBOS;
using EBOS.CRM.Application.Features.EBOS.TenantQuota.Queries.GetAllTenantQuotas;
using TenantQuotaEntity = EBOS.CRM.Domain.Entities.EBOS.TenantQuota;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.TenantQuota.Queries.GetAllTenantQuotas;

public class GetAllTenantQuotasQueryHandlerTest
{
    private readonly Mock<ITenantQuotaRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllTenantQuotasQueryHandler _handler;

    public GetAllTenantQuotasQueryHandlerTest()
    {
        _repositoryMock = new Mock<ITenantQuotaRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllTenantQuotasQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ItemsExist_ReturnsMappedDtos()
    {
        var entities = new List<TenantQuotaEntity>
        {
            new()
            {
                Id = 1,
                TenantId = 1,
                Metric = "users",
                Limit = 100,
                Unit = "count",
                EffectiveFrom = DateTime.UtcNow.AddDays(-1)
            }
        };
        var dtos = new List<TenantQuotaResponse>
        {
            new(1, 1, "users", 100, "count", entities[0].EffectiveFrom, null, true)
        };

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities.Count);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<TenantQuotaResponse>>(entities))
            .Returns(dtos);

        var query = new GetAllTenantQuotasQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("users", result.Items.First().Metric);
    }
}
