using EBOS.CRM.Api.Controllers.Operations;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.ApiTests.Controllers.Operations;

public class OperationalReadinessResponseContractsTest
{
    [Fact]
    public void DashboardResponse_StoresValues()
    {
        var snapshot = new Customer360MetricsSnapshot(1,2,3,4,5,6,7,8,9,10,11,DateTimeOffset.UtcNow,null);
        var dto = new Customer360OperationalDashboardResponse(snapshot, 12, 13);

        Assert.Equal(12, dto.OutboxPending);
        Assert.Equal(13, dto.OutboxFailed);
        Assert.Equal(snapshot, dto.Snapshot);
    }

    [Fact]
    public void AlertsResponse_StoresFlags()
    {
        var dto = new Customer360OperationalAlertsResponse(true, false, true, false, true);

        Assert.True(dto.OutboxPendingWarning);
        Assert.False(dto.OutboxPendingCritical);
        Assert.True(dto.OutboxFailedCritical);
        Assert.False(dto.OutboxDispatchStale);
        Assert.True(dto.ConcurrencyCritical);
    }
}
