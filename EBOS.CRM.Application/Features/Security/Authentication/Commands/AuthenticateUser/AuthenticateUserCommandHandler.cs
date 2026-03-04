using EBOS.CRM.Contracts.Requests.Security;
using EBOS.CRM.Contracts.Responses.Security;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.Identity;
using MediatR;

namespace EBOS.CRM.Application.Features.Security.Authentication.Commands.AuthenticateUser;

public sealed class AuthenticateUserCommandHandler(IAuthenticationService authenticationService)
    : IRequestHandler<AuthenticateUserCommand, AuthenticatedUserResponse>
{
    private readonly IAuthenticationService _authenticationService = authenticationService
        ?? throw new ArgumentNullException(nameof(authenticationService));

    public async Task<AuthenticatedUserResponse> Handle(AuthenticateUserCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var payload = request.Request;
        var serviceRequest = new AuthenticateUserRequest(
            payload.ExternalId,
            payload.Username,
            payload.Email,
            payload.DisplayName,
            payload.IsActive);

        var serviceResponse = await _authenticationService.AuthenticateAsync(serviceRequest, cancellationToken);

        return new AuthenticatedUserResponse(
            serviceResponse.UserId,
            serviceResponse.ExternalId,
            serviceResponse.Username,
            serviceResponse.Email,
            serviceResponse.DisplayName,
            serviceResponse.IsActive,
            serviceResponse.Roles,
            serviceResponse.Permissions);
    }
}