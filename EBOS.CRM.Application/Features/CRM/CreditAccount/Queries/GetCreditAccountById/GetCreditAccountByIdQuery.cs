using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CreditAccount.Queries.GetCreditAccountById;

public sealed record GetCreditAccountByIdQuery(long Id) : IRequest<CreditAccountResponse?>;
