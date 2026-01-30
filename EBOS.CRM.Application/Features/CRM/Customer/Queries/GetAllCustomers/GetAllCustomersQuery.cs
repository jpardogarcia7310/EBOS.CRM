using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Customer.Queries.GetAllCustomers;

public sealed record GetAllCustomersQuery() : IRequest<ICollection<CustomerResponse>>;
