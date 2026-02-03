using EBOS.CRM.Application.Contracts.Requests.CRM.BranchOffice;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.AddBranchOffice;

public record AddBranchOfficeCommand(AddBranchOfficeRequest BranchOfficeRequest) : IRequest<BranchOfficeResponse>;




