using EBOS.CRM.Application.Contracts.Requests.CRM.BranchOfficeAddress;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Commands.AddBranchOfficeAddress;

public record AddBranchOfficeAddressCommand(AddBranchOfficeAddressRequest BranchOfficeAddressRequest) : IRequest<BranchOfficeAddressResponse>;
