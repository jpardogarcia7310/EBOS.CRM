

using MediatR;


namespace EBOS.CRM.Application.Features.CRM.IndividualCustomer.Commands.DeleteIndividualCustomer;

public record DeleteIndividualCustomerCommand(long Id) : IRequest<bool>;




