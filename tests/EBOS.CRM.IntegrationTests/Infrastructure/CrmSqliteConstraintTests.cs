using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Entities.CRM;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EBOS.CRM.IntegrationTests.Infrastructure;

public class CrmSqliteConstraintTests
{
    [Fact]
    public async Task Address_Enforces_Check_Constraints()
    {
        using var context = SqliteCrmContextFactory.Create();
        var country = await EnsureCountryAsync(context);
        var addressType = await EnsureAddressTypeAsync(context);

        var address = BuildValidAddress(country.Id, addressType.Id);
        address.Latitude = 95;
        context.Add(address);

        var act = async () => await context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task Address_Enforces_GoogleMapsUrl_Constraint()
    {
        using var context = SqliteCrmContextFactory.Create();
        var country = await EnsureCountryAsync(context);
        var addressType = await EnsureAddressTypeAsync(context);

        var address = BuildValidAddress(country.Id, addressType.Id);
        address.GoogleMapsUrl = "http://maps.invalid";
        context.Add(address);

        var act = async () => await context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task Customer_Enforces_Email_Constraint()
    {
        using var context = SqliteCrmContextFactory.Create();
        var status = await EnsureStatusAsync(context);

        var customer = BuildValidCustomer(status.Id);
        customer.Email = "invalid-email";
        context.Add(customer);

        var act = async () => await context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }


    [Fact]
    public async Task CreditAccount_Enforces_Amount_Constraints()
    {
        using var context = SqliteCrmContextFactory.Create();
        var status = await EnsureStatusAsync(context);
        var customer = BuildValidCustomer(status.Id);
        context.Add(customer);
        await context.SaveChangesAsync();

        var creditAccount = new CreditAccount
        {
            TenantId = 1,
            CustomerId = customer.Id,
            MaxAmount = -1m,
            UsedAmount = 0m
        };
        context.Add(creditAccount);

        var act = async () => await context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task CreditTransaction_Enforces_Type_Constraint()
    {
        using var context = SqliteCrmContextFactory.Create();
        var status = await EnsureStatusAsync(context);
        var customer = BuildValidCustomer(status.Id);
        context.Add(customer);
        await context.SaveChangesAsync();

        var account = new CreditAccount
        {
            TenantId = 1,
            CustomerId = customer.Id,
            MaxAmount = 1000m,
            UsedAmount = 0m
        };
        context.Add(account);
        await context.SaveChangesAsync();

        var transaction = new CreditTransaction
        {
            TenantId = 1,
            CreditAccountId = account.Id,
            Date = DateTime.UtcNow,
            Amount = 100m,
            Type = "InvalidType"
        };
        context.Add(transaction);

        var act = async () => await context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }


    [Fact]
    public async Task Lead_Enforces_Email_Constraint()
    {
        using var context = SqliteCrmContextFactory.Create();

        var lead = BuildValidLead();
        lead.Email = "invalid-email";
        context.Add(lead);

        var act = async () => await context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }


    [Fact]
    public async Task Lead_Enforces_EstimatedValue_Constraint()
    {
        using var context = SqliteCrmContextFactory.Create();

        var lead = BuildValidLead();
        lead.EstimatedValue = -1m;
        context.Add(lead);

        var act = async () => await context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task Opportunity_Enforces_Amount_Constraint()
    {
        using var context = SqliteCrmContextFactory.Create();
        var status = await EnsureStatusAsync(context);
        var customer = BuildValidCustomer(status.Id);
        context.Add(customer);
        await context.SaveChangesAsync();
        var stage = await EnsureOpportunityStageAsync(context);

        var opportunity = BuildValidOpportunity(customer.Id, stage.Id);
        opportunity.Amount = -1m;
        context.Add(opportunity);

        var act = async () => await context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task Opportunity_Enforces_Probability_Constraint()
    {
        using var context = SqliteCrmContextFactory.Create();
        var status = await EnsureStatusAsync(context);
        var customer = BuildValidCustomer(status.Id);
        context.Add(customer);
        await context.SaveChangesAsync();
        var stage = await EnsureOpportunityStageAsync(context);

        var opportunity = BuildValidOpportunity(customer.Id, stage.Id);
        opportunity.Probability = 2m;
        context.Add(opportunity);

        var act = async () => await context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task OpportunityStage_Enforces_DefaultProbability_Constraint()
    {
        using var context = SqliteCrmContextFactory.Create();

        var stage = new OpportunityStage
        {
            TenantId = 1,
            Name = "Invalid",
            Order = 1,
            DefaultProbability = 2m,
            IsClosed = false,
            IsWon = false
        };
        context.Add(stage);

        var act = async () => await context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task Quote_Enforces_Discount_Constraint()
    {
        using var context = SqliteCrmContextFactory.Create();
        var status = await EnsureStatusAsync(context);
        var customer = BuildValidCustomer(status.Id);
        context.Add(customer);
        await context.SaveChangesAsync();
        var stage = await EnsureOpportunityStageAsync(context);
        var opportunity = BuildValidOpportunity(customer.Id, stage.Id);
        context.Add(opportunity);
        await context.SaveChangesAsync();

        var quote = BuildValidQuote(opportunity.Id);
        quote.DiscountAmount = quote.SubtotalAmount + 1m;
        context.Add(quote);

        var act = async () => await context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task Quote_Enforces_Total_Constraint()
    {
        using var context = SqliteCrmContextFactory.Create();
        var status = await EnsureStatusAsync(context);
        var customer = BuildValidCustomer(status.Id);
        context.Add(customer);
        await context.SaveChangesAsync();
        var stage = await EnsureOpportunityStageAsync(context);
        var opportunity = BuildValidOpportunity(customer.Id, stage.Id);
        context.Add(opportunity);
        await context.SaveChangesAsync();

        var quote = BuildValidQuote(opportunity.Id);
        quote.TotalAmount = -1m;
        context.Add(quote);

        var act = async () => await context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    private static Address BuildValidAddress(long countryId, long addressTypeId)
        => new()
        {
            TenantId = 1,
            Street = "Main St",
            ExternalNumber = "123",
            City = "City",
            StateOrProvince = "State",
            PostalCode = "12345",
            CountryId = countryId,
            AddressTypeId = addressTypeId
        };

    private static Customer BuildValidCustomer(long statusId)
        => new()
        {
            TenantId = 1,
            Code = "CUST-001",
            Email = "customer@test.com",
            Phone = "1234567890",
            StatusId = statusId
        };

    private static Lead BuildValidLead()
        => new()
        {
            TenantId = 1,
            Source = "Web",
            Status = "New",
            OwnerUserId = 10,
            CompanyName = "Acme",
            ContactName = "Jane Doe",
            Email = "lead@test.com",
            Phone = "123456",
            EstimatedValue = 100m
        };

    private static Opportunity BuildValidOpportunity(long customerId, long stageId)
        => new()
        {
            TenantId = 1,
            Name = "Deal A",
            StageId = stageId,
            OwnerUserId = 10,
            CustomerId = customerId,
            ExpectedCloseDate = DateTime.UtcNow.AddDays(30),
            Amount = 1000m,
            Probability = 0.5m
        };

    private static Quote BuildValidQuote(long opportunityId)
        => new()
        {
            TenantId = 1,
            OpportunityId = opportunityId,
            Status = "Draft",
            SubtotalAmount = 1000m,
            DiscountAmount = 0m,
            TotalAmount = 1000m
        };

    private static async Task<Country> EnsureCountryAsync(DbContext context)
    {
        var existing = await context.Set<Country>().FirstOrDefaultAsync();
        if (existing != null)
        {
            return existing;
        }

        var country = new Country
        {
            Name = "Test",
            Iso31661A2Code = "TS",
            Iso31661A3Code = "TST",
            Iso31661NumCode = "999",
            Domain = ".ts",
            Currency = "Test",
            CurrencyCode = "TST",
            InternationalPhoneCode = "1"
        };
        context.Add(country);
        await context.SaveChangesAsync();
        return country;
    }

    private static async Task<AddressType> EnsureAddressTypeAsync(DbContext context)
    {
        var existing = await context.Set<AddressType>().FirstOrDefaultAsync();
        if (existing != null)
        {
            return existing;
        }

        var addressType = new AddressType
        {
            Code = "HOME",
            Description = "Home",
            Category = "Shipping",
            AllowsMultiple = true,
            RequiresPrimary = false
        };
        context.Add(addressType);
        await context.SaveChangesAsync();
        return addressType;
    }

    private static async Task<Status> EnsureStatusAsync(DbContext context)
    {
        var existing = await context.Set<Status>().FirstOrDefaultAsync();
        if (existing != null)
        {
            return existing;
        }

        var status = new Status { Description = "Active" };
        context.Add(status);
        await context.SaveChangesAsync();
        return status;
    }

    private static async Task<OpportunityStage> EnsureOpportunityStageAsync(DbContext context)
    {
        var existing = await context.Set<OpportunityStage>().FirstOrDefaultAsync();
        if (existing != null)
        {
            return existing;
        }

        var stage = new OpportunityStage
        {
            TenantId = 1,
            Name = "Prospecting",
            Order = 1,
            DefaultProbability = 0.1m,
            IsClosed = false,
            IsWon = false
        };
        context.Add(stage);
        await context.SaveChangesAsync();
        return stage;
    }
}
