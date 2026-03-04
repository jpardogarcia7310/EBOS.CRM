using EBOS.CRM.Application.Features.CRM.Service.Case.Queries.GetAllCases;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Case.Queries.GetAllCases;

public class GetAllCasesQueryHandlerTest
{
    [Fact]
    public async Task Handle_WhenCanceled_ThrowsOperationCanceled()
    {
        var repository = new Mock<ICaseRepository>();
        var mapper = new Mock<IMapper>();
        var handler = new GetAllCasesQueryHandler(repository.Object, mapper.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(new GetAllCasesQuery(), cts.Token));
    }
}
