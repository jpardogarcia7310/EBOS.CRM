using EBOS.CRM.Contracts.Requests.Security;
using EBOS.CRM.Contracts.Responses.Security;

namespace EBOS.CRM.Domain.Interfaces.Services.Identity;

public interface IAuthorizationService
{
    Task<AuthorizeUserResponse> AuthorizeAsync(AuthorizeUserRequest request,
        CancellationToken cancellationToken = default);
}