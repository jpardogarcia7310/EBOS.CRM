using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Commands.DeleteAccountContact;

public record DeleteAccountContactCommand(long Id) : IRequest<bool>;
