using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.DeleteCreditAccount;

public sealed record DeleteCreditAccountCommand(long Id) : IRequest<bool>;
