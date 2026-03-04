namespace EBOS.CRM.StressTests.Infrastructure;

internal static class StressStatistics
{
    public static double Percentile(IReadOnlyList<double> values, double percentile)
    {
        if (values.Count == 0)
        {
            return 0d;
        }

        var sorted = values.OrderBy(x => x).ToArray();
        var index = (int)Math.Ceiling((percentile / 100d) * sorted.Length) - 1;
        index = Math.Clamp(index, 0, sorted.Length - 1);
        return sorted[index];
    }

    public static double ThroughputPerSecond(long operations, TimeSpan elapsed)
    {
        if (elapsed.TotalSeconds <= 0)
        {
            return operations;
        }

        return operations / elapsed.TotalSeconds;
    }
}

