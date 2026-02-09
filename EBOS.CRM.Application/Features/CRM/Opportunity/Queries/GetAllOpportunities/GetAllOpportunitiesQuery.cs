using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Contracts.Responses.Common;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Opportunity.Queries.GetAllOpportunities;

public record GetAllOpportunitiesQuery(int PageNumber = 1, int PageSize = 50)
    : IRequest<PagedResult<OpportunityResponse>>;
