using EBOS.CRM.Contracts.Requests.CRM.AccountContactRole;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContactRole.Commands.AddAccountContactRole;

public record AddAccountContactRoleCommand(AddAccountContactRoleRequest AccountContactRoleRequest)
    : IRequest<AccountContactRoleResponse>;
