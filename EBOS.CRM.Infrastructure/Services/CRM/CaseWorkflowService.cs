using EBOS.CRM.Application.Options;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Infrastructure.Services.CRM;

public sealed class CaseWorkflowService(ICaseActivityRepository activityRepository, IOptions<CaseWorkflowOptions> options)
    : ICaseWorkflowService
{
    private readonly ICaseActivityRepository _activityRepository = activityRepository
        ?? throw new ArgumentNullException(nameof(activityRepository));

    private readonly CaseWorkflowOptions _options = options?.Value
        ?? throw new ArgumentNullException(nameof(options));

    public async Task EnsureCanTransitionAsync(Case entity, string nextStatus,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.Equals(nextStatus, Case.StatusClosed, StringComparison.OrdinalIgnoreCase)
            && !_options.AllowCloseWithOpenActivities)
        {
            var hasOpen = await _activityRepository.HasOpenByCaseIdAsync(entity.Id, cancellationToken);
            if (hasOpen)
            {
                throw new InvalidOperationException("Cannot close case with open activities.");
            }
        }
    }

    public async Task ApplyStatusChangeAsync(Case entity, string nextStatus, DateTime timestamp,
        CancellationToken cancellationToken = default)
    {
        await EnsureCanTransitionAsync(entity, nextStatus, cancellationToken);

        if (string.Equals(nextStatus, Case.StatusClosed, StringComparison.OrdinalIgnoreCase))
        {
            entity.Close(timestamp);
        }
        else if (string.Equals(nextStatus, Case.StatusReopened, StringComparison.OrdinalIgnoreCase))
        {
            entity.Reopen();
        }
        else
        {
            entity.SetStatus(nextStatus);
        }

        entity.UpdatedAt = timestamp;
    }
}