namespace EBOS.CRM.ApiTests.Fixtures;

public static class CancellationTokenFixture
{
    public static CancellationToken CancelledToken()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();
        return cts.Token;
    }

    public static CancellationToken ActiveToken() => CancellationToken.None;
}
