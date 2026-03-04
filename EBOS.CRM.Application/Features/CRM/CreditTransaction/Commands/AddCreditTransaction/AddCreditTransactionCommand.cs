using EBOS.CRM.Contracts.Requests.CRM.CreditTransaction;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CreditTransaction.Commands.AddCreditTransaction;

public record AddCreditTransactionCommand(AddCreditTransactionRequest CreditTransactionRequest) :
    IRequest<CreditTransactionResponse>;




