using EBOS.CRM.Application.Features.CRM.Opportunity.Queries.GetOpportunityById;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Opportunity.Queries.GetOpportunityById;

public class GetOpportunityByIdQueryHandlerTest
{
    private readonly Mock<IOpportunityRepository> _repository = new();
    private readonly Mock<IMapper> _mapper = new();

    [Fact]
    public async Task Handle_WhenExists_ReturnsMappedResponse()
    {
        var entity = new global::EBOS.CRM.Domain.Entities.CRM.Opportunity { Id = 7, TenantId = 1, Name = "Opp", StageId = 2, OwnerUserId = 3, CustomerId = 4, Amount = 100m, Probability = 0.5m };
        _repository.Setup(x => x.GetByIdAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        _mapper.Setup(x => x.Map<OpportunityResponse>(entity))
            .Returns(new OpportunityResponse(7, 1, "Opp", 2, 3, 4, null, 100m, 0.5m, null, null, null, true));

        var handler = new GetOpportunityByIdQueryHandler(_repository.Object, _mapper.Object);
        var result = await handler.Handle(new GetOpportunityByIdQuery(7), CancellationToken.None);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ReturnsNull()
    {
        _repository.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.Opportunity?)null);
        var handler = new GetOpportunityByIdQueryHandler(_repository.Object, _mapper.Object);
        var result = await handler.Handle(new GetOpportunityByIdQuery(99), CancellationToken.None);
        Assert.Null(result);
    }
}
