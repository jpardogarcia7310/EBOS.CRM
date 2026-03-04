using EBOS.CRM.Application.Features.CRM.OpportunityStage.Queries.GetOpportunityStageById;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.OpportunityStage.Queries.GetOpportunityStageById;

public class GetOpportunityStageByIdQueryHandlerTest
{
    private readonly Mock<IOpportunityStageRepository> _repository = new();
    private readonly Mock<IMapper> _mapper = new();

    [Fact]
    public async Task Handle_WhenExists_ReturnsMappedResponse()
    {
        var entity = new global::EBOS.CRM.Domain.Entities.CRM.OpportunityStage { Id = 7, TenantId = 1, Name = "Qualified", Order = 1, DefaultProbability = 0.3m };
        _repository.Setup(x => x.GetByIdAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        _mapper.Setup(x => x.Map<OpportunityStageResponse>(entity))
            .Returns(new OpportunityStageResponse(7, 1, "Qualified", 1, 0.3m, false, false, true));

        var handler = new GetOpportunityStageByIdQueryHandler(_repository.Object, _mapper.Object);
        var result = await handler.Handle(new GetOpportunityStageByIdQuery(7), CancellationToken.None);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ReturnsNull()
    {
        _repository.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.OpportunityStage?)null);
        var handler = new GetOpportunityStageByIdQueryHandler(_repository.Object, _mapper.Object);
        var result = await handler.Handle(new GetOpportunityStageByIdQuery(99), CancellationToken.None);
        Assert.Null(result);
    }
}
