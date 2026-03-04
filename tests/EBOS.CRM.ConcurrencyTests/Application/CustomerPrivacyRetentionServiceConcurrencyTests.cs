using EBOS.CRM.Application.Features.CRM.CustomerPrivacy;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using Moq;

namespace EBOS.CRM.ConcurrencyTests.Application;

public class CustomerPrivacyRetentionServiceConcurrencyTests
{
    [Fact]
    public async Task RunAsync_WhenConcurrentTenants_ProcessesEachTenantIsolationCorrectly()
    {
        var tenant1Old = CreateRetentionCandidate(tenantId: 1, customerId: 101, daysAgo: 120);
        var tenant2Old = CreateRetentionCandidate(tenantId: 2, customerId: 201, daysAgo: 150);
        var tenant2Recent = CreateRetentionCandidate(tenantId: 2, customerId: 202, daysAgo: 10);
        var allRequests = new List<CustomerPrivacyRequest> { tenant1Old, tenant2Old, tenant2Recent };

        var repo = new Mock<ICustomerPrivacyRequestRepository>();
        repo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                lock (allRequests)
                {
                    return allRequests.ToList();
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
                new() { TenantId = 1, Key = "customer360.privacy.retention.days", ValueJson = "90", UpdatedAt = DateTime.UtcNow, UpdatedBy = 1 },
                new() { TenantId = 2, Key = "customer360.privacy.retention.days", ValueJson = "90", UpdatedAt = DateTime.UtcNow, UpdatedBy = 1 }
            });

        var audit = new Mock<IAuditService>();
        audit.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var sut = new CustomerPrivacyRetentionService(repo.Object, tenantConfigRepo.Object, audit.Object);

        var task1 = sut.RunAsync(tenantId: 1, dryRun: false, retentionDays: null, batchSize: 50, actorUserId: 9,
            correlationId: "corr-t1", cancellationToken: CancellationToken.None);
        var task2 = sut.RunAsync(tenantId: 2, dryRun: false, retentionDays: null, batchSize: 50, actorUserId: 9,
            correlationId: "corr-t2", cancellationToken: CancellationToken.None);

        await Task.WhenAll(task1, task2);

        var result1 = task1.Result;
        var result2 = task2.Result;

        Assert.Equal(1, result1.Candidates);
        Assert.Equal(1, result1.Affected);
        Assert.Equal(1, result2.Candidates);
        Assert.Equal(1, result2.Affected);

        Assert.True(tenant1Old.Erased);
        Assert.True(tenant2Old.Erased);
        Assert.False(tenant2Recent.Erased);

        audit.Verify(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task RunAsync_WhenDryRunConcurrentByTenant_DoesNotMutateData()
    {
        var tenant1Old = CreateRetentionCandidate(tenantId: 1, customerId: 301, daysAgo: 120);
        var tenant2Old = CreateRetentionCandidate(tenantId: 2, customerId: 401, daysAgo: 120);
        var allRequests = new List<CustomerPrivacyRequest> { tenant1Old, tenant2Old };

        var repo = new Mock<ICustomerPrivacyRequestRepository>();
        repo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(allRequests);
        repo.Setup(x => x.UpdateAsync(It.IsAny<CustomerPrivacyRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        repo.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var tenantConfigRepo = new Mock<ITenantConfigurationRepository>();
        tenantConfigRepo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<TenantConfiguration>());

        var audit = new Mock<IAuditService>();
        audit.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var sut = new CustomerPrivacyRetentionService(repo.Object, tenantConfigRepo.Object, audit.Object);

        var task1 = sut.RunAsync(tenantId: 1, dryRun: true, retentionDays: 30, batchSize: 50, actorUserId: 7,
            correlationId: "dry-t1", cancellationToken: CancellationToken.None);
        var task2 = sut.RunAsync(tenantId: 2, dryRun: true, retentionDays: 30, batchSize: 50, actorUserId: 7,
            correlationId: "dry-t2", cancellationToken: CancellationToken.None);

        await Task.WhenAll(task1, task2);

        Assert.Equal(1, task1.Result.Candidates);
        Assert.Equal(0, task1.Result.Affected);
        Assert.Equal(1, task2.Result.Candidates);
        Assert.Equal(0, task2.Result.Affected);
        Assert.False(tenant1Old.Erased);
        Assert.False(tenant2Old.Erased);

        repo.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        audit.Verify(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static CustomerPrivacyRequest CreateRetentionCandidate(long tenantId, long customerId, int daysAgo)
    {
        var item = CustomerPrivacyRequest.Create(
            tenantId,
            customerId,
            CustomerPrivacyRequest.TypeAnonymize,
            requestedBy: 1,
            reason: "retention",
            correlationId: Guid.NewGuid().ToString("N"),
            requestedAt: DateTime.UtcNow.AddDays(-daysAgo));

        SetPrivate(item, nameof(CustomerPrivacyRequest.Status), CustomerPrivacyRequest.StatusCompleted);
        SetPrivate(item, nameof(CustomerPrivacyRequest.ProcessedAt), DateTime.UtcNow.AddDays(-daysAgo));
        return item;
    }

    private static void SetPrivate<T>(CustomerPrivacyRequest target, string propertyName, T value)
    {
        var prop = typeof(CustomerPrivacyRequest).GetProperty(propertyName);
        Assert.NotNull(prop);
        prop!.SetValue(target, value);
    }
}

