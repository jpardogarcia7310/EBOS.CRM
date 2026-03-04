using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Contracts.Requests.CRM.CustomerPreference;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.CustomerPreference;

public class CustomerPreferenceEndpointTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _tenant1 = HttpClientFactory.CreateClientWithTenant(factory, 1);
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CustomerPreference");
    private readonly string _customerVersion = ApiVersionHelper.GetLatestVersion(factory, "Customer");
    private readonly string _statusVersion = ApiVersionHelper.GetLatestVersion(factory, "Status");

    [Fact]
    public async Task HappyPath_Upsert_And_GetByCustomer_Work()
    {
        var customer = await CreateCustomerAsync(1);
        var channelId = GetOrCreateActiveChannelTypeId(factory);

        var upsert = await _tenant1.PutAsJsonAsync(
            $"/api/v{_version}/CustomerPreference",
            new UpsertCustomerPreferenceRequest(1, customer.Id, channelId, true));
        upsert.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await _tenant1.GetAsync($"/api/v{_version}/CustomerPreference/by-customer/{customer.Id}?tenantId=1");
        list.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Upsert_WithTenantMismatch_ReturnsBadRequest()
    {
        var customer = await CreateCustomerAsync(1);
        var channelId = GetOrCreateActiveChannelTypeId(factory);
        var response = await _tenant1.PutAsJsonAsync(
            $"/api/v{_version}/CustomerPreference",
            new UpsertCustomerPreferenceRequest(2, customer.Id, channelId, false));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetByCustomer_WithInvalidTenant_ReturnsBadRequest()
    {
        var response = await _tenant1.GetAsync($"/api/v{_version}/CustomerPreference/by-customer/1?tenantId=0");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<CustomerResponse> CreateCustomerAsync(long tenantId)
    {
        var statusId = await LookupHelper.GetStatusIdAsync(_tenant1, _statusVersion);
        var response = await _tenant1.PostAsJsonAsync(
            $"/api/v{_customerVersion}/Customer",
            new AddCustomerRequest(
                tenantId,
                $"C-{Guid.NewGuid():N}"[..12],
                $"pref-{Guid.NewGuid():N}@example.com",
                "34600000002",
                statusId));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        created.Should().NotBeNull();
        return created!;
    }

    private static long GetOrCreateActiveChannelTypeId(CustomWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var existing = db.ChannelTypes.Where(x => x.IsActive).Select(x => (long?)x.Id).FirstOrDefault();
        if (existing.HasValue)
        {
            return existing.Value;
        }

        var channel = new ChannelType
        {
            Descripcion = $"IT-{Guid.NewGuid():N}"[..16],
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 1,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = 1
        };
        db.ChannelTypes.Add(channel);
        db.SaveChanges();
        return channel.Id;
    }
}
