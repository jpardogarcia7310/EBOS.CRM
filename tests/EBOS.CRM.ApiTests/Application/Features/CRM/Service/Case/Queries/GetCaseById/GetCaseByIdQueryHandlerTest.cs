using EBOS.CRM.Application.Features.CRM.Service.Case.Queries.GetCaseById;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Case.Queries.GetCaseById;

public class GetCaseByIdQueryHandlerTest
{
    [Fact]
    public async Task Handle_WhenNotFound_ReturnsNull()
    {
        var repository = new Mock<ICaseRepository>();
        var mapper = new Mock<IMapper>();
        repository.Setup(x => x.GetByIdAsync(404, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.Case?)null);

        var handler = new GetCaseByIdQueryHandler(repository.Object, mapper.Object);
        var result = await handler.Handle(new GetCaseByIdQuery(404), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenCanceled_ThrowsOperationCanceled()
    {
        var repository = new Mock<ICaseRepository>();
        var mapper = new Mock<IMapper>();
        var handler = new GetCaseByIdQueryHandler(repository.Object, mapper.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(new GetCaseByIdQuery(1), cts.Token));
    }
}
