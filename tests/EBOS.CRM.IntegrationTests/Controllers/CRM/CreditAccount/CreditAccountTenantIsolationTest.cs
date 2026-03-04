using System.Net;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using CRMCustomer = EBOS.CRM.Domain.Entities.CRM.Customer;
using CRMCreditAccount = EBOS.CRM.Domain.Entities.CRM.CreditAccount;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.CreditAccount;

public class CreditAccountTenantIsolationTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "CreditAccount");

    [Fact]
    public async Task GetAll_Filters_By_Tenant_Header()
    {
        SeedCreditAccounts(out var data);

        var clientTenant1 = HttpClientFactory.CreateClientWithTenant(factory, 1);
        var response1 = await clientTenant1.GetAsync($"/api/v{_version}/CreditAccount");
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant1 = await response1.Content.ReadItemsAsync<CreditAccountResponse>();
        itemsTenant1.Should().Contain(i => i.CustomerId == data.Customer1Id && i.Active);
        itemsTenant1.Should().NotContain(i => i.CustomerId == data.Customer2Id);
        itemsTenant1.Should().NotContain(i => i.CustomerId == data.ErasedCustomerId);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(factory, 2);
        var response2 = await clientTenant2.GetAsync($"/api/v{_version}/CreditAccount");
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant2 = await response2.Content.ReadItemsAsync<CreditAccountResponse>();
        itemsTenant2.Should().Contain(i => i.CustomerId == data.Customer2Id);
        itemsTenant2.Should().NotContain(i => i.CustomerId == data.Customer1Id);
    }

    [Fact]
    public async Task GetById_Returns_404_When_Requesting_Other_Tenant_Data()
    {
        SeedCreditAccounts(out var data);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(factory, 2);
        var response = await clientTenant2.GetAsync($"/api/v{_version}/CreditAccount/{data.CreditAccount1Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private void SeedCreditAccounts(out (long CreditAccount1Id, long CreditAccount2Id, long Customer1Id, long Customer2Id, long ErasedCustomerId) data)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var statusId = db.Statuses.Select(s => s.Id).First();

        var customer1 = new CRMCustomer
        {
            TenantId = 1,
            Code = $"C1-{Guid.NewGuid():N}",
            Email = $"{Guid.NewGuid():N}@site.com",
            Phone = "100",
            StatusId = statusId
        };

        var customer2 = new CRMCustomer
        {
            TenantId = 2,
            Code = $"C2-{Guid.NewGuid():N}",
            Email = $"{Guid.NewGuid():N}@site.com",
            Phone = "200",
            StatusId = statusId
        };

        var customerErased = new CRMCustomer
        {
            TenantId = 1,
            Code = $"C3-{Guid.NewGuid():N}",
            Email = $"{Guid.NewGuid():N}@site.com",
            Phone = "300",
            StatusId = statusId
        };

        db.Customers.AddRange(customer1, customer2, customerErased);
        db.SaveChanges();

        var account1 = new CRMCreditAccount
        {
            TenantId = 1,
            CustomerId = customer1.Id,
            MaxAmount = 1000m,
            UsedAmount = 100m
        };

        var account2 = new CRMCreditAccount
        {
            TenantId = 2,
            CustomerId = customer2.Id,
            MaxAmount = 2000m,
            UsedAmount = 200m
        };

        var accountErased = new CRMCreditAccount
        {
            TenantId = 1,
            CustomerId = customerErased.Id,
            MaxAmount = 3000m,
            UsedAmount = 0m,
            Erased = true
        };

        db.CreditAccounts.AddRange(account1, account2, accountErased);
        db.SaveChanges();

        data = (account1.Id, account2.Id, customer1.Id, customer2.Id, customerErased.Id);
    }

}

