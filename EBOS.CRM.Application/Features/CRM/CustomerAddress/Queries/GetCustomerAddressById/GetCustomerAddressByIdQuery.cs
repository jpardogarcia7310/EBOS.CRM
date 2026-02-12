using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerAddress.Queries.GetCustomerAddressById;

public record GetCustomerAddressByIdQuery(long Id) : IRequest<CustomerAddressResponse?>;




