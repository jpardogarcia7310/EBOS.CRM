using EBOS.CRM.Application.Features.CRM.OpportunityStage.Queries.GetAllOpportunityStages;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.OpportunityStage.Queries.GetAllOpportunityStages;

public class GetAllOpportunityStagesQueryHandlerTest
{
    private readonly Mock<IOpportunityStageRepository> _repository = new();
    private readonly Mock<IMapper> _mapper = new();

    [Fact]
    public async Task Handle_ReturnsPagedResult()
    {
        var entities = new List<global::EBOS.CRM.Domain.Entities.CRM.OpportunityStage>
        {
            new() { Id = 1, TenantId = 1, Name = "Qualified", Order = 1, DefaultProbability = 0.3m }
        };
        _repository.Setup(x => x.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(entities);
        _repository.Setup(x => x.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(entities.Count);
        _mapper.Setup(x => x.Map<IReadOnlyCollection<OpportunityStageResponse>>(entities)).Returns(new List<OpportunityStageResponse>());

        var handler = new GetAllOpportunityStagesQueryHandler(_repository.Object, _mapper.Object);
        var result = await handler.Handle(new GetAllOpportunityStagesQuery(), CancellationToken.None);

        Assert.NotNull(result);
        _repository.Verify(x => x.CountAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCanceled_ThrowsOperationCanceled()
    {
        var handler = new GetAllOpportunityStagesQueryHandler(_repository.Object, _mapper.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(new GetAllOpportunityStagesQuery(), cts.Token));
    }
}
