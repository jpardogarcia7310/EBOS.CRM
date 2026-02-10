using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.EBOS.TenantQuota.Queries.GetTenantQuotaById;
using TenantQuotaEntity = EBOS.CRM.Domain.Entities.CRM.TenantQuota;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.TenantQuota.Queries.GetTenantQuotaById;

public class GetTenantQuotaByIdQueryHandlerTest
{
    private readonly Mock<ITenantQuotaRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetTenantQuotaByIdQueryHandler _handler;

    public GetTenantQuotaByIdQueryHandlerTest()
    {
        _repositoryMock = new Mock<ITenantQuotaRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetTenantQuotaByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsMappedDto()
    {
        var entity = new TenantQuotaEntity
        {
            Id = 1,
            TenantId = 1,
            Metric = "users",
            Limit = 100,
            Unit = "count",
            EffectiveFrom = DateTime.UtcNow.AddDays(-1),
            EffectiveTo = null
        };
        var dto = new TenantQuotaResponse(entity.Id, entity.TenantId, entity.Metric, entity.Limit, entity.Unit,
            entity.EffectiveFrom, entity.EffectiveTo, true);

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<TenantQuotaResponse>(entity)).Returns(dto);

        var query = new GetTenantQuotaByIdQuery(1);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(dto.Id, result.Id);
        Assert.Equal(dto.Metric, result.Metric);
        _repositoryMock.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<TenantQuotaResponse>(entity), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingId_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TenantQuotaEntity?)null);
        var query = new GetTenantQuotaByIdQuery(99);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
        _mapperMock.Verify(m => m.Map<TenantQuotaResponse>(It.IsAny<TenantQuotaEntity>()), Times.Never);
    }
}
