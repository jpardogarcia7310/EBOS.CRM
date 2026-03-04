using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContactRole.Queries.GetAccountContactRoleById;

public record GetAccountContactRoleByIdQuery(long Id) : IRequest<AccountContactRoleResponse?>;
