using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Address.Commands.DeleteAddress;

public record DeleteAddressCommand(long Id) : IRequest<bool>;
