using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;
using EBOS.CRM.Application.Contracts.Requests.Common;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Queries.GetAllBranchOffices;

public record GetAllBranchOfficesQuery(PagedQueryRequest Query) : IRequest<PagedResponse<BranchOfficeResponse>>;




