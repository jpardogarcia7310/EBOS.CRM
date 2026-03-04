using EBOS.CRM.Application.Features.CRM.Opportunity.Queries.GetAllOpportunities;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Opportunity.Queries.GetAllOpportunities;

public class GetAllOpportunitiesQueryHandlerTest
{
    private readonly Mock<IOpportunityRepository> _repository = new();
    private readonly Mock<IMapper> _mapper = new();

    [Fact]
    public async Task Handle_ReturnsPagedResult()
    {
        var entities = new List<global::EBOS.CRM.Domain.Entities.CRM.Opportunity>
        {
            new() { Id = 1, TenantId = 1, Name = "Opp", StageId = 2, OwnerUserId = 3, CustomerId = 4, Amount = 100m, Probability = 0.5m }
        };
        _repository.Setup(x => x.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(entities);
        _repository.Setup(x => x.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(entities.Count);
        _mapper.Setup(x => x.Map<IReadOnlyCollection<OpportunityResponse>>(entities)).Returns(new List<OpportunityResponse>());

        var handler = new GetAllOpportunitiesQueryHandler(_repository.Object, _mapper.Object);
        var result = await handler.Handle(new GetAllOpportunitiesQuery(), CancellationToken.None);

        Assert.NotNull(result);
        _repository.Verify(x => x.CountAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCanceled_ThrowsOperationCanceled()
    {
        var handler = new GetAllOpportunitiesQueryHandler(_repository.Object, _mapper.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(new GetAllOpportunitiesQuery(), cts.Token));
    }
}
