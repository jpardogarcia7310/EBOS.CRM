namespace EBOS.CRM.Domain.Interfaces.Services;

public interface IPolicyService
{
    Task EnsureAuthorizedAsync(long userId, string policyCode, CancellationToken cancellationToken = default);
}