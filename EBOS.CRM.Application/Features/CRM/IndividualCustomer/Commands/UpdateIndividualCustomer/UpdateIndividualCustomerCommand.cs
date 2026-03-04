using EBOS.CRM.Contracts.Requests.CRM.IndividualCustomer;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.IndividualCustomer.Commands.UpdateIndividualCustomer;

public record UpdateIndividualCustomerCommand(long Id, UpdateIndividualCustomerRequest IndividualCustomerRequest) :
    IRequest<IndividualCustomerResponse?>;




