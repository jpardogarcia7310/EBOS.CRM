using EBOS.CRM.Application.Features.CRM.Quote.Queries.GetAllQuotes;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Quote.Queries.GetAllQuotes;

public class GetAllQuotesQueryHandlerTest
{
    private readonly Mock<IQuoteRepository> _repository = new();
    private readonly Mock<IMapper> _mapper = new();

    [Fact]
    public async Task Handle_ReturnsPagedResult()
    {
        var entities = new List<global::EBOS.CRM.Domain.Entities.CRM.Quote>
        {
            new() { Id = 1, TenantId = 1, OpportunityId = 10, Status = "Draft", SubtotalAmount = 100m, DiscountAmount = 10m, TotalAmount = 90m }
        };
        _repository.Setup(x => x.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(entities);
        _repository.Setup(x => x.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(entities.Count);
        _mapper.Setup(x => x.Map<IReadOnlyCollection<QuoteResponse>>(entities)).Returns(new List<QuoteResponse>());

        var handler = new GetAllQuotesQueryHandler(_repository.Object, _mapper.Object);
        var result = await handler.Handle(new GetAllQuotesQuery(), CancellationToken.None);

        Assert.NotNull(result);
        _repository.Verify(x => x.CountAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCanceled_ThrowsOperationCanceled()
    {
        var handler = new GetAllQuotesQueryHandler(_repository.Object, _mapper.Object);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(new GetAllQuotesQuery(), cts.Token));
    }
}
