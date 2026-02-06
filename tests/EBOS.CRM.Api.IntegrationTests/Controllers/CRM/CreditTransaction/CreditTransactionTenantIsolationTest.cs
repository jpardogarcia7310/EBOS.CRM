using System.Net;
using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Api.IntegrationTests.TestUtils;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using CRMCustomer = EBOS.CRM.Domain.Entities.CRM.Customer;
using CRMCreditAccount = EBOS.CRM.Domain.Entities.CRM.CreditAccount;
using CRMCreditTransaction = EBOS.CRM.Domain.Entities.CRM.CreditTransaction;

namespace EBOS.CRM.Api.IntegrationTests.Controllers.CRM.CreditTransaction;

public class CreditTransactionTenantIsolationTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly string _version;

    public CreditTransactionTenantIsolationTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _version = ApiVersionHelper.GetLatestVersion(factory, "CreditTransaction");
    }

    [Fact]
    public async Task GetAll_Filters_By_Tenant_Header()
    {
        SeedCreditTransactions(out var data);

        var clientTenant1 = HttpClientFactory.CreateClientWithTenant(_factory, 1);
        var response1 = await clientTenant1.GetAsync($"/api/v{_version}/CreditTransaction");
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant1 = await response1.Content.ReadItemsAsync<CreditTransactionResponse>();
        itemsTenant1.Should().Contain(i => i.CreditAccountId == data.CreditAccount1Id && i.Active);
        itemsTenant1.Should().NotContain(i => i.CreditAccountId == data.CreditAccount2Id);
        itemsTenant1.Should().NotContain(i => i.ExternalReference == data.ErasedReference);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(_factory, 2);
        var response2 = await clientTenant2.GetAsync($"/api/v{_version}/CreditTransaction");
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant2 = await response2.Content.ReadItemsAsync<CreditTransactionResponse>();
        itemsTenant2.Should().Contain(i => i.CreditAccountId == data.CreditAccount2Id);
        itemsTenant2.Should().NotContain(i => i.CreditAccountId == data.CreditAccount1Id);
    }

    [Fact]
    public async Task GetById_Returns_404_When_Requesting_Other_Tenant_Data()
    {
        SeedCreditTransactions(out var data);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(_factory, 2);
        var response = await clientTenant2.GetAsync($"/api/v{_version}/CreditTransaction/{data.CreditTransaction1Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private void SeedCreditTransactions(out (long CreditTransaction1Id, long CreditTransaction2Id, long CreditAccount1Id, long CreditAccount2Id, string ErasedReference) data)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var statusId = db.Statuses.Select(s => s.Id).First();
        var erasedReference = $"EXT-ERASED-{Guid.NewGuid():N}";

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

        db.Customers.AddRange(customer1, customer2);
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

        db.CreditAccounts.AddRange(account1, account2);
        db.SaveChanges();

        var transaction1 = new CRMCreditTransaction
        {
            TenantId = 1,
            CreditAccountId = account1.Id,
            Date = DateTime.UtcNow,
            Amount = 100m,
            Type = "Consumption",
            ExternalReference = "EXT-1",
            Comments = "Comment-1"
        };

        var transaction2 = new CRMCreditTransaction
        {
            TenantId = 2,
            CreditAccountId = account2.Id,
            Date = DateTime.UtcNow,
            Amount = 200m,
            Type = "Adjustment",
            ExternalReference = "EXT-2",
            Comments = "Comment-2"
        };

        var transactionErased = new CRMCreditTransaction
        {
            TenantId = 1,
            CreditAccountId = account1.Id,
            Date = DateTime.UtcNow,
            Amount = 999m,
            Type = "Consumption",
            ExternalReference = erasedReference,
            Comments = "Erased",
            Erased = true
        };

        db.CreditTransactions.AddRange(transaction1, transaction2, transactionErased);
        db.SaveChanges();

        data = (transaction1.Id, transaction2.Id, account1.Id, account2.Id, erasedReference);
    }

}
