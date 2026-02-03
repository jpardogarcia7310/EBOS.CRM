using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Queries.GetAllBranchOffices;

public record GetAllBranchOfficesQuery : IRequest<IReadOnlyCollection<BranchOfficeResponse>>;









