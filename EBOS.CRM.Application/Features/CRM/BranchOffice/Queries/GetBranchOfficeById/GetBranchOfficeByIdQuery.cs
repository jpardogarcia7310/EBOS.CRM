using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Queries.GetBranchOfficeById;

public sealed record GetBranchOfficeByIdQuery(long Id) : IRequest<BranchOfficeResponse?>;
