using EBOS.CRM.Contracts.Requests.Security;
using EBOS.CRM.Contracts.Responses.Security;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.Identity;
using MediatR;

namespace EBOS.CRM.Application.Features.Security.Authorization.Queries.AuthorizeUser;

public sealed class AuthorizeUserQueryHandler(IAuthorizationService authorizationService)
    : IRequestHandler<AuthorizeUserQuery, AuthorizeUserResponse>
{
    private readonly IAuthorizationService _authorizationService = authorizationService
        ?? throw new ArgumentNullException(nameof(authorizationService));

    public async Task<AuthorizeUserResponse> Handle(AuthorizeUserQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var payload = request.Request;
        var serviceRequest = new AuthorizeUserRequest(payload.UserId, payload.PolicyCode);
        var serviceResponse = await _authorizationService.AuthorizeAsync(serviceRequest, cancellationToken);

        return new AuthorizeUserResponse(serviceResponse.IsAuthorized);
    }
}