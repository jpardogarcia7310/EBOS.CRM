using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Infrastructure.Services.Audit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace EBOS.CRM.ApiTests.Infrastructure.Services.Audit;

public class AuditOutboxDispatcherTest
{
    [Fact]
    public async Task StartAsync_ExecutesDispatchIteration()
    {
        var outbox = new Mock<IAuditOutboxService>();
        var called = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        outbox.Setup(x => x.DispatchPendingAsync(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(ct =>
            {
                called.TrySetResult(true);
                return Task.FromResult(0);
            });

        var services = new ServiceCollection();
        services.AddSingleton(outbox.Object);
        var provider = services.BuildServiceProvider();

        var sut = new AuditOutboxDispatcher(
            provider.GetRequiredService<IServiceScopeFactory>(),
            new Mock<ILogger<AuditOutboxDispatcher>>().Object,
            global::Microsoft.Extensions.Options.Options.Create(new AuditOutboxOptions { DispatchIntervalSeconds = 1 }));

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        await sut.StartAsync(cts.Token);

        await Task.WhenAny(called.Task, Task.Delay(1500, CancellationToken.None));

        await sut.StopAsync(CancellationToken.None);

        Assert.True(called.Task.IsCompletedSuccessfully);
    }
}
