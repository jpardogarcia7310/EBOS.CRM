using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Contracts.Requests.CRM.Service.Sla;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.CheckCaseSla;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Service;

public class CaseQueueSlaEndToEndTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly string _version;

    public CaseQueueSlaEndToEndTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _version = ApiVersionHelper.GetLatestVersion(factory, "Case");
    }

    [Fact]
    public async Task Case_List_Is_Tenant_Isolated()
    {
        var tenant1 = SeedQueueAndSla(1);
        var tenant2 = SeedQueueAndSla(2);

        var title1 = $"Case-{Guid.NewGuid():N}";
        var title2 = $"Case-{Guid.NewGuid():N}";

        var clientTenant1 = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var addResponse1 = await clientTenant1.PostAsJsonAsync(
            $"/api/v{_version}/Case",
            BuildAddCaseRequest(1, title1, tenant1.QueueId, tenant1.SlaId, DateTime.UtcNow.AddHours(2)));
        await EnsureSuccessAsync(addResponse1);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(_factory, 2);
        var addResponse2 = await clientTenant2.PostAsJsonAsync(
            $"/api/v{_version}/Case",
            BuildAddCaseRequest(2, title2, tenant2.QueueId, tenant2.SlaId, DateTime.UtcNow.AddHours(2)));
        await EnsureSuccessAsync(addResponse2);

        var responseTenant1 = await clientTenant1.GetAsync($"/api/v{_version}/Case");
        responseTenant1.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant1 = await responseTenant1.Content.ReadItemsAsync<CaseResponse>();

        itemsTenant1.Should().Contain(i => i.Title == title1);
        itemsTenant1.Should().NotContain(i => i.Title == title2);
    }

    [Fact]
    public async Task CheckCaseSla_Returns_Breached_For_Past_DueAt()
    {
        var tenant = SeedQueueAndSla(1);
        var clientTenant1 = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var title = $"Case-{Guid.NewGuid():N}";

        var addResponse = await clientTenant1.PostAsJsonAsync(
            $"/api/v{_version}/Case",
            BuildAddCaseRequest(1, title, tenant.QueueId, tenant.SlaId, DateTime.UtcNow.AddMinutes(-10)));
        await EnsureSuccessAsync(addResponse);

        var created = await addResponse.Content.ReadFromJsonAsync<CaseResponse>();
        created.Should().NotBeNull();

        using var scope = _factory.Services.CreateScope();
        var accessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
        accessor.HttpContext = new DefaultHttpContext();
        accessor.HttpContext.Request.Headers["X-Tenant-Id"] = "1";

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(new CheckCaseSlaQuery(new CheckCaseSlaRequest(
            TenantId: 1,
            CaseId: created.Id,
            Now: DateTime.UtcNow)));

        result.Should().NotBeNull();
        result.IsBreached.Should().BeTrue();
        result.IsActive.Should().BeTrue();
    }

    private (long QueueId, long SlaId) SeedQueueAndSla(long tenantId)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

        var queue = new Queue
        {
            TenantId = tenantId,
            Name = $"Queue-{tenantId}-{Guid.NewGuid():N}",
            Code = $"Q-{tenantId}-{Guid.NewGuid():N}".Substring(0, 10),
            IsActive = true,
            DefaultOwnerUserId = 1,
            CreatedBy = 1,
            Erased = false
        };

        var sla = new Sla
        {
            TenantId = tenantId,
            Name = $"SLA-{tenantId}-{Guid.NewGuid():N}",
            TargetMinutes = 30,
            WarningMinutes = 10,
            IsActive = true,
            CreatedBy = 1,
            Erased = false
        };

        db.Queues.Add(queue);
        db.Slas.Add(sla);
        db.SaveChanges();

        return (queue.Id, sla.Id);
    }

    private static AddCaseRequest BuildAddCaseRequest(long tenantId, string title, long queueId, long slaId,
        DateTime? dueAt)
    {
        return new AddCaseRequest(
            TenantId: tenantId,
            Title: title,
            Description: "Case description",
            Status: Case.StatusOpen,
            Priority: Case.PriorityLow,
            OwnerUserId: 1,
            QueueId: queueId,
            SlaId: slaId,
            DueAt: dueAt);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var payload = await response.Content.ReadAsStringAsync();
        throw new InvalidOperationException(
            $"Request failed with {(int)response.StatusCode} {response.StatusCode}. Body: {payload}");
    }
}
