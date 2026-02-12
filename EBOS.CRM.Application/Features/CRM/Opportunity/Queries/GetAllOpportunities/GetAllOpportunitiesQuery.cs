using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Opportunity.Queries.GetAllOpportunities;

public record GetAllOpportunitiesQuery(int PageNumber = 1, int PageSize = 50)
    : IRequest<PagedResult<OpportunityResponse>>;
