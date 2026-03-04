using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Contracts.Requests.CRM.IndividualCustomer;
using EBOS.CRM.Contracts.Requests.CRM.CustomerMerge;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.CustomerMerge;

public class CustomerMergeEndpointTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _tenant1 = HttpClientFactory.CreateClientWithTenant(factory, 1);
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CustomerMerge");
    private readonly string _customerVersion = ApiVersionHelper.GetLatestVersion(factory, "Customer");
    private readonly string _individualVersion = ApiVersionHelper.GetLatestVersion(factory, "IndividualCustomer");
    private readonly string _statusVersion = ApiVersionHelper.GetLatestVersion(factory, "Status");
    private readonly string _identificationTypeVersion = ApiVersionHelper.GetLatestVersion(factory, "IdentificationType");

    [Fact]
    public async Task HappyPath_Duplicates_Merge_And_History_Work()
    {
        var duplicateEmail = $"dup-{Guid.NewGuid():N}@example.com";
        var winner = await CreateIndividualCustomerAsync(1, duplicateEmail);
        var merged = await CreateIndividualCustomerAsync(1, duplicateEmail);

        var duplicates = await _tenant1.GetAsync(
            $"/api/v{_version}/CustomerMerge/duplicates?tenantId=1&email={Uri.EscapeDataString(duplicateEmail)}");
        duplicates.StatusCode.Should().Be(HttpStatusCode.OK);

        var merge = await _tenant1.PostAsJsonAsync(
            $"/api/v{_version}/CustomerMerge/merge",
            new MergeCustomersRequest(1, winner.Id, [merged.Id], "it"));
        merge.StatusCode.Should().Be(HttpStatusCode.OK);

        var byWinner = await _tenant1.GetAsync($"/api/v{_version}/CustomerMerge/history/by-winner/{winner.Id}?tenantId=1");
        byWinner.StatusCode.Should().Be(HttpStatusCode.OK);

        var byMerged = await _tenant1.GetAsync($"/api/v{_version}/CustomerMerge/history/by-merged/{merged.Id}?tenantId=1");
        byMerged.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Merge_WithTenantMismatch_ReturnsBadRequest()
    {
        var duplicateEmail = $"dup-mis-{Guid.NewGuid():N}@example.com";
        var winner = await CreateIndividualCustomerAsync(1, duplicateEmail);
        var merged = await CreateIndividualCustomerAsync(1, duplicateEmail);

        var response = await _tenant1.PostAsJsonAsync(
            $"/api/v{_version}/CustomerMerge/merge",
            new MergeCustomersRequest(2, winner.Id, [merged.Id], "tenant mismatch"));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Duplicates_WithInvalidTenant_ReturnsBadRequest()
    {
        var response = await _tenant1.GetAsync($"/api/v{_version}/CustomerMerge/duplicates?tenantId=0&email=a@b.com");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<CustomerResponse> CreateCustomerAsync(long tenantId, string email)
    {
        var statusId = await LookupHelper.GetStatusIdAsync(_tenant1, _statusVersion);
        var response = await _tenant1.PostAsJsonAsync(
            $"/api/v{_customerVersion}/Customer",
            new AddCustomerRequest(
                tenantId,
                $"C-{Guid.NewGuid():N}"[..12],
                email,
                "34600000004",
                statusId));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        created.Should().NotBeNull();
        return created!;
    }

    private async Task<IndividualCustomerResponse> CreateIndividualCustomerAsync(long tenantId, string email)
    {
        var statusId = await LookupHelper.GetStatusIdAsync(_tenant1, _statusVersion);
        var identificationTypeId = await LookupHelper.GetIdentificationTypeIdAsync(_tenant1, _identificationTypeVersion);

        var response = await _tenant1.PostAsJsonAsync(
            $"/api/v{_individualVersion}/IndividualCustomer",
            new AddIndividualCustomerRequest(
                tenantId,
                Code: $"IC-{Guid.NewGuid():N}"[..10],
                Email: email,
                Phone: "34600000005",
                StatusId: statusId,
                FirstName: "Jane",
                LastName: "Doe",
                BirthDate: new DateTime(1990, 1, 1),
                IdentificationNumber: $"{Random.Shared.Next(100000000, 999999999)}",
                IdentificationTypeId: identificationTypeId));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await response.Content.ReadFromJsonAsync<IndividualCustomerResponse>();
        created.Should().NotBeNull();
        return created!;
    }
}
