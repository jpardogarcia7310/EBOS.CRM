using EBOS.CRM.Application.Contracts.Requests.Security;
using EBOS.CRM.Application.Contracts.Responses.Security;
using EBOS.CRM.Application.Services.Interfaces;

namespace EBOS.CRM.Infrastructure.Services.Security;

public sealed class AuthorizationService : IAuthorizationService
{
    public Task<AuthorizeUserResponse> AuthorizeAsync(AuthorizeUserRequest request,
        CancellationToken cancellationToken = default)
    {
        // Placeholder implementation for issue #67 (replace with real policy evaluation).
        return Task.FromResult(new AuthorizeUserResponse(true));
    }
}
