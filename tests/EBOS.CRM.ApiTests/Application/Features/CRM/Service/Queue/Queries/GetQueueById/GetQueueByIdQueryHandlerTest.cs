using EBOS.CRM.Application.Features.CRM.Service.Queue.Queries.GetQueueById;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Queue.Queries.GetQueueById;

public class GetQueueByIdQueryHandlerTest
{
    [Fact]
    public async Task Handle_WhenNotFound_ReturnsNull()
    {
        var repository = new Mock<IQueueRepository>();
        var mapper = new Mock<IMapper>();
        repository.Setup(x => x.GetByIdAsync(404, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.Queue?)null);

        var handler = new GetQueueByIdQueryHandler(repository.Object, mapper.Object);
        var result = await handler.Handle(new GetQueueByIdQuery(404), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenCanceled_ThrowsOperationCanceled()
    {
        var repository = new Mock<IQueueRepository>();
        var mapper = new Mock<IMapper>();
        var handler = new GetQueueByIdQueryHandler(repository.Object, mapper.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(new GetQueueByIdQuery(1), cts.Token));
    }
}
