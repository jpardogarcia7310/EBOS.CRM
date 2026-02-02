using EBOS.CRM.Application.Contracts.Requests.Common;
using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Application.Contracts.Responses.Common;
using MediatR;

namespace EBOS.CRM.Application.Features.AddressesType.Query.GetAllAddressesType;

public record GetAllAddressesTypeQuery(PagedQueryRequest Query) : IRequest<PagedResponse<AddressTypeResponse>>;





