using EBOS.CRM.Application.Contracts.Requests.CRM.Opportunity;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Opportunity.Commands.UpdateOpportunity;

public record UpdateOpportunityCommand(long Id, UpdateOpportunityRequest OpportunityRequest)
    : IRequest<OpportunityResponse?>;
