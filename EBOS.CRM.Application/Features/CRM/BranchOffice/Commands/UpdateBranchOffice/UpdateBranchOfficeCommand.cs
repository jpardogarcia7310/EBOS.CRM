using EBOS.CRM.Contracts.Requests.CRM.BranchOffice;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.UpdateBranchOffice;

public record UpdateBranchOfficeCommand(long Id, UpdateBranchOfficeRequest BranchOfficeRequest) :
    IRequest<BranchOfficeResponse?>;




