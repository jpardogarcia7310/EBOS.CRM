using EBOS.CRM.Application.Contracts.Requests.CRM.CorporateCustomer;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CorporateCustomer.Commands.UpdateCorporateCustomer;

public record UpdateCorporateCustomerCommand(long Id, UpdateCorporateCustomerRequest CorporateCustomerRequest) : IRequest<CorporateCustomerResponse?>;
