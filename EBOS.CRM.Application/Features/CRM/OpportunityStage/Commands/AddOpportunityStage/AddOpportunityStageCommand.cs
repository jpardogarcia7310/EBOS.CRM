using EBOS.CRM.Contracts.Requests.CRM.OpportunityStage;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.OpportunityStage.Commands.AddOpportunityStage;

public record AddOpportunityStageCommand(AddOpportunityStageRequest StageRequest) : IRequest<OpportunityStageResponse>;
