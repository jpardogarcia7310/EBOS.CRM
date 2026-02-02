using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Commands.DeleteBranchOfficeAddress;

public record DeleteBranchOfficeAddressCommand(long Id) : IRequest<bool>;
