using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Application.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.ApiTests.TestUtils;
using EBOS.CRM.ApiTests.Fixtures;
using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.ApiTests.Controllers.CRM.Lead;

public class LeadControllerTest(CustomWebApplicationFactory<Program> factory) :
    IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "Lead");
    private readonly CustomWebApplicationFactory<Program> _factory = factory;

    [Fact]
    public async Task GetAllLeads_ReturnsSuccessAndList()
    {
        var response = await _client.GetAsync($"/api/v{_version}/Lead");
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadItemsAsync<LeadResponse>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetLeadById_ExistingId_ReturnsLead()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<LeadResponse>(
            _client, $"/api/v{_version}/Lead", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/Lead/{id}");
        response.EnsureSuccessStatusCode();

        var item = await response.Content.ReadFromJsonAsync<LeadResponse>();
        Assert.NotNull(item);
        Assert.Equal(id, item.Id);
    }

    [Fact]
    public async Task GetLeadById_NonExistingId_ReturnsNotFound()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<LeadResponse>(
            _client, $"/api/v{_version}/Lead", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/Lead/{id + 9999}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetLeadConversion_ReturnsConversionInfo()
    {
        var id = await ControllerTestHelper.GetFirstIdAsync<LeadResponse>(
            _client, $"/api/v{_version}/Lead", x => x.Id);

        var response = await _client.GetAsync($"/api/v{_version}/Lead/{id}/conversion");
        response.EnsureSuccessStatusCode();

        var conversion = await response.Content.ReadFromJsonAsync<LeadConversionResponse>();
        Assert.NotNull(conversion);
        Assert.Equal(id, conversion.LeadId);
    }

    [Fact]
    public async Task CheckDebtor_ReturnsDebtorDetails_WhenMorosoCustomerExists()
    {
        const string debtorEmail = "debtor@example.com";
        const string debtorPhone = "+34 600 000 999";

        SeedDebtorCustomer(debtorEmail, debtorPhone);

        var request = new LeadDebtorCheckRequest(1, debtorEmail, debtorPhone, null, "Jane Doe");
        var response = await _client.PostAsJsonAsync($"/api/v{_version}/Lead/debtor-check", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<LeadDebtorCheckResponse>();
        Assert.NotNull(result);
        Assert.True(result.IsDebtor);
        Assert.Equal("Moroso", result.Status);
        Assert.Equal(debtorEmail, result.Email);
        Assert.Equal(debtorPhone, result.Phone);
        Assert.True(result.DebtAmount > 0);
        Assert.NotNull(result.DebtorSince);
    }

    [Fact]
    public async Task CheckDebtor_ReturnsNotDebtor_WhenNoMatchFound()
    {
        var request = new LeadDebtorCheckRequest(1, "missing@example.com", "000000000", null, null);
        var response = await _client.PostAsJsonAsync($"/api/v{_version}/Lead/debtor-check", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<LeadDebtorCheckResponse>();
        Assert.NotNull(result);
        Assert.False(result.IsDebtor);
    }

    private void SeedDebtorCustomer(string email, string phone)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

        var morosoStatus = db.Statuses.FirstOrDefault(s => s.Description == "Moroso");
        if (morosoStatus == null)
        {
            morosoStatus = new global::EBOS.CRM.Domain.Entities.EBOS.Status { Description = "Moroso" };
            db.Statuses.Add(morosoStatus);
            db.SaveChanges();
        }

        var idType = db.IdentificationTypes.First();
        var customer = new global::EBOS.CRM.Domain.Entities.CRM.IndividualCustomer
        {
            TenantId = 1,
            Code = "IND-DBT",
            Email = email,
            Phone = phone,
            StatusId = morosoStatus.Id,
            FirstName = "Jane",
            LastName = "Doe",
            BirthDate = new DateTime(1990, 1, 1),
            IdentificationNumber = "99999999",
            IdentificationTypeId = idType.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-30)
        };
        db.IndividualCustomers.Add(customer);
        db.SaveChanges();

        db.CreditAccounts.Add(new global::EBOS.CRM.Domain.Entities.CRM.CreditAccount
        {
            TenantId = 1,
            CustomerId = customer.Id,
            MaxAmount = 1000m,
            UsedAmount = 250m,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            CreatedBy = 1
        });
        db.SaveChanges();
    }
}
