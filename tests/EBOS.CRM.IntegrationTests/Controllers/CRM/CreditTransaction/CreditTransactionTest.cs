using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.CreditAccount;
using EBOS.CRM.Contracts.Requests.CRM.CreditTransaction;
using EBOS.CRM.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.CreditTransaction;

public class CreditTransactionTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CreditTransaction");
    private readonly string _creditAccountVersion = ApiVersionHelper.GetLatestVersion(factory, "CreditAccount");
    private readonly string _customerVersion = ApiVersionHelper.GetLatestVersion(factory, "Customer");
    private readonly string _statusVersion = ApiVersionHelper.GetLatestVersion(factory, "Status");

    [Fact]
    public async Task GetAll_Returns_ListOfItems()
    {
        var response = await _client.GetAsync($"/api/v{_version}/CreditTransaction");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadItemsAsync<CreditTransactionResponse>();
        items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_Returns_404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/api/v{_version}/CreditTransaction/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Add_Update_Delete_Works_For_CreditTransaction()
    {
        var creditAccountId = await CreateCreditAccountAsync();

        var addRequest = new AddCreditTransactionRequest(
            TenantId: 1,
            Date: DateTime.UtcNow.Date,
            Amount: 200m,
            Type: "Consumption",
            ExternalReference: "INV-1001",
            Comments: "Monthly service charge",
            CreditAccountId: creditAccountId);

        var addResponse = await _client.PostAsJsonAsync($"/api/v{_version}/CreditTransaction", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await addResponse.Content.ReadFromJsonAsync<CreditTransactionResponse>();
        created.Should().NotBeNull();

        var updateRequest = new UpdateCreditTransactionRequest(
            TenantId: 1,
            Date: DateTime.UtcNow.Date,
            Amount: 250m,
            Type: "Adjustment",
            ExternalReference: "INV-1002",
            Comments: "Adjustment",
            CreditAccountId: creditAccountId);

        var updateResponse =
            await _client.PutAsJsonAsync($"/api/v{_version}/CreditTransaction/{created.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var deleteResponse = await _client.DeleteAsync($"/api/v{_version}/CreditTransaction/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<long> CreateCreditAccountAsync()
    {
        var customerId = await CreateCustomerAsync();

        var request = new AddCreditAccountRequest(
            TenantId: 1,
            MaxAmount: 10000m,
            UsedAmount: 100m,
            CustomerId: customerId);

        var response = await _client.PostAsJsonAsync($"/api/v{_creditAccountVersion}/CreditAccount", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await response.Content.ReadFromJsonAsync<CreditAccountResponse>();
        created.Should().NotBeNull();
        return created.Id;
    }

    private async Task<long> CreateCustomerAsync()
    {
        var statusId = await LookupHelper.GetStatusIdAsync(_client, _statusVersion);

        var request = new AddCustomerRequest(
            TenantId: 1,
            Code: $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            Email: $"user{Guid.NewGuid():N}@example.com",
            Phone: "34600123456",
            StatusId: statusId);

        var response = await _client.PostAsJsonAsync($"/api/v{_customerVersion}/Customer", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        created.Should().NotBeNull();
        return created.Id;
    }
}






