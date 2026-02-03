using EBOS.CRM.Application.Contracts.Requests.CRM.IndividualCustomer;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.IndividualCustomer.Commands.UpdateIndividualCustomer;

public record UpdateIndividualCustomerCommand(long Id, UpdateIndividualCustomerRequest IndividualCustomerRequest) : IRequest<IndividualCustomerResponse?>;




