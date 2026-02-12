using EBOS.CRM.Contracts.Requests.CRM.CreditAccount;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CreditAccount.Commands.UpdateCreditAccount;

public record UpdateCreditAccountCommand(long Id, UpdateCreditAccountRequest CreditAccountRequest) : IRequest<CreditAccountResponse?>;




