using EBOS.CRM.Application.Contracts.Requests.Security;
using EBOS.CRM.Application.Contracts.Responses.Security;
using EBOS.CRM.Application.Services.Interfaces;

namespace EBOS.CRM.Infrastructure.Services.Security;

public sealed class AuthorizationService : IAuthorizationService
{
    private readonly IPolicyService _policyService;

    public AuthorizationService(IPolicyService policyService)
    {
        _policyService = policyService ?? throw new ArgumentNullException(nameof(policyService));
    }

    public async Task<AuthorizeUserResponse> AuthorizeAsync(AuthorizeUserRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _policyService.EnsureAuthorizedAsync(request.UserId, request.PolicyCode, cancellationToken)
                .ConfigureAwait(false);
            return new AuthorizeUserResponse(true);
        }
        catch (UnauthorizedAccessException)
        {
            return new AuthorizeUserResponse(false);
        }
    }
}
