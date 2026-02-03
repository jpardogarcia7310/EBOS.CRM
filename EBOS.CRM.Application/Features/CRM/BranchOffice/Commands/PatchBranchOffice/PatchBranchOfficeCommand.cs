using EBOS.CRM.Application.Contracts.Requests.CRM.BranchOffice;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.PatchBranchOffice;

public sealed record PatchBranchOfficeCommand(long Id, PatchBranchOfficeRequest BranchOfficeRequest)
    : IRequest<BranchOfficeResponse?>;




