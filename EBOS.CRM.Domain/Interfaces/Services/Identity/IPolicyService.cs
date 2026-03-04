namespace EBOS.CRM.Domain.Interfaces.Services.Identity;

public interface IPolicyService
{
    Task EnsureAuthorizedAsync(long userId, string policyCode, CancellationToken cancellationToken = default);
}