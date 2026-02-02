using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;
using EBOS.CRM.Application.Contracts.Requests.Common;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.Customer.Queries.GetAllCustomers;

public record GetAllCustomersQuery(PagedQueryRequest Query) : IRequest<PagedResponse<CustomerResponse>>;




