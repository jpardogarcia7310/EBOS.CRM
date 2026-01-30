using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Queries.GetAllBranchOffices;

public sealed record GetAllBranchOfficesQuery() : IRequest<ICollection<BranchOfficeResponse>>;
