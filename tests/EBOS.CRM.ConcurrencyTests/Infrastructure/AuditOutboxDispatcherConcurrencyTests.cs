using System.Reflection;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Infrastructure.Services.Audit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.ConcurrencyTests.Infrastructure;

public class AuditOutboxDispatcherConcurrencyTests
{
    [Fact]
    public async Task Dispatcher_WhenTransientFailuresUnderLoad_ContinuesIterationWithoutCrashing()
    {
        var outbox = new ConcurrentOutboxService(transientFailures: 2, delayMs: 50);
        var tasks = Enumerable.Range(0, 6)
            .Select(_ => RunSingleDispatcherAsync(outbox, cancelAfterMs: 300))
            .ToArray();

        await Task.WhenAll(tasks);

        Assert.True(outbox.Calls >= 4);
        Assert.True(outbox.Failures >= 1);
        Assert.True(outbox.Calls >= outbox.Failures + outbox.Successes);
    }

    [Fact]
    public async Task Dispatcher_WhenCanceled_StopsAfterAtLeastOneDispatchCall()
    {
        var outbox = new ConcurrentOutboxService(transientFailures: 0, delayMs: 20);
        await RunSingleDispatcherAsync(outbox, cancelAfterMs: 250);

        Assert.True(outbox.Calls >= 1);
    }

    private static async Task RunSingleDispatcherAsync(IAuditOutboxService outbox, int cancelAfterMs)
    {
        using var provider = BuildScopeFactory(outbox);
        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
        var dispatcher = new AuditOutboxDispatcher(
            scopeFactory,
            NullLogger<AuditOutboxDispatcher>.Instance,
            Options.Create(new AuditOutboxOptions { DispatchIntervalSeconds = 0 }));

        using var cts = new CancellationTokenSource(cancelAfterMs);
        var executeMethod = typeof(AuditOutboxDispatcher)
            .GetMethod("ExecuteAsync", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(executeMethod);

        var task = (Task?)executeMethod!.Invoke(dispatcher, [cts.Token]);
        Assert.NotNull(task);

        try
        {
            await task!;
        }
        catch (OperationCanceledException)
        {
            // Expected cancellation path.
        }
    }

    private static ServiceProvider BuildScopeFactory(IAuditOutboxService outbox)
    {
        var services = new ServiceCollection();
        services.AddScoped(_ => outbox);
        return services.BuildServiceProvider();
    }

    private sealed class ConcurrentOutboxService(int transientFailures, int delayMs) : IAuditOutboxService
    {
        private int _remainingFailures = transientFailures;
        public int Calls => _calls;
        public int Failures => _failures;
        public int Successes => _successes;

        private int _calls;
        private int _failures;
        private int _successes;

        public Task EnqueueAsync(string operation, Contracts.Requests.Services.AuditInsertRequest request, string? error,
            CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public async Task<int> DispatchPendingAsync(CancellationToken cancellationToken = default)
        {
            Interlocked.Increment(ref _calls);
            await Task.Delay(delayMs, cancellationToken);

            if (Interlocked.Decrement(ref _remainingFailures) >= 0)
            {
                Interlocked.Increment(ref _failures);
                throw new InvalidOperationException("Transient dispatcher failure.");
            }

            Interlocked.Increment(ref _successes);
            return 1;
        }
    }
}
