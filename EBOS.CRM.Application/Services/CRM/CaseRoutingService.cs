using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;

namespace EBOS.CRM.Application.Services.CRM;

public sealed class CaseRoutingService(IQueueRepository queueRepository) : ICaseRoutingService
{
    private readonly IQueueRepository _queueRepository = queueRepository
        ?? throw new ArgumentNullException(nameof(queueRepository));

    public async Task<RouteCaseResult> RouteAsync(Case entity, bool force, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!force && entity.QueueId > 0)
        {
            var currentQueue = await _queueRepository.GetByIdAsync(entity.QueueId, cancellationToken);
            if (currentQueue is not null && currentQueue.IsActive && currentQueue.TenantId == entity.TenantId)
            {
                var owner = entity.OwnerUserId > 0 ? entity.OwnerUserId : currentQueue.DefaultOwnerUserId;
                return new RouteCaseResult(currentQueue.Id, owner, "current-queue");
            }
        }

        var queues = await _queueRepository.GetAllAsync(cancellationToken);
        var candidates = queues
            .Where(q => q.TenantId == entity.TenantId && q.IsActive)
            .ToList();

        if (candidates.Count == 0)
        {
            throw new InvalidOperationException("No active queues available for routing.");
        }

        var selected = candidates
            .OrderByDescending(q => q.DefaultOwnerUserId.HasValue)
            .ThenBy(q => q.Id)
            .First();

        var selectedOwner = entity.OwnerUserId > 0 && !force
            ? entity.OwnerUserId
            : selected.DefaultOwnerUserId;

        return new RouteCaseResult(selected.Id, selectedOwner, "default-active-queue");
    }
}
