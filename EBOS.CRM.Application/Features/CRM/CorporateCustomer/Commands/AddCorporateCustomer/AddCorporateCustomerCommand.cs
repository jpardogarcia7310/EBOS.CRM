using EBOS.CRM.Contracts.Requests.CRM.CorporateCustomer;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CorporateCustomer.Commands.AddCorporateCustomer;

public record AddCorporateCustomerCommand(AddCorporateCustomerRequest CorporateCustomerRequest) :
    IRequest<CorporateCustomerResponse>;




