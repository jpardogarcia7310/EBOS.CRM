using EBOS.CRM.Application.Shared.Observability;
using EBOS.CRM.Domain.Events;
using EBOS.CRM.Domain.Interfaces.Services;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Shared.Observability;

public class DomainOperationalEventPublisherTests
{
    [Fact]
    public async Task PublishAsync_WhenEventsPresent_WritesAuditPerEvent()
    {
        var audit = new Mock<IAuditService>();
        audit.Setup(x => x.InsertAuditAsync(It.IsAny<global::EBOS.CRM.Contracts.Requests.Services.AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new global::EBOS.CRM.Contracts.Responses.Services.AuditInsertResponse(true, 1));

        var currentUser = new Mock<ICurrentUserContext>();
        currentUser.SetupGet(x => x.UserId).Returns(77);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-evt");

        var sut = new DomainOperationalEventPublisher(audit.Object, currentUser.Object);
        var events = new[]
        {
            new DomainOperationalEvent("CustomerPrivacyRequestRegistered", DomainOperationalEventCategory.Business, DateTime.UtcNow, new Dictionary<string, string>()),
            new DomainOperationalEvent("DomainCommandDeduplicated", DomainOperationalEventCategory.Technical, DateTime.UtcNow, new Dictionary<string, string>())
        };

        await sut.PublishAsync("CustomerPrivacyRequest", 11, events, CancellationToken.None);

        audit.Verify(x => x.InsertAuditAsync(It.IsAny<global::EBOS.CRM.Contracts.Requests.Services.AuditInsertRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}
