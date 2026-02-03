using EBOS.CRM.Application.Contracts.Requests.CRM.CreditAccount;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.PatchCreditAccount;

public sealed record PatchCreditAccountCommand(long Id, PatchCreditAccountRequest CreditAccountRequest)
    : IRequest<CreditAccountResponse?>;




