using EBOS.CRM.Contracts.Requests.Security;
using EBOS.CRM.Contracts.Responses.Security;

namespace EBOS.CRM.Domain.Interfaces.Services.Identity;

public interface IAuthenticationService
{
    Task<AuthenticatedUserResponse> AuthenticateAsync(AuthenticateUserRequest request,
        CancellationToken cancellationToken = default);
}