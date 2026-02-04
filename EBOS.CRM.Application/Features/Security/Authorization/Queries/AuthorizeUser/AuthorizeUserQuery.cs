using EBOS.CRM.Application.Contracts.Requests.Security;
using EBOS.CRM.Application.Contracts.Responses.Security;
using MediatR;

namespace EBOS.CRM.Application.Features.Security.Authorization.Queries.AuthorizeUser;

public sealed record AuthorizeUserQuery(AuthorizeUserRequest Request)
    : IRequest<AuthorizeUserResponse>;
