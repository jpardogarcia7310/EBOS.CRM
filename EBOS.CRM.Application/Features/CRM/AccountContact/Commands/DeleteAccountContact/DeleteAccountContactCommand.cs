using EBOS.CRM.Contracts.Requests.CRM.AccountContact;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Commands.DeleteAccountContact;

public record DeleteAccountContactCommand(long Id, DeleteAccountContactRequest AccountContactRequest) : IRequest<bool>;
