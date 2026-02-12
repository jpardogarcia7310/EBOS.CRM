using EBOS.CRM.Contracts.Requests.Security;
using EBOS.CRM.Contracts.Responses.Security;
using MediatR;

namespace EBOS.CRM.Application.Features.Security.Authentication.Commands.AuthenticateUser;

public sealed record AuthenticateUserCommand(AuthenticateUserRequest Request)
    : IRequest<AuthenticatedUserResponse>;
