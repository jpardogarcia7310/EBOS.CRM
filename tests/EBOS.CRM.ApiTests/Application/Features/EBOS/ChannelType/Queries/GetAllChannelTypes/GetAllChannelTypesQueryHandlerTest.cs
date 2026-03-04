using EBOS.CRM.Application.Features.EBOS.ChannelType.Queries.GetAllChannelTypes;
using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using Moq;
using ChannelTypeEntity = EBOS.CRM.Domain.Entities.EBOS.ChannelType;

namespace EBOS.CRM.ApiTests.Application.Features.EBOS.ChannelType.Queries.GetAllChannelTypes;

public class GetAllChannelTypesQueryHandlerTest
{
    [Fact]
    public async Task Handle_ReturnsPagedResult()
    {
        var repository = new Mock<IChannelTypeRepository>();
        var mapper = new Mock<IMapper>();
        var entities = new List<ChannelTypeEntity> { new() { Id = 1, Descripcion = "Email", IsActive = true } };
        repository.Setup(x => x.GetAllPagedAsync(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(entities);
        repository.Setup(x => x.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        mapper.Setup(x => x.Map<IReadOnlyCollection<ChannelTypeResponse>>(entities))
            .Returns(new List<ChannelTypeResponse> { new(1, "Email", true) });

        var handler = new GetAllChannelTypesQueryHandler(repository.Object, mapper.Object);
        var result = await handler.Handle(new GetAllChannelTypesQuery(1, 10), CancellationToken.None);

        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
    }
}
