using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.OpportunityStage.Queries.GetOpportunityStageById;

public record GetOpportunityStageByIdQuery(long Id) : IRequest<OpportunityStageResponse?>;
