using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.CorporateCustomer.Queries.GetAllCorporateCustomers;

public record GetAllCorporateCustomersQuery : IRequest<IReadOnlyCollection<CorporateCustomerResponse>>;









