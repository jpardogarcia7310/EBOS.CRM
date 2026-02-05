using EBOS.CRM.Application.Contracts.Requests.Security;
using EBOS.CRM.Application.Contracts.Responses.Security;

namespace EBOS.CRM.Application.Services.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticatedUserResponse> AuthenticateAsync(AuthenticateUserRequest request,
        CancellationToken cancellationToken = default);
}
