using EBOS.CRM.Application.Contracts.Requests.CRM;
using EBOS.CRM.Application.Contracts.Requests.CRM.CreditTransaction;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CreditTransaction.Commands.AddCreditTransaction;

public record AddCreditTransactionCommand(AddCreditTransactionRequest CreditTransactionRequest) : IRequest<CreditTransactionResponse>;
