using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.CreditTransaction.Queries.GetCreditTransactionById;

public record GetCreditTransactionByIdQuery(long Id) : IRequest<CreditTransactionResponse?>;




