using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.IndividualCustomer.Queries.GetIndividualCustomerById;

public record GetIndividualCustomerByIdQuery(long Id) : IRequest<IndividualCustomerResponse?>;




