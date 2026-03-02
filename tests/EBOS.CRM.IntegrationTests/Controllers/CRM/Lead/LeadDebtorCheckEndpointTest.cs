using System.Net.Http.Json;
using EBOS.CRM.Contracts.Requests.CRM.Lead;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EBOS.CRM.IntegrationTests.Controllers.CRM.Lead;

public class LeadDebtorCheckEndpointTest(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task DebtorCheck_ReturnsMorosoCustomer()
    {
        SeedDebtor(factory);

        var request = new LeadDebtorCheckRequest(1, "moroso@example.com", "34600999888", null, "Jane Doe");
        var response = await _client.PostAsJsonAsync("/api/v2/Lead/debtor-check", request);
        response.EnsureSuccessStatusCode();

        var dto = await response.Content.ReadFromJsonAsync<LeadDebtorCheckResponse>();
        Assert.NotNull(dto);
        Assert.True(dto.IsDebtor);
        Assert.Equal("Moroso", dto.Status);
        Assert.True(dto.DebtAmount > 0);
    }

    private static void SeedDebtor(CustomWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();

        var moroso = db.Statuses.FirstOrDefault(s => s.Description == "Moroso");
        if (moroso == null)
        {
            moroso = new global::EBOS.CRM.Domain.Entities.EBOS.Status { Description = "Moroso" };
            db.Statuses.Add(moroso);
            db.SaveChanges();
        }

        var idType = db.IdentificationTypes.First();
        var customer = new global::EBOS.CRM.Domain.Entities.CRM.IndividualCustomer
        {
            TenantId = 1,
            Code = "IND-MOR",
            Email = "moroso@example.com",
            Phone = "34600999888",
            StatusId = moroso.Id,
            FirstName = "Jane",
            LastName = "Doe",
            BirthDate = new DateTime(1990, 1, 1),
            IdentificationNumber = "12345678",
            IdentificationTypeId = idType.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-60)
        };
        db.IndividualCustomers.Add(customer);
        db.SaveChanges();

        db.CreditAccounts.Add(new global::EBOS.CRM.Domain.Entities.CRM.CreditAccount
        {
            TenantId = 1,
            CustomerId = customer.Id,
            MaxAmount = 1000m,
            UsedAmount = 400m,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            CreatedBy = 1
        });
        db.SaveChanges();
    }
}

