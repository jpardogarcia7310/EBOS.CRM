using System.Net;
using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.AccountContact;
using EBOS.CRM.Contracts.Requests.CRM.AccountContactRole;
using EBOS.CRM.Contracts.Requests.CRM.AccountHierarchy;
using EBOS.CRM.Contracts.Requests.CRM.CorporateCustomer;
using EBOS.CRM.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Contracts.Requests.CRM.CustomerConsent;
using EBOS.CRM.Contracts.Requests.CRM.CustomerMerge;
using EBOS.CRM.Contracts.Requests.CRM.CustomerPreference;
using EBOS.CRM.Contracts.Requests.CRM.IndividualCustomer;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Customer360;

public class Customer360E2EExtendedTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _tenant1 = HttpClientFactory.CreateClientWithTenant(factory, 1);
    private readonly HttpClient _tenant2 = HttpClientFactory.CreateClientWithTenant(factory, 2);

    private readonly string _customerVersion = ApiVersionHelper.GetLatestVersion(factory, "Customer");
    private readonly string _corporateVersion = ApiVersionHelper.GetLatestVersion(factory, "CorporateCustomer");
    private readonly string _individualVersion = ApiVersionHelper.GetLatestVersion(factory, "IndividualCustomer");
    private readonly string _accountContactVersion = ApiVersionHelper.GetLatestVersion(factory, "AccountContact");
    private readonly string _accountContactRoleVersion = ApiVersionHelper.GetLatestVersion(factory, "AccountContactRole");
    private readonly string _accountHierarchyVersion = ApiVersionHelper.GetLatestVersion(factory, "AccountHierarchy");
    private readonly string _customerPreferenceVersion = ApiVersionHelper.GetLatestVersion(factory, "CustomerPreference");
    private readonly string _customerConsentVersion = ApiVersionHelper.GetLatestVersion(factory, "CustomerConsent");
    private readonly string _customerMergeVersion = ApiVersionHelper.GetLatestVersion(factory, "CustomerMerge");
    private readonly string _statusVersion = ApiVersionHelper.GetLatestVersion(factory, "Status");
    private readonly string _identificationTypeVersion = ApiVersionHelper.GetLatestVersion(factory, "IdentificationType");

    [Fact]
    public async Task AccountContact_E2E_FullFlow_Works()
    {
        var statusId = await LookupHelper.GetStatusIdAsync(_tenant1, _statusVersion);
        var idTypeId = await LookupHelper.GetIdentificationTypeIdAsync(_tenant1, _identificationTypeVersion);
        var corp = await CreateCorporateAsync(_tenant1, statusId, tenantId: 1);
        var person = await CreateIndividualAsync(_tenant1, statusId, idTypeId, tenantId: 1);

        var addRequest = new AddAccountContactRequest(1, corp.Id, person.Id, false, DateTime.UtcNow.AddDays(-1), null);
        var addResponse = await _tenant1.PostAsJsonAsync($"/api/v{_accountContactVersion}/AccountContact", addRequest);
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await addResponse.Content.ReadFromJsonAsync<AccountContactResponse>();
        created.Should().NotBeNull();

        var getById = await _tenant1.GetAsync($"/api/v{_accountContactVersion}/AccountContact/{created!.Id}");
        getById.StatusCode.Should().Be(HttpStatusCode.OK);

        var updateResponse = await _tenant1.PutAsJsonAsync(
            $"/api/v{_accountContactVersion}/AccountContact/{created.Id}",
            new UpdateAccountContactRequest(1, corp.Id, person.Id, false, DateTime.UtcNow.AddDays(-2), null));
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var setPrimaryResponse = await _tenant1.PatchAsJsonAsync(
            $"/api/v{_accountContactVersion}/AccountContact/{created.Id}/primary",
            new SetPrimaryAccountContactRequest(1, true));
        setPrimaryResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var foreignTenantGet = await _tenant2.GetAsync($"/api/v{_accountContactVersion}/AccountContact/{created.Id}");
        foreignTenantGet.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var badTenantAdd = await _tenant1.PostAsJsonAsync(
            $"/api/v{_accountContactVersion}/AccountContact",
            new AddAccountContactRequest(2, corp.Id, person.Id, false, DateTime.UtcNow, null));
        badTenantAdd.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var notFoundDelete = await _tenant1.DeleteAsync($"/api/v{_accountContactVersion}/AccountContact/999999?tenantId=1");
        notFoundDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var concurrentReads = Enumerable.Range(0, 12)
            .Select(_ => _tenant1.GetAsync($"/api/v{_accountContactVersion}/AccountContact/{created.Id}"));
        var concurrentResults = await Task.WhenAll(concurrentReads);
        concurrentResults.Should().OnlyContain(r => (int)r.StatusCode < 500);

        var deleteResponse = await _tenant1.DeleteAsync($"/api/v{_accountContactVersion}/AccountContact/{created.Id}?tenantId=1");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AccountContactRole_E2E_FullFlow_Works()
    {
        var accountContact = await CreateAccountContactAsync(_tenant1, tenantId: 1);

        var addResponse = await _tenant1.PostAsJsonAsync(
            $"/api/v{_accountContactRoleVersion}/AccountContactRole",
            new AddAccountContactRoleRequest(1, accountContact.Id, "LEGAL_REP", false, DateTime.UtcNow.AddDays(-1), null));
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await addResponse.Content.ReadFromJsonAsync<AccountContactRoleResponse>();
        created.Should().NotBeNull();

        var getById = await _tenant1.GetAsync($"/api/v{_accountContactRoleVersion}/AccountContactRole/{created!.Id}");
        getById.StatusCode.Should().Be(HttpStatusCode.OK);

        var updateResponse = await _tenant1.PutAsJsonAsync(
            $"/api/v{_accountContactRoleVersion}/AccountContactRole/{created.Id}",
            new UpdateAccountContactRoleRequest(1, accountContact.Id, "BILLING_OWNER", true, DateTime.UtcNow.AddDays(-2), null));
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var foreignTenantGet = await _tenant2.GetAsync($"/api/v{_accountContactRoleVersion}/AccountContactRole/{created.Id}");
        foreignTenantGet.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var badTenantAdd = await _tenant1.PostAsJsonAsync(
            $"/api/v{_accountContactRoleVersion}/AccountContactRole",
            new AddAccountContactRoleRequest(2, accountContact.Id, "LEGAL_REP", false, DateTime.UtcNow, null));
        badTenantAdd.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var notFoundDelete = await _tenant1.DeleteAsync($"/api/v{_accountContactRoleVersion}/AccountContactRole/999999?tenantId=1");
        notFoundDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var concurrentReads = Enumerable.Range(0, 12)
            .Select(_ => _tenant1.GetAsync($"/api/v{_accountContactRoleVersion}/AccountContactRole/{created.Id}"));
        var concurrentResults = await Task.WhenAll(concurrentReads);
        concurrentResults.Should().OnlyContain(r => (int)r.StatusCode < 500);

        var deleteResponse = await _tenant1.DeleteAsync($"/api/v{_accountContactRoleVersion}/AccountContactRole/{created.Id}?tenantId=1");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AccountHierarchy_E2E_FullFlow_Works()
    {
        var statusId = await LookupHelper.GetStatusIdAsync(_tenant1, _statusVersion);
        var parent = await CreateCorporateAsync(_tenant1, statusId, tenantId: 1);
        var child = await CreateCorporateAsync(_tenant1, statusId, tenantId: 1);

        var addResponse = await _tenant1.PostAsJsonAsync(
            $"/api/v{_accountHierarchyVersion}/AccountHierarchy",
            new AddAccountHierarchyRequest(1, parent.Id, child.Id, "GROUP_PARENT", DateTime.UtcNow.AddDays(-10)));
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await addResponse.Content.ReadFromJsonAsync<AccountHierarchyResponse>();
        created.Should().NotBeNull();

        var getById = await _tenant1.GetAsync($"/api/v{_accountHierarchyVersion}/AccountHierarchy/{created!.Id}");
        getById.StatusCode.Should().Be(HttpStatusCode.OK);

        var endResponse = await _tenant1.PatchAsJsonAsync(
            $"/api/v{_accountHierarchyVersion}/AccountHierarchy/{created.Id}/end",
            new EndAccountHierarchyRequest(1, DateTime.UtcNow));
        endResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var foreignTenantGet = await _tenant2.GetAsync($"/api/v{_accountHierarchyVersion}/AccountHierarchy/{created.Id}");
        foreignTenantGet.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var badTenantAdd = await _tenant1.PostAsJsonAsync(
            $"/api/v{_accountHierarchyVersion}/AccountHierarchy",
            new AddAccountHierarchyRequest(2, parent.Id, child.Id, "GROUP_PARENT", DateTime.UtcNow));
        badTenantAdd.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var notFoundEnd = await _tenant1.PatchAsJsonAsync(
            $"/api/v{_accountHierarchyVersion}/AccountHierarchy/999999/end",
            new EndAccountHierarchyRequest(1, DateTime.UtcNow));
        notFoundEnd.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var concurrentReads = Enumerable.Range(0, 12)
            .Select(_ => _tenant1.GetAsync($"/api/v{_accountHierarchyVersion}/AccountHierarchy/{created.Id}"));
        var concurrentResults = await Task.WhenAll(concurrentReads);
        concurrentResults.Should().OnlyContain(r => (int)r.StatusCode < 500);
    }

    [Fact]
    public async Task CustomerPreference_E2E_FullFlow_Works()
    {
        var customer = await CreateCustomerAsync(_tenant1, tenantId: 1);
        var channelId = GetActiveChannelTypeId();

        var upsertResponse = await _tenant1.PutAsJsonAsync(
            $"/api/v{_customerPreferenceVersion}/CustomerPreference",
            new UpsertCustomerPreferenceRequest(1, customer.Id, ChannelId: channelId, Preferred: true));
        upsertResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var upserted = await upsertResponse.Content.ReadFromJsonAsync<CustomerPreferenceResponse>();
        upserted.Should().NotBeNull();

        var getByCustomer = await _tenant1.GetAsync(
            $"/api/v{_customerPreferenceVersion}/CustomerPreference/by-customer/{customer.Id}?tenantId=1");
        getByCustomer.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await getByCustomer.Content.ReadItemsAsync<CustomerPreferenceResponse>();
        items.Should().Contain(x => x.CustomerId == customer.Id && x.ChannelId == channelId);

        var foreignTenantGet = await _tenant2.GetAsync(
            $"/api/v{_customerPreferenceVersion}/CustomerPreference/by-customer/{customer.Id}?tenantId=2");
        foreignTenantGet.StatusCode.Should().Be(HttpStatusCode.OK);
        var foreignItems = await foreignTenantGet.Content.ReadItemsAsync<CustomerPreferenceResponse>();
        foreignItems.Should().NotContain(x => x.CustomerId == customer.Id);

        var badTenantUpsert = await _tenant1.PutAsJsonAsync(
            $"/api/v{_customerPreferenceVersion}/CustomerPreference",
            new UpsertCustomerPreferenceRequest(2, customer.Id, ChannelId: channelId, Preferred: false));
        badTenantUpsert.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var concurrentReads = Enumerable.Range(0, 12)
            .Select(_ => _tenant1.GetAsync(
                $"/api/v{_customerPreferenceVersion}/CustomerPreference/by-customer/{customer.Id}?tenantId=1"));
        var concurrentResults = await Task.WhenAll(concurrentReads);
        concurrentResults.Should().OnlyContain(r => (int)r.StatusCode < 500);
    }

    [Fact]
    public async Task CustomerConsent_E2E_FullFlow_Works()
    {
        var customer = await CreateCustomerAsync(_tenant1, tenantId: 1);

        var addResponse = await _tenant1.PostAsJsonAsync(
            $"/api/v{_customerConsentVersion}/CustomerConsent",
            new AddCustomerConsentRequest(1, customer.Id, "MARKETING_EMAIL", true, DateTime.UtcNow.AddMinutes(-1), "e2e", null));
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await addResponse.Content.ReadFromJsonAsync<CustomerConsentResponse>();
        created.Should().NotBeNull();

        var getByCustomer = await _tenant1.GetAsync(
            $"/api/v{_customerConsentVersion}/CustomerConsent/by-customer/{customer.Id}?tenantId=1");
        getByCustomer.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await getByCustomer.Content.ReadItemsAsync<CustomerConsentResponse>();
        items.Should().Contain(x => x.Id == created!.Id && x.Granted);

        var revokeResponse = await _tenant1.PatchAsJsonAsync(
            $"/api/v{_customerConsentVersion}/CustomerConsent/{created!.Id}/revoke",
            new RevokeCustomerConsentRequest(1, DateTime.UtcNow));
        revokeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var foreignTenantGet = await _tenant2.GetAsync(
            $"/api/v{_customerConsentVersion}/CustomerConsent/by-customer/{customer.Id}?tenantId=2");
        foreignTenantGet.StatusCode.Should().Be(HttpStatusCode.OK);
        var foreignItems = await foreignTenantGet.Content.ReadItemsAsync<CustomerConsentResponse>();
        foreignItems.Should().NotContain(x => x.CustomerId == customer.Id);

        var badTenantAdd = await _tenant1.PostAsJsonAsync(
            $"/api/v{_customerConsentVersion}/CustomerConsent",
            new AddCustomerConsentRequest(2, customer.Id, "MARKETING_EMAIL", true, DateTime.UtcNow, "e2e", null));
        badTenantAdd.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var notFoundRevoke = await _tenant1.PatchAsJsonAsync(
            $"/api/v{_customerConsentVersion}/CustomerConsent/999999/revoke",
            new RevokeCustomerConsentRequest(1, DateTime.UtcNow));
        notFoundRevoke.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var concurrentReads = Enumerable.Range(0, 12)
            .Select(_ => _tenant1.GetAsync(
                $"/api/v{_customerConsentVersion}/CustomerConsent/by-customer/{customer.Id}?tenantId=1"));
        var concurrentResults = await Task.WhenAll(concurrentReads);
        concurrentResults.Should().OnlyContain(r => (int)r.StatusCode < 500);
    }

    [Fact]
    public async Task CustomerMerge_E2E_FullFlow_Works()
    {
        var statusId = await LookupHelper.GetStatusIdAsync(_tenant1, _statusVersion);
        var idTypeId = await LookupHelper.GetIdentificationTypeIdAsync(_tenant1, _identificationTypeVersion);
        var duplicateEmail = $"dup-{Guid.NewGuid():N}@example.com";
        var winner = await CreateIndividualAsync(_tenant1, statusId, idTypeId, tenantId: 1, forcedEmail: duplicateEmail);
        var duplicate = await CreateIndividualAsync(_tenant1, statusId, idTypeId, tenantId: 1, forcedEmail: duplicateEmail);

        var findResponse = await _tenant1.GetAsync(
            $"/api/v{_customerMergeVersion}/CustomerMerge/duplicates?tenantId=1&email={Uri.EscapeDataString(winner.Email)}");
        findResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var duplicates = await findResponse.Content.ReadItemsAsync<CustomerDuplicateCandidateResponse>();
        duplicates.Should().NotBeEmpty();

        var mergeResponse = await _tenant1.PostAsJsonAsync(
            $"/api/v{_customerMergeVersion}/CustomerMerge/merge",
            new MergeCustomersRequest(1, winner.Id, [duplicate.Id], "E2E merge validation"));
        mergeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var mergeResult = await mergeResponse.Content.ReadFromJsonAsync<CustomerMergeResultResponse>();
        mergeResult.Should().NotBeNull();
        mergeResult!.WinnerCustomerId.Should().Be(winner.Id);
        mergeResult.MergedCustomerIds.Should().Contain(duplicate.Id);

        var foreignTenantFind = await _tenant2.GetAsync(
            $"/api/v{_customerMergeVersion}/CustomerMerge/duplicates?tenantId=2&email={Uri.EscapeDataString(winner.Email)}");
        foreignTenantFind.StatusCode.Should().Be(HttpStatusCode.OK);
        var foreignDuplicates = await foreignTenantFind.Content.ReadItemsAsync<CustomerDuplicateCandidateResponse>();
        foreignDuplicates.Should().BeEmpty();

        var badTenantMerge = await _tenant1.PostAsJsonAsync(
            $"/api/v{_customerMergeVersion}/CustomerMerge/merge",
            new MergeCustomersRequest(2, winner.Id, [duplicate.Id], "should fail by tenant mismatch"));
        badTenantMerge.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var concurrentReads = Enumerable.Range(0, 12)
            .Select(_ => _tenant1.GetAsync(
                $"/api/v{_customerMergeVersion}/CustomerMerge/duplicates?tenantId=1&email={Uri.EscapeDataString(winner.Email)}"));
        var concurrentResults = await Task.WhenAll(concurrentReads);
        concurrentResults.Should().OnlyContain(r => (int)r.StatusCode < 500);
    }

    private async Task<CustomerResponse> CreateCustomerAsync(HttpClient client, long tenantId, string? forcedEmail = null)
    {
        var statusId = await LookupHelper.GetStatusIdAsync(client, _statusVersion);
        var response = await client.PostAsJsonAsync(
            $"/api/v{_customerVersion}/Customer",
            new AddCustomerRequest(
                tenantId,
                Code: $"C-{Guid.NewGuid():N}"[..12],
                Email: forcedEmail ?? $"c-{Guid.NewGuid():N}@example.com",
                Phone: "34600000000",
                StatusId: statusId));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        created.Should().NotBeNull();
        return created!;
    }

    private async Task<CorporateCustomerResponse> CreateCorporateAsync(HttpClient client, long statusId, long tenantId)
    {
        var response = await client.PostAsJsonAsync(
            $"/api/v{_corporateVersion}/CorporateCustomer",
            new AddCorporateCustomerRequest(
                tenantId,
                Code: $"CC-{Guid.NewGuid():N}"[..10],
                Email: $"corp-{Guid.NewGuid():N}@example.com",
                Phone: "34600000001",
                StatusId: statusId,
                LegalName: $"Corp {Guid.NewGuid():N}"[..20],
                TaxIdentification: $"TAX{Random.Shared.Next(100000, 999999)}"));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await response.Content.ReadFromJsonAsync<CorporateCustomerResponse>();
        created.Should().NotBeNull();
        return created!;
    }

    private async Task<IndividualCustomerResponse> CreateIndividualAsync(HttpClient client, long statusId, long idTypeId, long tenantId)
    {
        var response = await client.PostAsJsonAsync(
            $"/api/v{_individualVersion}/IndividualCustomer",
            new AddIndividualCustomerRequest(
                tenantId,
                Code: $"IC-{Guid.NewGuid():N}"[..10],
                Email: $"ind-{Guid.NewGuid():N}@example.com",
                Phone: "34600000002",
                StatusId: statusId,
                FirstName: "Jane",
                LastName: "Doe",
                BirthDate: new DateTime(1990, 1, 1),
                IdentificationNumber: $"{Random.Shared.Next(100000000, 999999999)}",
                IdentificationTypeId: idTypeId));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await response.Content.ReadFromJsonAsync<IndividualCustomerResponse>();
        created.Should().NotBeNull();
        return created!;
    }

    private async Task<IndividualCustomerResponse> CreateIndividualAsync(HttpClient client, long statusId, long idTypeId, long tenantId, string forcedEmail)
    {
        var response = await client.PostAsJsonAsync(
            $"/api/v{_individualVersion}/IndividualCustomer",
            new AddIndividualCustomerRequest(
                tenantId,
                Code: $"IC-{Guid.NewGuid():N}"[..10],
                Email: forcedEmail,
                Phone: "34600000002",
                StatusId: statusId,
                FirstName: "Jane",
                LastName: "Doe",
                BirthDate: new DateTime(1990, 1, 1),
                IdentificationNumber: $"{Random.Shared.Next(100000000, 999999999)}",
                IdentificationTypeId: idTypeId));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await response.Content.ReadFromJsonAsync<IndividualCustomerResponse>();
        created.Should().NotBeNull();
        return created!;
    }

    private async Task<AccountContactResponse> CreateAccountContactAsync(HttpClient client, long tenantId)
    {
        var statusId = await LookupHelper.GetStatusIdAsync(client, _statusVersion);
        var idTypeId = await LookupHelper.GetIdentificationTypeIdAsync(client, _identificationTypeVersion);
        var corp = await CreateCorporateAsync(client, statusId, tenantId);
        var person = await CreateIndividualAsync(client, statusId, idTypeId, tenantId);

        var response = await client.PostAsJsonAsync(
            $"/api/v{_accountContactVersion}/AccountContact",
            new AddAccountContactRequest(tenantId, corp.Id, person.Id, false, DateTime.UtcNow.AddDays(-1), null));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await response.Content.ReadFromJsonAsync<AccountContactResponse>();
        created.Should().NotBeNull();
        return created!;
    }

    private long GetActiveChannelTypeId()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var existingId = db.ChannelTypes.Where(x => x.IsActive).Select(x => (long?)x.Id).FirstOrDefault();
        if (existingId.HasValue)
        {
            return existingId.Value;
        }

        var channel = new ChannelType
        {
            Descripcion = $"E2E-{Guid.NewGuid():N}"[..16],
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
