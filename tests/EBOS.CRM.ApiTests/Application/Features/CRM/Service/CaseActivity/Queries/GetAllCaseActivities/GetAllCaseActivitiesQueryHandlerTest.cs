using EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Queries.GetAllCaseActivities;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.CaseActivity.Queries.GetAllCaseActivities;

public class GetAllCaseActivitiesQueryHandlerTest
{
    [Fact]
    public async Task Handle_WhenCanceled_ThrowsOperationCanceled()
    {
        var repository = new Mock<ICaseActivityRepository>();
        var mapper = new Mock<IMapper>();
        var handler = new GetAllCaseActivitiesQueryHandler(repository.Object, mapper.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(new GetAllCaseActivitiesQuery(), cts.Token));
    }
}
