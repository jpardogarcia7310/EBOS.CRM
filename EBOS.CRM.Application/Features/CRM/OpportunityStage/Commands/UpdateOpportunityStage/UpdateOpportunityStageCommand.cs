using EBOS.CRM.Application.Contracts.Requests.CRM.OpportunityStage;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.OpportunityStage.Commands.UpdateOpportunityStage;

public record UpdateOpportunityStageCommand(long Id, UpdateOpportunityStageRequest StageRequest)
    : IRequest<OpportunityStageResponse?>;
