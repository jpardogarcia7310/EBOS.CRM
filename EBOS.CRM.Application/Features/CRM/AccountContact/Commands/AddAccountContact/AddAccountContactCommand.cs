using EBOS.CRM.Contracts.Requests.CRM.AccountContact;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Commands.AddAccountContact;

public record AddAccountContactCommand(AddAccountContactRequest AccountContactRequest)
    : IRequest<AccountContactResponse>;
