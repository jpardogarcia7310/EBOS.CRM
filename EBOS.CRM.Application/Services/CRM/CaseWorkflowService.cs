using System;
using System.Threading;
using System.Threading.Tasks;
using EBOS.CRM.Application.Options;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Application.Services.CRM;

public sealed class CaseWorkflowService(
    ICaseActivityRepository activityRepository,
    IOptions<CaseWorkflowOptions> options) : ICaseWorkflowService
{
    private readonly ICaseActivityRepository _activityRepository = activityRepository
        ?? throw new ArgumentNullException(nameof(activityRepository));
    private readonly CaseWorkflowOptions _options = options?.Value ?? new CaseWorkflowOptions();

    public async Task EnsureCanTransitionAsync(Case entity, string nextStatus, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.Equals(nextStatus, Case.StatusClosed, StringComparison.OrdinalIgnoreCase))
        {
            if (!_options.AllowCloseWithOpenActivities &&
                await _activityRepository.HasOpenByCaseIdAsync(entity.Id, cancellationToken))
            {
                throw new InvalidOperationException("Case has open activities and cannot be closed.");
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
            return;
        }

        if (string.Equals(nextStatus, Case.StatusReopened, StringComparison.OrdinalIgnoreCase))
        {
            entity.Reopen();
            return;
        }

        entity.SetStatus(nextStatus);
    }
}
