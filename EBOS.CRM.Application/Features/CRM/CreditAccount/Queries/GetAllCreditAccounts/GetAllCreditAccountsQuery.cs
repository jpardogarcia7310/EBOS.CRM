using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;
using EBOS.CRM.Application.Contracts.Requests.Common;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.CreditAccount.Queries.GetAllCreditAccounts;

public record GetAllCreditAccountsQuery(PagedQueryRequest Query) : IRequest<PagedResponse<CreditAccountResponse>>;




