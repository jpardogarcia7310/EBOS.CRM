using EBOS.CRM.Contracts.Requests.Security;
using EBOS.CRM.Contracts.Responses.Security;
using EBOS.CRM.Domain.Interfaces.Services.Models;

namespace EBOS.CRM.Domain.Interfaces.Services;

public interface IAuthorizationService
{
    Task<AuthorizeUserResponse> AuthorizeAsync(AuthorizeUserRequest request,
        CancellationToken cancellationToken = default);
}