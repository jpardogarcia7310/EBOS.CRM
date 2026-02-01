using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;
using EBOS.CRM.Application.Contracts.Requests.Common;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.CustomerAddress.Queries.GetAllCustomerAddresses;

public record GetAllCustomerAddressesQuery(PagedQueryRequest Query) : IRequest<PagedResponse<CustomerAddressResponse>>;




