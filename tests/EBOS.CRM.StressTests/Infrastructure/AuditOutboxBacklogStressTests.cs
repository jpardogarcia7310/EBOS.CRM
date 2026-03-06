using System.Net;
using System.Text;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.Infrastructure.Services.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.StressTests.Infrastructure;

public class AuditOutboxBacklogStressTests
{
    [Fact]
    public async Task AuditOutbox_BacklogProcessing_ReportsThroughputAndLatency()
    {
        await using var db = CreateContext();
        var clientFactory = new StubHttpClientFactory(new StubHandler(statusCode: HttpStatusCode.OK, delayMs: 2));
        var metrics = new NoOpCustomer360Metrics();
        var options = Options.Create(new AuditOutboxOptions
        {
            Enabled = true,
            BatchSize = 50,
            MaxAttempts = 3
        });

        var sut = new AuditOutboxService(db, clientFactory, options, NullLogger<AuditOutboxService>.Instance, metrics);
        var messageCount = 300;

        for (var i = 0; i < messageCount; i++)
        {
            await sut.EnqueueAsync("InsertAudit",
                new AuditInsertRequest(
                    UserId: 1,
                    TimeStamp: DateTimeOffset.UtcNow,
                    Action: "UPDATE",
                    Entity: "Customer",
                    RegisterId: i + 1,
                    OldValues: null,
                    NewValues: "{\"v\":1}",
                    CorrelationId: $"stress-{i}"),
                null,
                CancellationToken.None);
        }

        var perBatchMs = new List<double>();
        var swTotal = System.Diagnostics.Stopwatch.StartNew();
        var processed = 0;

        while (processed < messageCount)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var sent = await sut.DispatchPendingAsync(CancellationToken.None);
            sw.Stop();
            perBatchMs.Add(sw.Elapsed.TotalMilliseconds);
            if (sent == 0)
            {
                break;
            }

            processed += sent;
        }

        swTotal.Stop();

        var pending = await db.AuditOutboxMessages.CountAsync(x => x.ProcessedAt == null);
        Assert.Equal(0, pending);
        Assert.Equal(messageCount, processed);

        var throughput = StressStatistics.ThroughputPerSecond(processed, swTotal.Elapsed);
        var p95 = StressStatistics.Percentile(perBatchMs, 95);
        var p99 = StressStatistics.Percentile(perBatchMs, 99);

        Assert.True(throughput > 10, $"Throughput too low: {throughput:F2} msg/s");
        Assert.True(p95 < 15000, $"p95 too high: {p95:F2} ms");
        Assert.True(p99 >= p95, $"p99 should be >= p95, got p95={p95:F2} ms, p99={p99:F2} ms");
        Assert.True(p99 < 20000, $"p99 too high: {p99:F2} ms");
    }

    private static CrmDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CrmDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;
        return new CrmDbContext(options);
    }

    private sealed class StubHttpClientFactory(HttpMessageHandler handler) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(handler) { BaseAddress = new Uri("http://localhost/") };
    }

    private sealed class StubHandler(HttpStatusCode statusCode, int delayMs) : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            await Task.Delay(delayMs, cancellationToken);
            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent("{\"success\":true,\"id\":1}", Encoding.UTF8, "application/json")
            };
        }
    }

    private sealed class NoOpCustomer360Metrics : ICustomer360Metrics
    {
        public void RecordMerge(long tenantId, int mergedCount, bool success) { }
        public void RecordDedupeQuery(long tenantId, int candidateCount, int scoreThreshold) { }
        public void RecordConsentEvent(long tenantId, string consentType, bool granted) { }
        public void RecordAuditOutboxEnqueue(string operation) { }
        public void RecordAuditOutboxDispatch(string operation, bool success) { }
        public void RecordConcurrencyConflict(bool exhaustedRetries) { }
        public EBOS.CRM.Domain.Interfaces.Services.CRM.Customer360MetricsSnapshot GetSnapshot()
            => new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null, null);
    }
}
