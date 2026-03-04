using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM.Models;
using EBOS.CRM.Infrastructure.Options;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.StressTests.Infrastructure;

public class CustomerMergeDedupeHighVolumeStressTests
{
    [Fact]
    public async Task CustomerMerge_DedupeHighVolume_ReportsThroughputAndLatency()
    {
        await using var db = CreateContext();
        await SeedCustomersAsync(db, tenantId: 1, total: 5000, duplicateGroupSize: 250);

        var repo = new CustomerDedupeRepository(db, Options.Create(new CustomerDedupeOptions
        {
            MinScore = 50,
            EmailWeight = 70,
            PhoneWeight = 30,
            EnablePhoneSuffixFallback = true,
            PhoneSuffixLength = 6,
            PhoneApproxWeight = 10
        }));

        var criteria = new CustomerDedupeCriteria(
            TenantId: 1,
            Email: "dup@example.com",
            Phone: null,
            TaxId: null,
            IdentificationNumber: null);

        const int queries = 120;
        var latencies = new List<double>(queries);
        var totalCandidates = 0;
        var totalCount = 0;
        var swTotal = System.Diagnostics.Stopwatch.StartNew();

        for (var i = 0; i < queries; i++)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var page = await repo.FindDuplicatesAsync(criteria, pageNumber: 1, pageSize: 100, CancellationToken.None);
            var count = await repo.CountDuplicatesAsync(criteria, CancellationToken.None);
            sw.Stop();

            latencies.Add(sw.Elapsed.TotalMilliseconds);
            totalCandidates += page.Count;
            totalCount += count;
        }

        swTotal.Stop();

        Assert.True(totalCandidates > 0);
        Assert.True(totalCount > 0);

        var throughput = StressStatistics.ThroughputPerSecond(queries, swTotal.Elapsed);
        var p95 = StressStatistics.Percentile(latencies, 95);
        var p99 = StressStatistics.Percentile(latencies, 99);

        Assert.True(throughput > 3, $"Throughput too low: {throughput:F2} qps");
        Assert.True(p95 < 1000, $"p95 too high: {p95:F2} ms");
        Assert.True(p99 < 1500, $"p99 too high: {p99:F2} ms");
    }

    private static CrmDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CrmDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;
        return new CrmDbContext(options);
    }

    private static async Task SeedCustomersAsync(CrmDbContext db, long tenantId, int total, int duplicateGroupSize)
    {
        var status = new Status
        {
            Description = "Active",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1
        };
        db.Statuses.Add(status);
        await db.SaveChangesAsync();

        var customers = new List<Customer>(total);
        for (var i = 0; i < total; i++)
        {
            var isDuplicate = i < duplicateGroupSize;
            customers.Add(new Customer
            {
                TenantId = tenantId,
                Code = $"C-{i:D6}",
                Email = isDuplicate ? "dup@example.com" : $"u{i}@example.com",
                Phone = $"346{(1000000 + i):D7}",
                StatusId = status.Id,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1
            });
        }

        db.Customers.AddRange(customers);
        await db.SaveChangesAsync();
    }
}
