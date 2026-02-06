using System.Net;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using CRMBranchOffice = EBOS.CRM.Domain.Entities.CRM.BranchOffice;
using CRMCorporateCustomer = EBOS.CRM.Domain.Entities.CRM.CorporateCustomer;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.CRM.BranchOffice;

public class BranchOfficeTenantIsolationTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly string _version;

    public BranchOfficeTenantIsolationTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _version = ApiVersionHelper.GetLatestVersion(factory, "BranchOffice");
    }

    [Fact]
    public async Task GetAll_Filters_By_Tenant_Header()
    {
        var name1 = $"Branch-1-{Guid.NewGuid():N}";
        var name2 = $"Branch-2-{Guid.NewGuid():N}";
        var erasedName = SeedBranchOffices(name1, name2, out var _);

        var clientTenant1 = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var response1 = await clientTenant1.GetAsync($"/api/v{_version}/BranchOffice");
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant1 = await response1.Content.ReadItemsAsync<BranchOfficeResponse>();
        itemsTenant1.Should().Contain(i => i.Name == name1 && i.Active);
        itemsTenant1.Should().NotContain(i => i.Name == name2);
        itemsTenant1.Should().NotContain(i => i.Name == erasedName);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(_factory, 2);
        var response2 = await clientTenant2.GetAsync($"/api/v{_version}/BranchOffice");
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant2 = await response2.Content.ReadItemsAsync<BranchOfficeResponse>();
        itemsTenant2.Should().Contain(i => i.Name == name2);
        itemsTenant2.Should().NotContain(i => i.Name == name1);
    }

    [Fact]
    public async Task GetById_Returns_404_When_Requesting_Other_Tenant_Data()
    {
        var name1 = $"Branch-1-{Guid.NewGuid():N}";
        var name2 = $"Branch-2-{Guid.NewGuid():N}";
        SeedBranchOffices(name1, name2, out var ids);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(_factory, 2);
        var response = await clientTenant2.GetAsync($"/api/v{_version}/BranchOffice/{ids.Tenant1Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private string SeedBranchOffices(string name1, string name2, out (long Tenant1Id, long Tenant2Id) ids)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var statusId = db.Statuses.Select(s => s.Id).First();
        var erasedName = $"Branch-Erased-{Guid.NewGuid():N}";

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
            Name = name1,
            PhoneNumber = "111",
            CorporateCustomerId = corporate1.Id
        };

        var branch2 = new CRMBranchOffice
        {
            TenantId = 2,
            Name = name2,
            PhoneNumber = "222",
            CorporateCustomerId = corporate2.Id
        };

        var branchErased = new CRMBranchOffice
        {
            TenantId = 1,
            Name = erasedName,
            PhoneNumber = "333",
            CorporateCustomerId = corporate1.Id,
            Erased = true
        };

        db.BranchOffices.AddRange(branch1, branch2, branchErased);
        db.SaveChanges();

        ids = (branch1.Id, branch2.Id);
        return erasedName;
    }

}
