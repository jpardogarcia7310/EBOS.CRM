using EBOS.CRM.Contracts.Requests.CRM.AccountContactRole;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContactRole.Commands.DeleteAccountContactRole;

public record DeleteAccountContactRoleCommand(long Id, DeleteAccountContactRoleRequest AccountContactRoleRequest)
    : IRequest<bool>;
