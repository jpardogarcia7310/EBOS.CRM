using EBOS.CRM.Application.Contracts.Requests.CRM;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.AddBranchOffice;

public sealed record AddBranchOfficeCommand(AddBranchOfficeRequest BranchOfficeRequest)
    : IRequest<BranchOfficeResponse>;
