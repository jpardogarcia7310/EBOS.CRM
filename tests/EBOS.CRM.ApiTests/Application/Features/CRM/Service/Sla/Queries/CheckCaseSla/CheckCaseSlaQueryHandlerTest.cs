using EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.CheckCaseSla;
using EBOS.CRM.Contracts.Requests.CRM.Service.Sla;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Sla.Queries.CheckCaseSla;

public class CheckCaseSlaQueryHandlerTest
{
    [Fact]
    public async Task Handle_WhenSlaMissing_ReturnsNull()
    {
        var caseRepository = new Mock<ICaseRepository>();
        var slaRepository = new Mock<ISlaRepository>();
        caseRepository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new global::EBOS.CRM.Domain.Entities.CRM.Case
            {
                Id = 1, TenantId = 1, SlaId = 100, Status = global::EBOS.CRM.Domain.Entities.CRM.Case.StatusOpen,
                Priority = global::EBOS.CRM.Domain.Entities.CRM.Case.PriorityLow, OwnerUserId = 1, QueueId = 1, CreatedAt = DateTime.UtcNow
            });
        slaRepository.Setup(x => x.GetByIdAsync(100, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.Sla?)null);

        var handler = new CheckCaseSlaQueryHandler(caseRepository.Object, slaRepository.Object);
        var result = await handler.Handle(new CheckCaseSlaQuery(new CheckCaseSlaRequest(1, 1, DateTime.UtcNow)), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenCanceled_ThrowsOperationCanceled()
    {
        var caseRepository = new Mock<ICaseRepository>();
        var slaRepository = new Mock<ISlaRepository>();
        var handler = new CheckCaseSlaQueryHandler(caseRepository.Object, slaRepository.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(new CheckCaseSlaQuery(new CheckCaseSlaRequest(1, 1, DateTime.UtcNow)), cts.Token));
    }
}
