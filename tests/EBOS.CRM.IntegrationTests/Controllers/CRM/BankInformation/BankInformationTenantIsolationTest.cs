using System.Net;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.IntegrationTests.Infrastructure;
using EBOS.CRM.IntegrationTests.TestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using CRMCustomer = global::EBOS.CRM.Domain.Entities.CRM.Customer;
using CRMBankInformation = global::EBOS.CRM.Domain.Entities.CRM.BankInformation;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.BankInformation;

public class BankInformationTenantIsolationTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly string _version = ApiVersionHelper.GetLatestVersion(factory, "BankInformation");

    [Fact]
    public async Task GetAll_Filters_By_Tenant_Header()
    {
        var iban1 = $"IBAN-{Guid.NewGuid():N}";
        var iban2 = $"IBAN-{Guid.NewGuid():N}";
        var erasedIban = SeedBankInformation(iban1, iban2, out var _);

        var clientTenant1 = HttpClientFactory.CreateClientWithTenant(factory, 1);
        var response1 = await clientTenant1.GetAsync($"/api/v{_version}/BankInformation");
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant1 = await response1.Content.ReadItemsAsync<BankInformationResponse>();
        itemsTenant1.Should().Contain(i => i.Iban == iban1 && i.Active);
        itemsTenant1.Should().NotContain(i => i.Iban == iban2);
        itemsTenant1.Should().NotContain(i => i.Iban == erasedIban);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(factory, 2);
        var response2 = await clientTenant2.GetAsync($"/api/v{_version}/BankInformation");
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsTenant2 = await response2.Content.ReadItemsAsync<BankInformationResponse>();
        itemsTenant2.Should().Contain(i => i.Iban == iban2);
        itemsTenant2.Should().NotContain(i => i.Iban == iban1);
    }

    [Fact]
    public async Task GetById_Returns_404_When_Requesting_Other_Tenant_Data()
    {
        var iban1 = $"IBAN-{Guid.NewGuid():N}";
        var iban2 = $"IBAN-{Guid.NewGuid():N}";
        SeedBankInformation(iban1, iban2, out var ids);

        var clientTenant2 = HttpClientFactory.CreateClientWithTenant(factory, 2);
        var response = await clientTenant2.GetAsync($"/api/v{_version}/BankInformation/{ids.Tenant1Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private string SeedBankInformation(string iban1, string iban2, out (long Tenant1Id, long Tenant2Id) ids)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
        var statusId = db.Statuses.Select(s => s.Id).First();
        var erasedIban = $"IBAN-ERASED-{Guid.NewGuid():N}";

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

        var bank1 = new CRMBankInformation
        {
            TenantId = 1,
            Iban = iban1,
            Bic = "BIC1",
            BankName = "Bank1",
            CustomerId = customer1.Id
        };

        var bank2 = new CRMBankInformation
        {
            TenantId = 2,
            Iban = iban2,
            Bic = "BIC2",
            BankName = "Bank2",
            CustomerId = customer2.Id
        };

        var bankErased = new CRMBankInformation
        {
            TenantId = 1,
            Iban = erasedIban,
            Bic = "BICX",
            BankName = "BankX",
            CustomerId = customerErased.Id,
            Erased = true
        };

        db.BankInformation.AddRange(bank1, bank2, bankErased);
        db.SaveChanges();

        ids = (bank1.Id, bank2.Id);
        return erasedIban;
    }

}

