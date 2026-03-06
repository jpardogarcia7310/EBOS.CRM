using EBOS.CRM.Application.Features.CRM.Quote.Commands.UpdateQuote;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.CRM.Quote;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Quote.Commands.UpdateQuote;

public class UpdateQuoteCommandHandlerTest
{
    [Fact]
    public async Task Handle_WhenNotFound_ReturnsNull()
    {
        var repository = new Mock<IQuoteRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var mapper = new Mock<IMapper>();
        var quoteOpportunityValidation = new Mock<IQuoteOpportunityValidationService>();
        repository.Setup(x => x.GetByIdAsync(404, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.Quote?)null);

        var handler = new UpdateQuoteCommandHandler(repository.Object, audit.Object, currentUser.Object, mapper.Object, quoteOpportunityValidation.Object);
        var req = new UpdateQuoteRequest(404, 1, 10, "Draft", "Q-1", 100m, 10m, 90m, null, null);
        var result = await handler.Handle(new UpdateQuoteCommand(404, req), CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenValid_UpdatesAndReturnsDto()
    {
        var repository = new Mock<IQuoteRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var mapper = new Mock<IMapper>();
        var quoteOpportunityValidation = new Mock<IQuoteOpportunityValidationService>();
        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        audit.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var entity = new global::EBOS.CRM.Domain.Entities.CRM.Quote
        {
            Id = 1, TenantId = 1, OpportunityId = 10, Status = "Draft", SubtotalAmount = 100m, DiscountAmount = 10m, TotalAmount = 90m
        };
        repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        quoteOpportunityValidation.Setup(x => x.EnsureOpportunityAvailableAsync(1, 10, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mapper.Setup(x => x.Map(It.IsAny<UpdateQuoteRequest>(), entity)).Callback(() => entity.Status = "Approved");
        mapper.Setup(x => x.Map<QuoteResponse>(entity))
            .Returns(new QuoteResponse(1, 1, 10, "Approved", "Q-1", 100m, 10m, 90m, null, null, true));

        var handler = new UpdateQuoteCommandHandler(repository.Object, audit.Object, currentUser.Object, mapper.Object, quoteOpportunityValidation.Object);
        var req = new UpdateQuoteRequest(1, 1, 10, "Approved", "Q-1", 100m, 10m, 90m, null, null);
        var result = await handler.Handle(new UpdateQuoteCommand(1, req), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Approved", result!.Status);
        repository.Verify(x => x.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }
}
