using System.Net;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Service;

public class CaseTenantIsolationTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly string _version;

    public CaseTenantIsolationTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _version = ApiVersionHelper.GetLatestVersion(factory, "Case");
    }

    [Fact]
    public async Task GetAll_Filters_By_Tenant_Header()
    {
        var ids = SeedCases();

        var clientTenant1 = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var response1 = await clientTenant1.GetAsync($"/api/v{_version}/Case");
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant1 = await response1.Content.ReadItemsAsync<CaseResponse>();

        itemsTenant1.Should().Contain(i => i.Id == ids.Tenant1CaseId && i.Active);
        itemsTenant1.Should().NotContain(i => i.Id == ids.Tenant2CaseId);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(_factory, 2);
        var response2 = await clientTenant2.GetAsync($"/api/v{_version}/Case");
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant2 = await response2.Content.ReadItemsAsync<CaseResponse>();

        itemsTenant2.Should().Contain(i => i.Id == ids.Tenant2CaseId);
        itemsTenant2.Should().NotContain(i => i.Id == ids.Tenant1CaseId);
    }

    private (long Tenant1CaseId, long Tenant2CaseId) SeedCases()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

        var sla1 = new global::EBOS.CRM.Domain.Entities.CRM.Sla
        {
            TenantId = 1,
            Name = $"SLA-1-{Guid.NewGuid():N}",
            TargetMinutes = 60,
            WarningMinutes = 30,
            IsActive = true
        };
        var sla2 = new global::EBOS.CRM.Domain.Entities.CRM.Sla
        {
            TenantId = 2,
            Name = $"SLA-2-{Guid.NewGuid():N}",
            TargetMinutes = 60,
            WarningMinutes = 30,
            IsActive = true
        };

        var queue1 = new global::EBOS.CRM.Domain.Entities.CRM.Queue
        {
            TenantId = 1,
            Name = "Queue-1",
            Code = "Q1",
            IsActive = true
        };
        var queue2 = new global::EBOS.CRM.Domain.Entities.CRM.Queue
        {
            TenantId = 2,
            Name = "Queue-2",
            Code = "Q2",
            IsActive = true
        };

        db.Slas.AddRange(sla1, sla2);
        db.Queues.AddRange(queue1, queue2);
        db.SaveChanges();

        var case1 = new global::EBOS.CRM.Domain.Entities.CRM.Case
        {
            TenantId = 1,
            Title = "Case-1",
            Status = global::EBOS.CRM.Domain.Entities.CRM.Case.StatusOpen,
            Priority = global::EBOS.CRM.Domain.Entities.CRM.Case.PriorityLow,
            OwnerUserId = 10,
            QueueId = queue1.Id,
            SlaId = sla1.Id,
            CreatedAt = DateTime.UtcNow
        };
        var case2 = new global::EBOS.CRM.Domain.Entities.CRM.Case
        {
            TenantId = 2,
            Title = "Case-2",
            Status = global::EBOS.CRM.Domain.Entities.CRM.Case.StatusOpen,
            Priority = global::EBOS.CRM.Domain.Entities.CRM.Case.PriorityLow,
            OwnerUserId = 10,
            QueueId = queue2.Id,
            SlaId = sla2.Id,
            CreatedAt = DateTime.UtcNow
        };

        db.Cases.AddRange(case1, case2);
        db.SaveChanges();

        return (case1.Id, case2.Id);
    }
}
