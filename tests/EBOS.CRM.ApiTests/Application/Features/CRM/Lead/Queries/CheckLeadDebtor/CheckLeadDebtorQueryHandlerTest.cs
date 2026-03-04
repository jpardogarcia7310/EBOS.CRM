using EBOS.CRM.Application.Features.CRM.Lead.Queries.CheckLeadDebtor;
using EBOS.CRM.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Lead.Queries.CheckLeadDebtor;

public class CheckLeadDebtorQueryHandlerTest
{
    [Fact]
    public async Task Handle_WhenServiceReturnsData_MapsResponse()
    {
        var service = new Mock<ILeadDebtorCheckService>();
        service.Setup(x => x.CheckAsync(It.IsAny<LeadDebtorCheckRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LeadDebtorCheckResponse(true, 10, "Corporate", "C-1", "ACME", "john@acme.com", "111", 1, "Debtor", DateTime.UtcNow.AddDays(-30), 1000m));

        var handler = new CheckLeadDebtorQueryHandler(service.Object);
        var result = await handler.Handle(new CheckLeadDebtorQuery(new LeadDebtorCheckRequest(1, "john@acme.com", "111", "ACME", "John")), CancellationToken.None);

        Assert.True(result.IsDebtor);
        Assert.Equal(10, result.CustomerId);
    }

    [Fact]
    public async Task Handle_WhenCanceled_ThrowsOperationCanceled()
    {
        var service = new Mock<ILeadDebtorCheckService>();
        var handler = new CheckLeadDebtorQueryHandler(service.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(new CheckLeadDebtorQuery(new LeadDebtorCheckRequest(1, null, null, null, null)), cts.Token));
    }
}
