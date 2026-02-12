using EBOS.CRM.Contracts.Requests.CRM.Opportunity;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Opportunity.Commands.CloseOpportunity;

public record CloseOpportunityCommand(long Id, CloseOpportunityRequest OpportunityRequest)
    : IRequest<OpportunityResponse?>;
