using EBOS.CRM.Application.Contracts.Requests.Security;
using EBOS.CRM.Application.Contracts.Responses.Security;

namespace EBOS.CRM.Application.Services.Interfaces;

public interface IAuthorizationService
{
    Task<AuthorizeUserResponse> AuthorizeAsync(
        AuthorizeUserRequest request,
        CancellationToken cancellationToken = default);
}
