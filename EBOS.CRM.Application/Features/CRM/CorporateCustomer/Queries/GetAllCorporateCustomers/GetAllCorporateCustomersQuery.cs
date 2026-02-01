using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;
using EBOS.CRM.Application.Contracts.Requests.Common;
using EBOS.CRM.Application.Contracts.Responses.Common;

namespace EBOS.CRM.Application.Features.CRM.CorporateCustomer.Queries.GetAllCorporateCustomers;

public record GetAllCorporateCustomersQuery(PagedQueryRequest Query) : IRequest<PagedResponse<CorporateCustomerResponse>>;




