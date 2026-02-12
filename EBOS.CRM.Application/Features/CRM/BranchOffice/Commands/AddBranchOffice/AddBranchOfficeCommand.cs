using EBOS.CRM.Contracts.Requests.CRM.BranchOffice;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.AddBranchOffice;

public record AddBranchOfficeCommand(AddBranchOfficeRequest BranchOfficeRequest) : IRequest<BranchOfficeResponse>;




