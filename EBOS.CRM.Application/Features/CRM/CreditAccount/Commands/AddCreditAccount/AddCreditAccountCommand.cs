using EBOS.CRM.Application.Contracts.Requests.CRM;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.AddCreditAccount;

public sealed record AddCreditAccountCommand(AddCreditAccountRequest CreditAccountRequest)
    : IRequest<CreditAccountResponse>;
