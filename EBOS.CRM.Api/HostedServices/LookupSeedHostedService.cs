using EBOS.CRM.Domain.Interfaces.Services;

namespace EBOS.CRM.Api.HostedServices;

public sealed class LookupSeedHostedService(IServiceScopeFactory scopeFactory, ILogger<LookupSeedHostedService> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ILookupSeedService>();
            await service.EnsureCanonicalLookupsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Lookup seed failed. Startup will continue.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
