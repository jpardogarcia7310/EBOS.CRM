using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MediatR;
using System.Linq;

namespace EBOS.CRM.Application.Features.CRM.Forecast.Queries.GetForecastSummary;

public class GetForecastSummaryQueryHandler(IOpportunityRepository opportunityRepository,
    IOpportunityStageRepository stageRepository)
    : IRequestHandler<GetForecastSummaryQuery, ForecastSummaryResponse>
{
    public async Task<ForecastSummaryResponse> Handle(GetForecastSummaryQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var filter = request.ForecastRequest ?? throw new ArgumentNullException(nameof(request.ForecastRequest));

        var opportunities = await opportunityRepository.GetAllAsync(cancellationToken);
        var stages = await stageRepository.GetAllAsync(cancellationToken);
        var stageNames = stages.ToDictionary(s => s.Id, s => s.Name);

        var filtered = opportunities.AsEnumerable();

        if (filter.OwnerUserId.HasValue)
        {
            filtered = filtered.Where(o => o.OwnerUserId == filter.OwnerUserId.Value);
        }

        if (filter.StageId.HasValue)
        {
            filtered = filtered.Where(o => o.StageId == filter.StageId.Value);
        }

        if (filter.From.HasValue)
        {
            filtered = filtered.Where(o => o.ExpectedCloseDate.HasValue && o.ExpectedCloseDate.Value >= filter.From.Value);
        }

        if (filter.To.HasValue)
        {
            filtered = filtered.Where(o => o.ExpectedCloseDate.HasValue && o.ExpectedCloseDate.Value <= filter.To.Value);
        }

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
