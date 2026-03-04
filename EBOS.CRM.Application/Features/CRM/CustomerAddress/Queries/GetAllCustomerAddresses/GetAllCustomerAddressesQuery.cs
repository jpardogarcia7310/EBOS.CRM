using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;
using EBOS.CRM.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.CustomerAddress.Queries.GetAllCustomerAddresses;

public record GetAllCustomerAddressesQuery(int PageNumber = 1, int PageSize = 50) :
    IRequest<PagedResult<CustomerAddressResponse>>;









