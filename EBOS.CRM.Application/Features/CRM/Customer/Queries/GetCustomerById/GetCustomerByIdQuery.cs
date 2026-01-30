using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Customer.Queries.GetCustomerById;

public sealed record GetCustomerByIdQuery(long Id) : IRequest<CustomerResponse?>;
