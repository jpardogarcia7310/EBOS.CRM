using EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.GetAllSlas;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Sla.Queries.GetAllSlas;

public class GetAllSlasQueryHandlerTest
{
    [Fact]
    public async Task Handle_WhenCanceled_ThrowsOperationCanceled()
    {
        var repository = new Mock<ISlaRepository>();
        var mapper = new Mock<IMapper>();
        var handler = new GetAllSlasQueryHandler(repository.Object, mapper.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(new GetAllSlasQuery(), cts.Token));
    }
}
