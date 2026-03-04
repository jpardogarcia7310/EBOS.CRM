using System.Net;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using CRMIndividualCustomer = EBOS.CRM.Domain.Entities.CRM.IndividualCustomer;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.IndividualCustomer;

public class IndividualCustomerTenantIsolationTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "IndividualCustomer");

    [Fact]
    public async Task GetAll_Filters_By_Tenant_Header()
    {
        var firstName1 = $"First-{Guid.NewGuid():N}";
        var firstName2 = $"First-{Guid.NewGuid():N}";
        var erasedFirstName = SeedIndividualCustomers(firstName1, firstName2, out var _);

        var clientTenant1 = HttpClientFactory.CreateClientWithTenant(factory, 1);
        var response1 = await clientTenant1.GetAsync($"/api/v{_version}/IndividualCustomer");
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant1 = await response1.Content.ReadItemsAsync<IndividualCustomerResponse>();
        itemsTenant1.Should().Contain(i => i.FirstName == firstName1 && i.Active);
        itemsTenant1.Should().NotContain(i => i.FirstName == firstName2);
        itemsTenant1.Should().NotContain(i => i.FirstName == erasedFirstName);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(factory, 2);
        var response2 = await clientTenant2.GetAsync($"/api/v{_version}/IndividualCustomer");
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant2 = await response2.Content.ReadItemsAsync<IndividualCustomerResponse>();
        itemsTenant2.Should().Contain(i => i.FirstName == firstName2);
        itemsTenant2.Should().NotContain(i => i.FirstName == firstName1);
    }

    [Fact]
    public async Task GetById_Returns_404_When_Requesting_Other_Tenant_Data()
    {
        var firstName1 = $"First-{Guid.NewGuid():N}";
        var firstName2 = $"First-{Guid.NewGuid():N}";
        SeedIndividualCustomers(firstName1, firstName2, out var ids);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(factory, 2);
        var response = await clientTenant2.GetAsync($"/api/v{_version}/IndividualCustomer/{ids.Tenant1Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private string SeedIndividualCustomers(string firstName1, string firstName2, out (long Tenant1Id, long Tenant2Id) ids)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var statusId = db.Statuses.Select(s => s.Id).First();
        var identificationTypeId = db.IdentificationTypes.Select(i => i.Id).First();
        var erasedFirstName = $"First-Erased-{Guid.NewGuid():N}";

        var customer1 = new CRMIndividualCustomer
        {
            TenantId = 1,
            Code = $"C1-{Guid.NewGuid():N}",
            Email = $"{Guid.NewGuid():N}@site.com",
            Phone = "100",
            StatusId = statusId,
            FirstName = firstName1,
            LastName = "Last1",
            BirthDate = new DateTime(1990, 1, 1),
            IdentificationNumber = "ID-1",
            IdentificationTypeId = identificationTypeId
        };

        var customer2 = new CRMIndividualCustomer
        {
            TenantId = 2,
            Code = $"C2-{Guid.NewGuid():N}",
            Email = $"{Guid.NewGuid():N}@site.com",
            Phone = "200",
            StatusId = statusId,
            FirstName = firstName2,
            LastName = "Last2",
            BirthDate = new DateTime(1991, 2, 2),
            IdentificationNumber = "ID-2",
            IdentificationTypeId = identificationTypeId
        };

        var customerErased = new CRMIndividualCustomer
        {
            TenantId = 1,
            Code = $"C3-{Guid.NewGuid():N}",
            Email = $"{Guid.NewGuid():N}@site.com",
            Phone = "300",
            StatusId = statusId,
            FirstName = erasedFirstName,
            LastName = "Last3",
            BirthDate = new DateTime(1992, 3, 3),
            IdentificationNumber = "ID-3",
            IdentificationTypeId = identificationTypeId,
            Erased = true
        };

        db.IndividualCustomers.AddRange(customer1, customer2, customerErased);
        db.SaveChanges();

        ids = (customer1.Id, customer2.Id);
        return erasedFirstName;
    }

}

