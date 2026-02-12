using EBOS.CRM.Application.Contracts.Responses.Security;
using EBOS.CRM.Application.Services.Interfaces;
using MediatR;

namespace EBOS.CRM.Application.Features.Security.Authentication.Commands.AuthenticateUser;

public sealed class AuthenticateUserCommandHandler(IAuthenticationService authenticationService)
    : IRequestHandler<AuthenticateUserCommand, AuthenticatedUserResponse>
{
    private readonly IAuthenticationService _authenticationService = authenticationService
        ?? throw new ArgumentNullException(nameof(authenticationService));

    public Task<AuthenticatedUserResponse> Handle(AuthenticateUserCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _authenticationService.AuthenticateAsync(request.Request, cancellationToken);
    }
}
