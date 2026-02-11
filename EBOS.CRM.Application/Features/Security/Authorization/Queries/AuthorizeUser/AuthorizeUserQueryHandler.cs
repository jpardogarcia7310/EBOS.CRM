using EBOS.CRM.Application.Contracts.Responses.Security;
using EBOS.CRM.Application.Services.Interfaces;
using MediatR;

namespace EBOS.CRM.Application.Features.Security.Authorization.Queries.AuthorizeUser;

public sealed class AuthorizeUserQueryHandler(IAuthorizationService authorizationService)
    : IRequestHandler<AuthorizeUserQuery, AuthorizeUserResponse>
{
    private readonly IAuthorizationService _authorizationService = authorizationService
        ?? throw new ArgumentNullException(nameof(authorizationService));

    public Task<AuthorizeUserResponse> Handle(AuthorizeUserQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return _authorizationService.AuthorizeAsync(request.Request, cancellationToken);
    }
}
