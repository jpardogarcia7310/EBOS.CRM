using EBOS.CRM.Contracts.Requests.CRM.CreditAccount;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.PatchCreditAccount;

public sealed record PatchCreditAccountCommand(long Id, PatchCreditAccountRequest CreditAccountRequest)
    : IRequest<CreditAccountResponse?>;




