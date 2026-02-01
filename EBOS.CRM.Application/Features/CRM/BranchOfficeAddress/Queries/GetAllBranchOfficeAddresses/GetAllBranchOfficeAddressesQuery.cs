using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;
using EBOS.CRM.Application.Contracts.Requests.Common;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Queries.GetAllBranchOfficeAddresses;

public record GetAllBranchOfficeAddressesQuery(PagedQueryRequest Query) : IRequest<PagedResponse<BranchOfficeAddressResponse>>;




