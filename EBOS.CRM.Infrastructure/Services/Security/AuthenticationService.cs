using EBOS.CRM.Contracts.Requests.Security;
using EBOS.CRM.Contracts.Responses.Security;
using EBOS.CRM.Domain.Interfaces.Services;

namespace EBOS.CRM.Infrastructure.Services.Security;

public sealed class AuthenticationService : IAuthenticationService
{
    public Task<AuthenticatedUserResponse> AuthenticateAsync(AuthenticateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        // Placeholder implementation for issue #67 (replace with real IdP integration).
        var response = new AuthenticatedUserResponse(
            0,
            request.ExternalId,
            request.Username,
            request.Email,
            request.DisplayName,
            request.IsActive,
            Array.Empty<string>(),
            Array.Empty<string>());

        return Task.FromResult(response);
    }
}