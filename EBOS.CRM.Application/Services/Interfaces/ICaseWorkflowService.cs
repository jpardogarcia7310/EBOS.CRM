using System;
using System.Threading;
using System.Threading.Tasks;
using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Application.Services.Interfaces;

public interface ICaseWorkflowService
{
    Task EnsureCanTransitionAsync(Case entity, string nextStatus, CancellationToken cancellationToken = default);
    Task ApplyStatusChangeAsync(Case entity, string nextStatus, DateTime timestamp, CancellationToken cancellationToken = default);
}
