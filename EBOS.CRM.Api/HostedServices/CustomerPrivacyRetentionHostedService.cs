using EBOS.CRM.Api.Options;
using EBOS.CRM.Application.Features.CRM.CustomerPrivacy;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Api.HostedServices;

public sealed class CustomerPrivacyRetentionHostedService(
    IServiceScopeFactory scopeFactory,
    IOptions<CustomerPrivacyRetentionJobOptions> options,
    ILogger<CustomerPrivacyRetentionHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var cfg = options.Value;
        if (!cfg.Enabled)
        {
            logger.LogInformation("CustomerPrivacyRetentionHostedService disabled.");
            return;
        }

        var delay = TimeSpan.FromMinutes(Math.Max(1, cfg.SweepIntervalMinutes));
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SweepAsync(cfg, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Customer privacy retention sweep failed.");
            }

            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task SweepAsync(CustomerPrivacyRetentionJobOptions cfg, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICustomerPrivacyRequestRepository>();
        var retentionService = scope.ServiceProvider.GetRequiredService<CustomerPrivacyRetentionService>();

        var tenantIds = (await repository.GetAllAsync(cancellationToken))
            .Select(x => x.TenantId)
            .Distinct()
            .ToList();

        foreach (var tenantId in tenantIds)
        {
            var result = await retentionService.RunAsync(
                tenantId,
                cfg.DryRun,
                retentionDays: null,
                batchSize: cfg.BatchSize,
                actorUserId: cfg.SystemUserId,
                correlationId: $"retention-job-{Guid.NewGuid():N}",
                cancellationToken);

            logger.LogInformation(
                "CustomerPrivacyRetention run tenant={TenantId} dryRun={DryRun} candidates={Candidates} affected={Affected} cutoff={Cutoff}",
                result.TenantId, result.DryRun, result.Candidates, result.Affected, result.CutoffUtc);
        }
    }
}
