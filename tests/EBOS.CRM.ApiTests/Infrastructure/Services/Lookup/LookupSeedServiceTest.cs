using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.Infrastructure.Services.Lookup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace EBOS.CRM.ApiTests.Infrastructure.Services.Lookup;

public class LookupSeedServiceTest
{
    [Fact]
    public async Task EnsureCanonicalLookupsAsync_SeedsAndNormalizes()
    {
        var options = BuildOptions();
        await using var context = new CrmDbContext(options);
        var normalization = new Mock<ILookupNormalizationService>(MockBehavior.Strict);
        normalization.Setup(x => x.NormalizeAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var sut = new LookupSeedService(context, normalization.Object);
        await sut.EnsureCanonicalLookupsAsync();

        Assert.True(await context.Countries.AnyAsync(c => c.Iso31661A2Code == "ES"));
        Assert.True(await context.AddressTypes.AnyAsync(a => a.Code == "HOME"));
        Assert.True(await context.IdentificationTypes.AnyAsync(i => i.Code == "DNI"));
        Assert.True(await context.Statuses.AnyAsync(s => s.Description == "Active"));
        normalization.Verify(x => x.NormalizeAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    private static DbContextOptions<CrmDbContext> BuildOptions()
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        return new DbContextOptionsBuilder<CrmDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .UseInternalServiceProvider(serviceProvider)
            .Options;
    }
}
