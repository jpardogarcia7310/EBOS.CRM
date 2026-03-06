using EBOS.CRM.Application.Features.CRM.Quote.Commands.AddQuote;
using EBOS.CRM.Contracts.Requests.CRM.Quote;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MapsterMapper;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Quote.Commands.AddQuote;

public class AddQuoteCommandHandlerTest
{
    [Fact]
    public async Task Handle_ValidRequest_PersistsAndReturnsResponse()
    {
        var repository = new Mock<IQuoteRepository>();
        var auditService = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var mapper = new Mock<IMapper>();
        var quoteOpportunityValidation = new Mock<IQuoteOpportunityValidationService>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        auditService.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        mapper.Setup(x => x.Map<global::EBOS.CRM.Domain.Entities.CRM.Quote>(It.IsAny<AddQuoteRequest>()))
            .Returns(new global::EBOS.CRM.Domain.Entities.CRM.Quote
            {
                Id = 1,
                TenantId = 1,
                OpportunityId = 10,
                Status = "Draft",
                SubtotalAmount = 100m,
                DiscountAmount = 10m,
                TotalAmount = 90m
            });
        mapper.Setup(x => x.Map<QuoteResponse>(It.IsAny<global::EBOS.CRM.Domain.Entities.CRM.Quote>()))
            .Returns(new QuoteResponse(1, 1, 10, "Draft", "Q-1", 100m, 10m, 90m, null, null, true));
        quoteOpportunityValidation.Setup(x => x.EnsureOpportunityAvailableAsync(1, 10, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new AddQuoteCommandHandler(repository.Object, auditService.Object, currentUser.Object, mapper.Object, quoteOpportunityValidation.Object);
        var result = await handler.Handle(new AddQuoteCommand(new AddQuoteRequest(1, 10, "Draft", "Q-1", 100m, 10m, 90m, null, null)), CancellationToken.None);

        Assert.NotNull(result);
        repository.Verify(x => x.AddAsync(It.IsAny<global::EBOS.CRM.Domain.Entities.CRM.Quote>(), It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenOpportunityResolutionTransient_ThrowsTransientDomainFailure()
    {
        var repository = new Mock<IQuoteRepository>();
        var auditService = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var mapper = new Mock<IMapper>();
        var quoteOpportunityValidation = new Mock<IQuoteOpportunityValidationService>();

        mapper.Setup(x => x.Map<global::EBOS.CRM.Domain.Entities.CRM.Quote>(It.IsAny<AddQuoteRequest>()))
            .Returns(new global::EBOS.CRM.Domain.Entities.CRM.Quote { Id = 1, TenantId = 1, OpportunityId = 10, Status = "Draft", SubtotalAmount = 100m, DiscountAmount = 10m, TotalAmount = 90m });
        quoteOpportunityValidation.Setup(x => x.EnsureOpportunityAvailableAsync(1, 10, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TransientDomainFailureException(
                "Transient failure while resolving quote opportunity.",
                "DOMAIN_TRANSIENT_QUOTE_OPPORTUNITY_RESOLUTION"));

        var handler = new AddQuoteCommandHandler(repository.Object, auditService.Object, currentUser.Object, mapper.Object, quoteOpportunityValidation.Object);

        var ex = await Assert.ThrowsAsync<TransientDomainFailureException>(() =>
            handler.Handle(new AddQuoteCommand(new AddQuoteRequest(1, 10, "Draft", "Q-1", 100m, 10m, 90m, null, null)), CancellationToken.None));

        Assert.Equal("DOMAIN_TRANSIENT_QUOTE_OPPORTUNITY_RESOLUTION", ex.Code);
    }
}
