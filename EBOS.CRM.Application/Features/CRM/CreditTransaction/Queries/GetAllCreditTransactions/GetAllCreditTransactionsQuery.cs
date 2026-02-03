using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.CreditTransaction.Queries.GetAllCreditTransactions;

public record GetAllCreditTransactionsQuery : IRequest<IReadOnlyCollection<CreditTransactionResponse>>;









