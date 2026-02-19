using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Forecast.Queries.GetForecastSummary;

public class GetForecastSummaryQueryHandler(IOpportunityRepository opportunityRepository,
    IOpportunityStageRepository stageRepository)
    : IRequestHandler<GetForecastSummaryQuery, ForecastSummaryResponse>
{
    public async Task<ForecastSummaryResponse> Handle(GetForecastSummaryQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var filter = request.ForecastRequest ?? throw new ArgumentNullException(nameof(request.ForecastRequest));

        var opportunities = await opportunityRepository.GetByForecastCriteriaAsync(
            filter.TenantId,
            filter.OwnerUserId,
            filter.StageId,
            filter.From,
            filter.To,
            cancellationToken);
        var stages = await stageRepository.GetActiveAsync(cancellationToken);
        var stageNames = stages.ToDictionary(s => s.Id, s => s.Name);

        var filtered = opportunities.AsEnumerable();

        var grouped = filtered
            .GroupBy(o => o.StageId)
            .Select(g =>
            {
                var totalAmount = g.Sum(x => x.Amount);
                var weightedAmount = g.Sum(x => x.Amount * x.Probability);
                var stageName = stageNames.TryGetValue(g.Key, out var name) ? name : "Unknown";
                return new ForecastStageSummaryResponse(
                    g.Key,
                    stageName,
                    g.Count(),
                    totalAmount,
                    weightedAmount);
            })
            .OrderBy(s => s.StageId)
            .ToList();

        var total = grouped.Sum(x => x.TotalAmount);
        var weighted = grouped.Sum(x => x.WeightedAmount);

        return new ForecastSummaryResponse(
            filter.From,
            filter.To,
            filter.OwnerUserId,
            filter.StageId,
            grouped,
            total,
            weighted);
    }
}
