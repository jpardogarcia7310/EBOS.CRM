using EBOS.CRM.Contracts.Requests.Security;
using EBOS.CRM.Contracts.Responses.Security;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.Identity;

namespace EBOS.CRM.Infrastructure.Services.Security;

public sealed class AuthorizationService(IPolicyService policyService) : IAuthorizationService
{
    private readonly IPolicyService _policyService = policyService ?? throw new ArgumentNullException(nameof(policyService));

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