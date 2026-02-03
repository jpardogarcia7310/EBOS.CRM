using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.CreditAccount.Queries.GetAllCreditAccounts;

public record GetAllCreditAccountsQuery : IRequest<IReadOnlyCollection<CreditAccountResponse>>;









