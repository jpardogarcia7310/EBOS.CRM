using EBOS.CRM.Contracts.Requests.CRM.OpportunityStage;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.OpportunityStage.Commands.UpdateOpportunityStage;

public record UpdateOpportunityStageCommand(long Id, UpdateOpportunityStageRequest StageRequest)
    : IRequest<OpportunityStageResponse?>;
