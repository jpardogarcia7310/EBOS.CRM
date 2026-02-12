using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;
using EBOS.CRM.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Queries.GetAllBranchOffices;

public record GetAllBranchOfficesQuery(int PageNumber = 1, int PageSize = 50) :
    IRequest<PagedResult<BranchOfficeResponse>>;









