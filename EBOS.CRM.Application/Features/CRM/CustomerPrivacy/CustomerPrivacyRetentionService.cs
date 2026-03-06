using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;

namespace EBOS.CRM.Application.Features.CRM.CustomerPrivacy;

public sealed class CustomerPrivacyRetentionService(
    ICustomerPrivacyRequestRepository privacyRequestRepository,
    ITenantConfigurationRepository tenantConfigurationRepository,
    IAuditService auditService)
{
    private const string RetentionDaysConfigKey = "customer360.privacy.retention.days";

    public async Task<CustomerPrivacyRetentionRunResponse> RunAsync(long tenantId, bool dryRun, int? retentionDays,
        int? batchSize, long actorUserId, string? correlationId, CancellationToken cancellationToken)
    {
        if (tenantId <= 0)
        {
            throw new DomainValidationException("TenantId must be a positive value.", "DOMAIN_VALIDATION_TENANT_ID_POSITIVE");
        }

        if (actorUserId <= 0)
        {
            throw new DomainValidationException("Actor user id must be a positive value.", "DOMAIN_VALIDATION_ACTOR_USER_ID_POSITIVE");
        }
        try
        {
            var resolvedDays = retentionDays ?? await ResolveRetentionDaysAsync(tenantId, cancellationToken);
            var resolvedBatchSize = batchSize.GetValueOrDefault(500);
            resolvedBatchSize = Math.Clamp(resolvedBatchSize, 1, 5000);
            var cutoff = DateTime.UtcNow.AddDays(-resolvedDays);

            var all = await privacyRequestRepository.GetAllAsync(cancellationToken);
            var candidates = all
                .Where(x => x.TenantId == tenantId)
                .Where(x =>
                    x.Status == CustomerPrivacyRequest.StatusCompleted ||
                    x.Status == CustomerPrivacyRequest.StatusFailed ||
                    x.Status == CustomerPrivacyRequest.StatusCanceled)
                .Where(x => (x.ProcessedAt ?? x.RequestedAt) <= cutoff)
                .OrderBy(x => x.Id)
                .Take(resolvedBatchSize)
                .ToList();

            if (dryRun)
            {
                return new CustomerPrivacyRetentionRunResponse(
                    tenantId,
                    true,
                    resolvedDays,
                    resolvedBatchSize,
                    cutoff,
                    candidates.Count,
                    0);
            }

            foreach (var candidate in candidates)
            {
                candidate.Erased = true;
                await privacyRequestRepository.UpdateAsync(candidate, cancellationToken);
            }

            await privacyRequestRepository.SaveChangesAsync(cancellationToken);

            var summary = new
            {
                tenantId,
                retentionDays = resolvedDays,
                batchSize = resolvedBatchSize,
                cutoffUtc = cutoff,
                affected = candidates.Count
            };

            await auditService.InsertAuditAsync(new AuditInsertRequest(
                UserId: actorUserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Delete,
                Entity: nameof(CustomerPrivacyRequest),
                RegisterId: tenantId,
                OldValues: null,
                NewValues: AuditSerialization.Serialize(summary),
                CorrelationId: correlationId ?? $"retention-{Guid.NewGuid():N}"), cancellationToken);

            return new CustomerPrivacyRetentionRunResponse(
                tenantId,
                false,
                resolvedDays,
                resolvedBatchSize,
                cutoff,
                candidates.Count,
                candidates.Count);
        }
        catch (Exception ex) when (DomainTransientFailureClassifier.TryClassify(ex, nameof(RunAsync), out var transient))
        {
            throw transient;
        }
    }

    private async Task<int> ResolveRetentionDaysAsync(long tenantId, CancellationToken cancellationToken)
    {
        var config = (await tenantConfigurationRepository.GetAllAsync(cancellationToken))
            .Where(x => x.TenantId == tenantId && x.Key == RetentionDaysConfigKey)
            .OrderByDescending(x => x.UpdatedAt)
            .FirstOrDefault();

        if (config is null)
        {
            return 90;
        }

        if (int.TryParse(config.ValueJson, out var parsed) && parsed > 0)
        {
            return parsed;
        }

        return 90;
    }
}
