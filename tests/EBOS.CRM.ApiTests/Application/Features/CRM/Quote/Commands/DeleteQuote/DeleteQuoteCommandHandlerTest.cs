using EBOS.CRM.Application.Features.CRM.Quote.Commands.DeleteQuote;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Quote.Commands.DeleteQuote;

public class DeleteQuoteCommandHandlerTest
{
    [Fact]
    public async Task Handle_WhenNotFound_ReturnsFalse()
    {
        var repository = new Mock<IQuoteRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        repository.Setup(x => x.GetByIdAsync(404, It.IsAny<CancellationToken>()))
            .ReturnsAsync((global::EBOS.CRM.Domain.Entities.CRM.Quote?)null);

        var handler = new DeleteQuoteCommandHandler(repository.Object, audit.Object, currentUser.Object);
        var result = await handler.Handle(new DeleteQuoteCommand(404), CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task Handle_WhenFound_DeletesAndAudits()
    {
        var repository = new Mock<IQuoteRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        audit.Setup(x => x.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var entity = new global::EBOS.CRM.Domain.Entities.CRM.Quote
        {
            Id = 1, TenantId = 1, OpportunityId = 10, Status = "Draft", SubtotalAmount = 100m, DiscountAmount = 10m, TotalAmount = 90m
        };
        repository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var handler = new DeleteQuoteCommandHandler(repository.Object, audit.Object, currentUser.Object);
        var result = await handler.Handle(new DeleteQuoteCommand(1), CancellationToken.None);

        Assert.True(result);
        repository.Verify(x => x.DeleteAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }
}
