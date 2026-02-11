using System.Net;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using CRMAddress = global::EBOS.CRM.Domain.Entities.CRM.Address;
using CRMBranchOffice = global::EBOS.CRM.Domain.Entities.CRM.BranchOffice;
using CRMBranchOfficeAddress = global::EBOS.CRM.Domain.Entities.CRM.BranchOfficeAddress;
using CRMCorporateCustomer = global::EBOS.CRM.Domain.Entities.CRM.CorporateCustomer;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.BranchOfficeAddress;

public class BranchOfficeAddressTenantIsolationTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "BranchOfficeAddress");

    [Fact]
    public async Task GetAll_Filters_By_Tenant_Header()
    {
        SeedBranchOfficeAddresses(out var data);

        var clientTenant1 = HttpClientFactory.CreateClientWithTenant(factory, 1);
        var response1 = await clientTenant1.GetAsync($"/api/v{_version}/BranchOfficeAddress");
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant1 = await response1.Content.ReadItemsAsync<BranchOfficeAddressResponse>();
        itemsTenant1.Should().Contain(i => i.AddressId == data.Address1Id && i.Active);
        itemsTenant1.Should().NotContain(i => i.AddressId == data.Address2Id);
        itemsTenant1.Should().NotContain(i => i.AddressId == data.ErasedAddressId);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(factory, 2);
        var response2 = await clientTenant2.GetAsync($"/api/v{_version}/BranchOfficeAddress");
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant2 = await response2.Content.ReadItemsAsync<BranchOfficeAddressResponse>();
        itemsTenant2.Should().Contain(i => i.AddressId == data.Address2Id);
        itemsTenant2.Should().NotContain(i => i.AddressId == data.Address1Id);
    }

    [Fact]
    public async Task GetById_Returns_404_When_Requesting_Other_Tenant_Data()
    {
        SeedBranchOfficeAddresses(out var data);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(factory, 2);
        var response = await clientTenant2.GetAsync($"/api/v{_version}/BranchOfficeAddress/{data.BranchOfficeAddress1Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private void SeedBranchOfficeAddresses(out (long BranchOfficeAddress1Id, long BranchOfficeAddress2Id, long Address1Id, long Address2Id, long ErasedAddressId) data)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var statusId = db.Statuses.Select(s => s.Id).First();
        var countryId = db.Countries.Select(c => c.Id).First();
        var addressTypeId = db.AddressTypes.Select(a => a.Id).First();

        var corporate1 = new CRMCorporateCustomer
        {
            TenantId = 1,
            Code = $"C1-{Guid.NewGuid():N}",
            Email = $"{Guid.NewGuid():N}@site.com",
            Phone = "100",
            StatusId = statusId,
            LegalName = "LegalName1",
            TaxIdentification = "TAX-1"
        };

        var corporate2 = new CRMCorporateCustomer
        {
            TenantId = 2,
            Code = $"C2-{Guid.NewGuid():N}",
            Email = $"{Guid.NewGuid():N}@site.com",
            Phone = "200",
            StatusId = statusId,
            LegalName = "LegalName2",
            TaxIdentification = "TAX-2"
        };

        db.CorporateCustomers.AddRange(corporate1, corporate2);
        db.SaveChanges();

        var branch1 = new CRMBranchOffice
        {
            TenantId = 1,
            Name = $"Branch-1-{Guid.NewGuid():N}",
            PhoneNumber = "111",
            CorporateCustomerId = corporate1.Id
        };

        var branch2 = new CRMBranchOffice
        {
            TenantId = 2,
            Name = $"Branch-2-{Guid.NewGuid():N}",
            PhoneNumber = "222",
            CorporateCustomerId = corporate2.Id
        };

        db.BranchOffices.AddRange(branch1, branch2);
        db.SaveChanges();

        var address1 = new CRMAddress
        {
            TenantId = 1,
            Street = "Street 1",
            ExternalNumber = "10",
            City = "City1",
            StateOrProvince = "State1",
            PostalCode = "11111",
            CountryId = countryId,
            AddressTypeId = addressTypeId
        };

        var address2 = new CRMAddress
        {
            TenantId = 2,
            Street = "Street 2",
            ExternalNumber = "20",
            City = "City2",
            StateOrProvince = "State2",
            PostalCode = "22222",
            CountryId = countryId,
            AddressTypeId = addressTypeId
        };

        var addressErased = new CRMAddress
        {
            TenantId = 1,
            Street = "Street 3",
            ExternalNumber = "30",
            City = "City3",
            StateOrProvince = "State3",
            PostalCode = "33333",
            CountryId = countryId,
            AddressTypeId = addressTypeId
        };

        db.Addresses.AddRange(address1, address2, addressErased);
        db.SaveChanges();

        var link1 = new CRMBranchOfficeAddress
        {
            TenantId = 1,
            BranchOfficeId = branch1.Id,
            AddressId = address1.Id,
            IsPrimary = true,
            ValidFrom = DateTime.UtcNow,
            IsCurrent = true
        };

        var link2 = new CRMBranchOfficeAddress
        {
            TenantId = 2,
            BranchOfficeId = branch2.Id,
            AddressId = address2.Id,
            IsPrimary = true,
            ValidFrom = DateTime.UtcNow,
            IsCurrent = true
        };

        var linkErased = new CRMBranchOfficeAddress
        {
            TenantId = 1,
            BranchOfficeId = branch1.Id,
            AddressId = addressErased.Id,
            IsPrimary = false,
            ValidFrom = DateTime.UtcNow,
            IsCurrent = true,
            Erased = true
        };

        db.Set<CRMBranchOfficeAddress>().AddRange(link1, link2, linkErased);
        db.SaveChanges();

        data = (link1.Id, link2.Id, address1.Id, address2.Id, addressErased.Id);
    }

}

