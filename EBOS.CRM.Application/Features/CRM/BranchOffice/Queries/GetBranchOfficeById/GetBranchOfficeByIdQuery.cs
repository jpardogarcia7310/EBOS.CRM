using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Queries.GetBranchOfficeById;

public record GetBranchOfficeByIdQuery(long Id) : IRequest<BranchOfficeResponse?>;




