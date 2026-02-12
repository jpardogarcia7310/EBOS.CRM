using EBOS.CRM.Contracts.Requests.CRM.Opportunity;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Opportunity.Commands.UpdateOpportunity;

public record UpdateOpportunityCommand(long Id, UpdateOpportunityRequest OpportunityRequest)
    : IRequest<OpportunityResponse?>;
