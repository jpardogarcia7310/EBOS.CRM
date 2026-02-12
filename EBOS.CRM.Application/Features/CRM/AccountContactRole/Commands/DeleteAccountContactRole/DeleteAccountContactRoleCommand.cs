using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContactRole.Commands.DeleteAccountContactRole;

public record DeleteAccountContactRoleCommand(long Id) : IRequest<bool>;
