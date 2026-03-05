using System.Collections.Concurrent;
using EBOS.CRM.Domain.Exceptions;

namespace EBOS.CRM.ConcurrencyTests.Domain;

public class DomainConflictConcurrencyTests
{
    [Fact]
    public async Task VersionClash_ConcurrentCommands_ProducesRetryableDomainConflict()
    {
        var gate = new VersionedCommandGate(initialVersion: 1);
        var captured = new ConcurrentBag<DomainConflictException>();

        var t1 = Task.Run(() =>
        {
            try
            {
                gate.Execute("cmd-a", expectedVersion: 1);
            }
            catch (DomainConflictException ex)
            {
                captured.Add(ex);
            }
        });
        var t2 = Task.Run(() =>
        {
            try
            {
                gate.Execute("cmd-b", expectedVersion: 1);
            }
            catch (DomainConflictException ex)
            {
                captured.Add(ex);
            }
        });

        await Task.WhenAll(t1, t2);

        var mismatch = captured.Single(ex => ex.Code == "DOMAIN_CONFLICT_VERSION_MISMATCH");
        Assert.Equal(DomainErrorTaxonomyType.DomainConflict, mismatch.TaxonomyType);
        Assert.True(mismatch.Retryable);
    }

    [Fact]
    public async Task ReplayedCommand_ConcurrentExecution_ProducesNonRetryableDomainConflict()
    {
        var gate = new VersionedCommandGate(initialVersion: 1);
        DomainConflictException? captured = null;

        var t1 = Task.Run(() => gate.Execute("same-command", expectedVersion: 1));
        var t2 = Task.Run(() =>
        {
            try
            {
                gate.Execute("same-command", expectedVersion: 2);
            }
            catch (DomainConflictException ex)
            {
                captured = ex;
            }
        });

        await Task.WhenAll(t1, t2);

        Assert.NotNull(captured);
        Assert.Equal(DomainErrorTaxonomyType.DomainConflict, captured!.TaxonomyType);
        Assert.Equal("DOMAIN_CONFLICT_COMMAND_REPLAY", captured.Code);
        Assert.False(captured.Retryable);
    }

    private sealed class VersionedCommandGate
    {
        private readonly object _sync = new();
        private readonly ConcurrentDictionary<string, byte> _processed = new(StringComparer.Ordinal);
        private int _version;

        public VersionedCommandGate(int initialVersion)
        {
            _version = initialVersion;
        }

        public void Execute(string commandId, int expectedVersion)
        {
            lock (_sync)
            {
                if (_processed.ContainsKey(commandId))
                {
                    throw new DomainConflictException(
                        "Command was already processed.",
                        code: "DOMAIN_CONFLICT_COMMAND_REPLAY",
                        retryable: false);
                }

                if (expectedVersion != _version)
                {
                    throw new DomainConflictException(
                        "Version mismatch detected.",
                        code: "DOMAIN_CONFLICT_VERSION_MISMATCH",
                        retryable: true);
                }

                _processed[commandId] = 1;
                _version++;
            }
        }
    }
}
