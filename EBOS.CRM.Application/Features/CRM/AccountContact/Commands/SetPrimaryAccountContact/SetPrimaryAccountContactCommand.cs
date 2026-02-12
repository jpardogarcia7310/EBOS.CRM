using EBOS.CRM.Contracts.Requests.CRM.AccountContact;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Commands.SetPrimaryAccountContact;

public record SetPrimaryAccountContactCommand(long Id, SetPrimaryAccountContactRequest AccountContactRequest)
    : IRequest<AccountContactResponse?>;
