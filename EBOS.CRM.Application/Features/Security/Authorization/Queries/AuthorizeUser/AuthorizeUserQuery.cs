using EBOS.CRM.Contracts.Requests.Security;
using EBOS.CRM.Contracts.Responses.Security;
using MediatR;

namespace EBOS.CRM.Application.Features.Security.Authorization.Queries.AuthorizeUser;

public sealed record AuthorizeUserQuery(AuthorizeUserRequest Request) : IRequest<AuthorizeUserResponse>;
