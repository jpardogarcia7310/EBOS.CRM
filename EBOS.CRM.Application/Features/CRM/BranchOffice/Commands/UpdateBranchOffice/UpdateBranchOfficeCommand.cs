using EBOS.CRM.Application.Contracts.Requests.CRM;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.UpdateBranchOffice;

public sealed record UpdateBranchOfficeCommand(UpdateBranchOfficeRequest BranchOfficeRequest)
    : IRequest<BranchOfficeResponse?>;
