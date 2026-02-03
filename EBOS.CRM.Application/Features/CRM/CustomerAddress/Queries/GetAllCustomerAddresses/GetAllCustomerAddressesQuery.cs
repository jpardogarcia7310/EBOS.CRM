using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.CustomerAddress.Queries.GetAllCustomerAddresses;

public record GetAllCustomerAddressesQuery : IRequest<IReadOnlyCollection<CustomerAddressResponse>>;









