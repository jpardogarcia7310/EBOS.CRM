using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using EBOS.CRM.Infrastructure.Persistence;
using EBOS.CRM.Infrastructure.Services.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace EBOS.CRM.ApiTests.Infrastructure.Services.Audit;

public class AuditOutboxServiceTest
{
    [Fact]
    public async Task EnqueueAsync_WhenEnabled_PersistsMessage()
    {
        var db = BuildDb();
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient(new StubHandler()) { BaseAddress = new Uri("http://localhost/") });
        var metrics = new Mock<ICustomer360Metrics>();
        var opts = global::Microsoft.Extensions.Options.Options.Create(new AuditOutboxOptions { Enabled = true });

        var sut = new AuditOutboxService(db, factory.Object, opts, new Mock<ILogger<AuditOutboxService>>().Object, metrics.Object);

        await sut.EnqueueAsync("InsertAudit", BuildRequest(), null, CancellationToken.None);

        Assert.Single(db.AuditOutboxMessages);
        metrics.Verify(x => x.RecordAuditOutboxEnqueue("InsertAudit"), Times.Once);
    }

    [Fact]
    public async Task DispatchPendingAsync_WhenSuccess_MarksProcessed()
    {
        var db = BuildDb();
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient(new StubHandler(HttpStatusCode.OK, "{\"success\":true,\"id\":1}")) { BaseAddress = new Uri("http://localhost/") });
        var metrics = new Mock<ICustomer360Metrics>();
        var opts = global::Microsoft.Extensions.Options.Options.Create(new AuditOutboxOptions { Enabled = true });

                var sut = new AuditOutboxService(db, factory.Object, opts, new Mock<ILogger<AuditOutboxService>>().Object, metrics.Object);

        await sut.EnqueueAsync("InsertAudit", BuildRequest(), null, CancellationToken.None);

        var sent = await sut.DispatchPendingAsync(CancellationToken.None);

        Assert.Equal(1, sent);
        Assert.NotNull(db.AuditOutboxMessages.Single().ProcessedAt);
        metrics.Verify(x => x.RecordAuditOutboxDispatch("InsertAudit", true), Times.Once);
    }

    private static CrmDbContext BuildDb()
    {
        var options = new DbContextOptionsBuilder<CrmDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;
        return new CrmDbContext(options);
    }

    private static AuditInsertRequest BuildRequest() => new(1, DateTimeOffset.UtcNow, "Add", "Entity", 1, null, "{}", "corr");

    private sealed class StubHandler(HttpStatusCode status = HttpStatusCode.OK, string? body = null) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(status)
            {
                Content = new StringContent(body ?? string.Empty, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        }
    }
}



