using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;
using EBOS.CRM.Application.Contracts.Requests.Common;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.Address.Queries.GetAllAddresses;

public record GetAllAddressQuery(PagedQueryRequest Query) : IRequest<PagedResponse<AddressResponse>>;




