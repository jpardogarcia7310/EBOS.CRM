using EBOS.CRM.Api.Infrastructure.HealthChecks;
using EBOS.CRM.Api.Options;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;

namespace EBOS.CRM.ApiTests.Infrastructure.HealthChecks;

public class Customer360OperationalReadinessHealthCheckTest
{
    [Fact]
    public async Task CheckHealthAsync_WhenThresholdExceeded_ReturnsUnhealthy()
    {
        var options = new DbContextOptionsBuilder<CrmDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        await using var context = new CrmDbContext(options);
        context.AuditOutboxMessages.Add(new AuditOutboxMessage
        {
            Operation = "enqueue",
            Payload = "{}",
            CreatedAt = DateTime.UtcNow,
            NextAttemptAt = DateTime.UtcNow,
            AttemptCount = 0,
            ProcessedAt = null,
            LastError = null
        });
        await context.SaveChangesAsync();

        var metrics = new Mock<ICustomer360Metrics>();
        metrics.Setup(x => x.GetSnapshot()).Returns(new Customer360MetricsSnapshot(
            0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0,
            DateTimeOffset.UtcNow.AddHours(-1),
            null));

        var opts = Microsoft.Extensions.Options.Options.Create(new OperationalReadinessOptions
        {
            OutboxPendingCriticalThreshold = 1,
            OutboxPendingWarningThreshold = 1,
            OutboxFailedCriticalThreshold = 1,
            ConcurrencyFailuresCriticalThreshold = 1,
            OutboxDispatchStaleMinutesThreshold = 1
        });

        var sut = new Customer360OperationalReadinessHealthCheck(context, metrics.Object, opts);

        var result = await sut.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
    }
}
