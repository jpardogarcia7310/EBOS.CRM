using EBOS.CRM.Application.Contracts.Requests.CRM.IndividualCustomer;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.IndividualCustomer.Commands.AddIndividualCustomer;

public record AddIndividualCustomerCommand(AddIndividualCustomerRequest IndividualCustomerRequest) : IRequest<IndividualCustomerResponse>;
