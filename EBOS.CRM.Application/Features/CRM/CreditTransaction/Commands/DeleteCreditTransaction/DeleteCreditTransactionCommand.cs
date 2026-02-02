using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CreditTransaction.Commands.DeleteCreditTransaction;

public record DeleteCreditTransactionCommand(long Id) : IRequest<bool>;
