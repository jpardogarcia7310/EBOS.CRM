using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Queries.GetAllBranchOfficeAddresses;

public record GetAllBranchOfficeAddressesQuery(int PageNumber = 1, int PageSize = 50) :
    IRequest<PagedResult<BranchOfficeAddressResponse>>;









