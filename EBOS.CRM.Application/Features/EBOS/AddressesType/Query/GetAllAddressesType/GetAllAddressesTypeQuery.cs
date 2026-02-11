using EBOS.CRM.Application.Contracts.Responses;
using MediatR;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.EBOS.AddressesType.Query.GetAllAddressesType;

public record GetAllAddressesTypeQuery(int PageNumber = 1, int PageSize = 50) :
    IRequest<PagedResult<AddressTypeResponse>>;










