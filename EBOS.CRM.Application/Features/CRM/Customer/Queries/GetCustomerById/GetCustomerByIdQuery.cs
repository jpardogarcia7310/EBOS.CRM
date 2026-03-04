using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Customer.Queries.GetCustomerById;

public record GetCustomerByIdQuery(long Id) : IRequest<CustomerResponse?>;




