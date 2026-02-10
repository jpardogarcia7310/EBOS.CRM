using EBOS.CRM.Application.Contracts.Requests.CRM.Opportunity;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Opportunity.Commands.PatchOpportunityStage;

public record PatchOpportunityStageCommand(long Id, PatchOpportunityStageRequest OpportunityRequest)
    : IRequest<OpportunityResponse?>;
