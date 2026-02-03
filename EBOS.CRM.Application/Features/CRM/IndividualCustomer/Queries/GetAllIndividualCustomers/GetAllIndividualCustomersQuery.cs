using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.IndividualCustomer.Queries.GetAllIndividualCustomers;

public record GetAllIndividualCustomersQuery : IRequest<IReadOnlyCollection<IndividualCustomerResponse>>;









