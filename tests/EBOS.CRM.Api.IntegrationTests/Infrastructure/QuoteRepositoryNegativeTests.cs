using EBOS.CRM.Api.IntegrationTests.Infrastructure;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Infrastructure.Options;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.Infrastructure.Repositories.Concrete.CRM;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Api.IntegrationTests.Infrastructure;

public class QuoteRepositoryNegativeTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Quote_Save_Throws_When_TenantId_Mismatch()
    {
        using var scope = factory.Services.CreateScope();
        var context = CreateTenantContext(scope.ServiceProvider, 1);
        var repository = new QuoteRepository(context);

        var quote = new Quote
        {
            TenantId = 2,
            OpportunityId = 1,
            Status = "Draft",
            SubtotalAmount = 1000m,
            DiscountAmount = 0m,
            TotalAmount = 1000m
        };

        await repository.AddAsync(quote);

        var act = async () => await repository.SaveChangesAsync();
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Quote_Save_Throws_When_Opportunity_Missing_On_Relational()
    {
        using var scope = factory.Services.CreateScope();
        var context = CreateTenantContext(scope.ServiceProvider, 1);
        if (!context.Database.IsRelational())
        {
            return;
        }

        var repository = new QuoteRepository(context);

        var quote = new Quote
        {
            TenantId = 1,
            OpportunityId = 999999,
            Status = "Draft",
            SubtotalAmount = 1000m,
            DiscountAmount = 0m,
            TotalAmount = 1000m
        };

        await repository.AddAsync(quote);

        var act = async () => await repository.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    private static CrmDbContext CreateTenantContext(IServiceProvider services, long tenantId)
    {
        var options = services.GetRequiredService<DbContextOptions<CrmDbContext>>();
        var multiTenantOptions = services.GetService<IOptions<MultiTenantOptions>>();
        var tenantContext = new TestTenantContext(tenantId);
        return new CrmDbContext(options, tenantContext, multiTenantOptions);
    }

    private sealed class TestTenantContext(long tenantId) : EBOS.CRM.Application.Services.Interfaces.ITenantContext
    {
        public long TenantId { get; } = tenantId;
    }
}
