using EBOS.CRM.Application.Features.CRM.CustomerPrivacy;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.StressTests.Infrastructure;
using Moq;

namespace EBOS.CRM.StressTests.Application;

public class CustomerPrivacyRetentionHighVolumeStressTests
{
    [Fact]
    public async Task CustomerPrivacyRetention_HighVolumePerTenant_ReportsThroughputAndLatency()
    {
        var all = BuildRequests(tenantCount: 3, requestsPerTenant: 1200);

        var repo = new Mock<ICustomerPrivacyRequestRepository>();
        repo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                lock (all)
                {
                    return all.ToList();
                }
            });
        repo.Setup(x => x.UpdateAsync(It.IsAny<CustomerPrivacyRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        repo.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var tenantConfigRepo = new Mock<ITenantConfigurationRepository>();
        tenantConfigRepo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TenantConfiguration>
            {
                BuildTenantConfig(1, 90),
                BuildTenantConfig(2, 90),
                BuildTenantConfig(3, 90)
            });

        var audit = new Mock<IAuditService>();
        audit.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var sut = new CustomerPrivacyRetentionService(repo.Object, tenantConfigRepo.Object, audit.Object);
        var latencies = new List<double>();
        var affectedTotal = 0;
        var swTotal = System.Diagnostics.Stopwatch.StartNew();

        foreach (var tenantId in new long[] { 1, 2, 3 })
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var result = await sut.RunAsync(tenantId, dryRun: false, retentionDays: null, batchSize: 2000, actorUserId: 9,
                correlationId: $"stress-ret-{tenantId}", cancellationToken: CancellationToken.None);
            sw.Stop();
            latencies.Add(sw.Elapsed.TotalMilliseconds);
            affectedTotal += result.Affected;
        }

        swTotal.Stop();

        Assert.True(affectedTotal > 0);
        Assert.Equal(3600, affectedTotal);

        var throughput = StressStatistics.ThroughputPerSecond(affectedTotal, swTotal.Elapsed);
        var p95 = StressStatistics.Percentile(latencies, 95);
        var p99 = StressStatistics.Percentile(latencies, 99);

        Assert.True(throughput > 100, $"Throughput too low: {throughput:F2} req/s");
        Assert.True(p95 < 2500, $"p95 too high: {p95:F2} ms");
        Assert.True(p99 < 3000, $"p99 too high: {p99:F2} ms");
    }

    private static List<CustomerPrivacyRequest> BuildRequests(int tenantCount, int requestsPerTenant)
    {
        var list = new List<CustomerPrivacyRequest>(tenantCount * requestsPerTenant);
        for (var tenant = 1; tenant <= tenantCount; tenant++)
        {
            for (var i = 0; i < requestsPerTenant; i++)
            {
                var item = CustomerPrivacyRequest.Create(
                    tenant,
                    customerId: (tenant * 100000) + i + 1,
                    requestType: CustomerPrivacyRequest.TypeAnonymize,
                    requestedBy: 1,
                    reason: "stress",
                    correlationId: $"{tenant}-{i}",
                    requestedAt: DateTime.UtcNow.AddDays(-120));

                SetPrivate(item, nameof(CustomerPrivacyRequest.Status), CustomerPrivacyRequest.StatusCompleted);
                SetPrivate(item, nameof(CustomerPrivacyRequest.ProcessedAt), DateTime.UtcNow.AddDays(-120));
                list.Add(item);
            }
        }

        return list;
    }

    private static TenantConfiguration BuildTenantConfig(long tenantId, int days) => new()
    {
        TenantId = tenantId,
        Key = "customer360.privacy.retention.days",
        ValueJson = days.ToString(),
        UpdatedAt = DateTime.UtcNow,
        UpdatedBy = 1
    };

    private static void SetPrivate<T>(CustomerPrivacyRequest target, string propertyName, T value)
    {
        var prop = typeof(CustomerPrivacyRequest).GetProperty(propertyName);
        Assert.NotNull(prop);
        prop!.SetValue(target, value);
    }
}
