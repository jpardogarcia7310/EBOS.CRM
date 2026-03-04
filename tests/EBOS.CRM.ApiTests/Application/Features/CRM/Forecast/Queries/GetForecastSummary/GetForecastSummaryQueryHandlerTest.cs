using EBOS.CRM.Application.Features.CRM.Forecast.Queries.GetForecastSummary;
using EBOS.CRM.Contracts.Requests.CRM.Forecast;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using Moq;
using OpportunityEntity = EBOS.CRM.Domain.Entities.CRM.Opportunity;
using OpportunityStageEntity = EBOS.CRM.Domain.Entities.CRM.OpportunityStage;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Forecast.Queries.GetForecastSummary;

public class GetForecastSummaryQueryHandlerTest
{
    private readonly Mock<IOpportunityRepository> _opportunityRepository = new();
    private readonly Mock<IOpportunityStageRepository> _stageRepository = new();

    [Fact]
    public async Task Handle_ValidRequest_ReturnsStageSummary()
    {
        var now = DateTime.UtcNow;
        var opportunities = new List<OpportunityEntity>
        {
            new() { Id = 1, TenantId = 1, StageId = 10, Amount = 100m, Probability = 0.5m, Name = "Opp-1" },
            new() { Id = 2, TenantId = 1, StageId = 10, Amount = 200m, Probability = 0.25m, Name = "Opp-2" }
        };
        var stages = new List<OpportunityStageEntity>
        {
            new() { Id = 10, TenantId = 1, Name = "Qualified" }
        };

        _opportunityRepository
            .Setup(r => r.GetByForecastCriteriaAsync(1, null, null, It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(opportunities);
        _stageRepository
            .Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(stages);

        var handler = new GetForecastSummaryQueryHandler(_opportunityRepository.Object, _stageRepository.Object);

        var result = await handler.Handle(
            new GetForecastSummaryQuery(new GetForecastRequest(1, now.AddDays(-7), now, null, null)),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Stages);
        Assert.Equal(300m, result.TotalAmount);
        Assert.Equal(100m, result.WeightedAmount);
        Assert.Equal("Qualified", result.Stages.First().StageName);
    }

    [Fact]
    public async Task Handle_WhenForecastRequestIsNull_ThrowsArgumentNullException()
    {
        var handler = new GetForecastSummaryQueryHandler(_opportunityRepository.Object, _stageRepository.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            handler.Handle(new GetForecastSummaryQuery(null!), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenCanceled_ThrowsOperationCanceled()
    {
        var handler = new GetForecastSummaryQueryHandler(_opportunityRepository.Object, _stageRepository.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(new GetForecastSummaryQuery(new GetForecastRequest(1, null, null, null, null)), cts.Token));
    }
}
