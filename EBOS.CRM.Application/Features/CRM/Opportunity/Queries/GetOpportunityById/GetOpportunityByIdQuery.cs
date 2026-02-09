using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Opportunity.Queries.GetOpportunityById;

public record GetOpportunityByIdQuery(long Id) : IRequest<OpportunityResponse?>;
