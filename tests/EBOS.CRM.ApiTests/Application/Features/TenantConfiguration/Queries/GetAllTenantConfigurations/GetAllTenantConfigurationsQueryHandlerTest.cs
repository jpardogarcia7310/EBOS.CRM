using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Contracts.Responses.EBOS;
using EBOS.CRM.Application.Features.EBOS.TenantConfiguration.Queries.GetAllTenantConfigurations;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using Moq;
using TenantConfigurationEntity = EBOS.CRM.Domain.Entities.EBOS.TenantConfiguration;

namespace EBOS.CRM.ApiTests.Application.Features.TenantConfiguration.Queries.GetAllTenantConfigurations;

public class GetAllTenantConfigurationsQueryHandlerTest
{
    private readonly Mock<ITenantConfigurationRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllTenantConfigurationsQueryHandler _handler;

    public GetAllTenantConfigurationsQueryHandlerTest()
    {
        _repositoryMock = new Mock<ITenantConfigurationRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllTenantConfigurationsQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ItemsExist_ReturnsMappedDtos()
    {
        var entities = new List<TenantConfigurationEntity>
        {
            new()
            {
                Id = 1,
                TenantId = 1,
                Key = "limits.maxUsers",
                ValueJson = "{\"value\":25}",
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = 10
            }
        };
        var dtos = new List<TenantConfigurationResponse>
        {
            new(1, 1, "limits.maxUsers", "{\"value\":25}", true)
        };

        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities.Count);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<TenantConfigurationResponse>>(entities))
            .Returns(dtos);

        var query = new GetAllTenantConfigurationsQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("limits.maxUsers", result.Items.First().Key);
    }

    [Fact]
    public async Task Handle_NoItems_ReturnsEmpty()
    {
        _repositoryMock.Setup(r => r.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TenantConfigurationEntity>());
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _mapperMock.Setup(m => m.Map<IReadOnlyCollection<TenantConfigurationResponse>>(It.IsAny<IReadOnlyCollection<TenantConfigurationEntity>>()))
            .Returns(new List<TenantConfigurationResponse>());

        var query = new GetAllTenantConfigurationsQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }
}
