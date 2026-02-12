using EBOS.CRM.Contracts.Requests.CRM.BranchOffice;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.PatchBranchOffice;

public sealed record PatchBranchOfficeCommand(long Id, PatchBranchOfficeRequest BranchOfficeRequest)
    : IRequest<BranchOfficeResponse?>;




