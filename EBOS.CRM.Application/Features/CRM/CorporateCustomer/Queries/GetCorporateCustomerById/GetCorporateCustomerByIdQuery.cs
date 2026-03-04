using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CorporateCustomer.Queries.GetCorporateCustomerById;

public record GetCorporateCustomerByIdQuery(long Id) : IRequest<CorporateCustomerResponse?>;




