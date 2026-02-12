using EBOS.CRM.Contracts.Requests.Security;
using EBOS.CRM.Contracts.Responses.Security;

namespace EBOS.CRM.Domain.Interfaces.Services;

public interface IAuthenticationService
{
    Task<AuthenticatedUserResponse> AuthenticateAsync(AuthenticateUserRequest request,
        CancellationToken cancellationToken = default);
}