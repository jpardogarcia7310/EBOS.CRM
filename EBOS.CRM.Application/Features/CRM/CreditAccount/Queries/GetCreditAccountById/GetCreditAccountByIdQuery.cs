using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CreditAccount.Queries.GetCreditAccountById;

public record GetCreditAccountByIdQuery(long Id) : IRequest<CreditAccountResponse?>;




