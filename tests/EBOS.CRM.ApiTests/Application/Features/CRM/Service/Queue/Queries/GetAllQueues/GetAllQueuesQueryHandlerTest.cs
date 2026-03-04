using EBOS.CRM.Application.Features.CRM.Service.Queue.Queries.GetAllQueues;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Queue.Queries.GetAllQueues;

public class GetAllQueuesQueryHandlerTest
{
    [Fact]
    public async Task Handle_WhenCanceled_ThrowsOperationCanceled()
    {
        var repository = new Mock<IQueueRepository>();
        var mapper = new Mock<IMapper>();
        var handler = new GetAllQueuesQueryHandler(repository.Object, mapper.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(new GetAllQueuesQuery(), cts.Token));
    }
}
