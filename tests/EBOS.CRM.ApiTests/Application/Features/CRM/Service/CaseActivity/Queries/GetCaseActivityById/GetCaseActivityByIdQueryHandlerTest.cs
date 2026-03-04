using EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Queries.GetCaseActivityById;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.CaseActivity.Queries.GetCaseActivityById;

public class GetCaseActivityByIdQueryHandlerTest
{
    [Fact]
    public async Task Handle_WhenNotFound_ReturnsNull()
    {
        var repository = new Mock<ICaseActivityRepository>();
        var mapper = new Mock<IMapper>();
        repository.Setup(x => x.GetByIdAsync(404, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.CaseActivity?)null);

        var handler = new GetCaseActivityByIdQueryHandler(repository.Object, mapper.Object);
        var result = await handler.Handle(new GetCaseActivityByIdQuery(404), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenCanceled_ThrowsOperationCanceled()
    {
        var repository = new Mock<ICaseActivityRepository>();
        var mapper = new Mock<IMapper>();
        var handler = new GetCaseActivityByIdQueryHandler(repository.Object, mapper.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(new GetCaseActivityByIdQuery(1), cts.Token));
    }
}
