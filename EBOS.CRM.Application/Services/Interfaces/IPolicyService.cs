namespace EBOS.CRM.Application.Services.Interfaces;

public interface IPolicyService
{
    Task EnsureAuthorizedAsync(
        long userId,
        string policyCode,
        CancellationToken cancellationToken = default);
}
