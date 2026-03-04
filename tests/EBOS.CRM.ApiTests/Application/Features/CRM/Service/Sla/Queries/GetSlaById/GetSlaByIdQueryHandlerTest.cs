using EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.GetSlaById;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Sla.Queries.GetSlaById;

public class GetSlaByIdQueryHandlerTest
{
    [Fact]
    public async Task Handle_WhenNotFound_ReturnsNull()
    {
        var repository = new Mock<ISlaRepository>();
        var mapper = new Mock<IMapper>();
        repository.Setup(x => x.GetByIdAsync(404, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.Sla?)null);

        var handler = new GetSlaByIdQueryHandler(repository.Object, mapper.Object);
        var result = await handler.Handle(new GetSlaByIdQuery(404), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenCanceled_ThrowsOperationCanceled()
    {
        var repository = new Mock<ISlaRepository>();
        var mapper = new Mock<IMapper>();
        var handler = new GetSlaByIdQueryHandler(repository.Object, mapper.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(new GetSlaByIdQuery(1), cts.Token));
    }
}
