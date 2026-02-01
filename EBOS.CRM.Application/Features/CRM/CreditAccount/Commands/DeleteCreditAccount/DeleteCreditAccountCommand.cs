using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.DeleteCreditAccount;

public record DeleteCreditAccountCommand(long Id) : IRequest<bool>;
