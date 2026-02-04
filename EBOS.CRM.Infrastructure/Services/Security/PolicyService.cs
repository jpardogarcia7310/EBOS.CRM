using EBOS.CRM.Application.Services.Interfaces;

namespace EBOS.CRM.Infrastructure.Services.Security;

public sealed class PolicyService : IPolicyService
{
    public Task EnsureAuthorizedAsync(
        long userId,
        string policyCode,
        CancellationToken cancellationToken = default)
    {
        // Placeholder implementation for issue #67 (replace with real RBAC policy evaluation).
        return Task.CompletedTask;
    }
}
