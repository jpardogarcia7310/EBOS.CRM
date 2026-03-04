using EBOS.CRM.Api.HostedServices;
using EBOS.CRM.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace EBOS.CRM.ApiTests.HostedServices;

public class LookupSeedHostedServiceTest
{
    [Fact]
    public async Task StartAsync_WhenServiceAvailable_EnsuresCanonicalLookups()
    {
        var lookup = new Mock<ILookupSeedService>();

        var services = new ServiceCollection();
        services.AddSingleton(lookup.Object);
        var provider = services.BuildServiceProvider();

        var sut = new LookupSeedHostedService(
            provider.GetRequiredService<IServiceScopeFactory>(),
            new Mock<ILogger<LookupSeedHostedService>>().Object);

        await sut.StartAsync(CancellationToken.None);

        lookup.Verify(x => x.EnsureCanonicalLookupsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
