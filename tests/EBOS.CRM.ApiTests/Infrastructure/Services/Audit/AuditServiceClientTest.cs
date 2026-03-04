using System.Net;
using System.Text;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Infrastructure.Services.Audit;
using Moq;

namespace EBOS.CRM.ApiTests.Infrastructure.Services.Audit;

public class AuditServiceClientTest
{
    [Fact]
    public async Task InsertAuditAsync_WhenDisabled_ReturnsSuccessWithoutHttp()
    {
        var outbox = new Mock<IAuditOutboxService>();
        var sut = new AuditServiceClient(new HttpClient(new StubHandler()),
            global::Microsoft.Extensions.Options.Options.Create(new AuditServiceOptions { Enabled = false }), outbox.Object);

        var result = await sut.InsertAuditAsync(BuildRequest(), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(0, result.Id);
        outbox.Verify(x => x.EnqueueAsync(It.IsAny<string>(), It.IsAny<AuditInsertRequest>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InsertAuditAsync_WhenHttpFails_EnqueuesOutbox()
    {
        var outbox = new Mock<IAuditOutboxService>();
        var sut = new AuditServiceClient(new HttpClient(new StubHandler(HttpStatusCode.InternalServerError, "boom")),
            global::Microsoft.Extensions.Options.Options.Create(new AuditServiceOptions { Enabled = true, RetryCount = 1 }), outbox.Object);

        var result = await sut.InsertAuditAsync(BuildRequest(), CancellationToken.None);

        Assert.True(result.Success);
        outbox.Verify(x => x.EnqueueAsync("InsertAudit", It.IsAny<AuditInsertRequest>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private static AuditInsertRequest BuildRequest() => new(1, DateTimeOffset.UtcNow, "Add", "Entity", 1, null, "{}", "corr");

    private sealed class StubHandler(HttpStatusCode status = HttpStatusCode.OK, string? body = null) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(status)
            {
                Content = new StringContent(body ?? "{\"success\":true,\"Id\":1}", Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        }
    }
}
