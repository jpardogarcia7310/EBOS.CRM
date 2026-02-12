using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CreditTransaction.Queries.GetCreditTransactionById;

public record GetCreditTransactionByIdQuery(long Id) : IRequest<CreditTransactionResponse?>;




