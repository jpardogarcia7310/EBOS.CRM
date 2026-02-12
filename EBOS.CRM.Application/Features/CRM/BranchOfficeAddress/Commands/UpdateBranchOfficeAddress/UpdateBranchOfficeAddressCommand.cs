using EBOS.CRM.Contracts.Requests.CRM.BranchOfficeAddress;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Commands.UpdateBranchOfficeAddress;

public record UpdateBranchOfficeAddressCommand(long Id, UpdateBranchOfficeAddressRequest BranchOfficeAddressRequest) :
    IRequest<BranchOfficeAddressResponse?>;




