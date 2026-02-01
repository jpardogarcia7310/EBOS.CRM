using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;
using EBOS.CRM.Application.Contracts.Requests.Common;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.CreditTransaction.Queries.GetAllCreditTransactions;

public record GetAllCreditTransactionsQuery(PagedQueryRequest Query) : IRequest<PagedResponse<CreditTransactionResponse>>;




