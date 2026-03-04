using EBOS.CRM.Application.Features.EBOS.ChannelType.Queries.GetChannelTypeById;
using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using MapsterMapper;
using Moq;
using ChannelTypeEntity = EBOS.CRM.Domain.Entities.EBOS.ChannelType;

namespace EBOS.CRM.ApiTests.Application.Features.EBOS.ChannelType.Queries.GetChannelTypeById;

public class GetChannelTypeByIdQueryHandlerTest
{
    [Fact]
    public async Task Handle_WhenExists_ReturnsDto()
    {
        var repository = new Mock<IChannelTypeRepository>();
        var mapper = new Mock<IMapper>();
        var entity = new ChannelTypeEntity { Id = 1, Descripcion = "Email", IsActive = true };
        repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        mapper.Setup(x => x.Map<ChannelTypeResponse>(entity)).Returns(new ChannelTypeResponse(1, "Email", true));

        var handler = new GetChannelTypeByIdQueryHandler(repository.Object, mapper.Object);
        var result = await handler.Handle(new GetChannelTypeByIdQuery(1), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ReturnsNull()
    {
        var repository = new Mock<IChannelTypeRepository>();
        var mapper = new Mock<IMapper>();
        repository.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((ChannelTypeEntity?)null);

        var handler = new GetChannelTypeByIdQueryHandler(repository.Object, mapper.Object);
        var result = await handler.Handle(new GetChannelTypeByIdQuery(99), CancellationToken.None);

        Assert.Null(result);
    }
}
