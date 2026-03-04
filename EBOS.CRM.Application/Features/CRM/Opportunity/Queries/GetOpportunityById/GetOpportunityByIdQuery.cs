using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Opportunity.Queries.GetOpportunityById;

public record GetOpportunityByIdQuery(long Id) : IRequest<OpportunityResponse?>;
