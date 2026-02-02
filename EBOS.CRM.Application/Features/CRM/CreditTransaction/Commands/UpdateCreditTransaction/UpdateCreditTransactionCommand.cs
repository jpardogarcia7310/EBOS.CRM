using EBOS.CRM.Application.Contracts.Requests.CRM.CreditTransaction;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CreditTransaction.Commands.UpdateCreditTransaction;

public record UpdateCreditTransactionCommand(long Id, UpdateCreditTransactionRequest CreditTransactionRequest) : IRequest<CreditTransactionResponse?>;
