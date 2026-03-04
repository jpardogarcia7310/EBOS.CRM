using EBOS.CRM.Application.Features.CRM.Quote.Queries.GetQuoteById;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Quote.Queries.GetQuoteById;

public class GetQuoteByIdQueryHandlerTest
{
    private readonly Mock<IQuoteRepository> _repository = new();
    private readonly Mock<IMapper> _mapper = new();

    [Fact]
    public async Task Handle_WhenExists_ReturnsMappedResponse()
    {
        var entity = new global::EBOS.CRM.Domain.Entities.CRM.Quote
        {
            Id = 7, TenantId = 1, OpportunityId = 10, Status = "Draft", SubtotalAmount = 100m, DiscountAmount = 10m, TotalAmount = 90m
        };
        _repository.Setup(x => x.GetByIdAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        _mapper.Setup(x => x.Map<QuoteResponse>(entity))
            .Returns(new QuoteResponse(7, 1, 10, "Draft", null, 100m, 10m, 90m, null, null, true));

        var handler = new GetQuoteByIdQueryHandler(_repository.Object, _mapper.Object);
        var result = await handler.Handle(new GetQuoteByIdQuery(7), CancellationToken.None);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ReturnsNull()
    {
        _repository.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.Quote?)null);
        var handler = new GetQuoteByIdQueryHandler(_repository.Object, _mapper.Object);
        var result = await handler.Handle(new GetQuoteByIdQuery(99), CancellationToken.None);
        Assert.Null(result);
    }
}
