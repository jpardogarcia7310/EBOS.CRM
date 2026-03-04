namespace EBOS.CRM.Application.Options;

public sealed class CustomerMergeOptions
{
    public const string SectionName = "CustomerMerge";

    public int WinnerBoostScore { get; init; } = 1000;
    public bool PreferWinnerOnTie { get; init; } = true;
    public int MaxFieldLength { get; init; } = 200;
    public int UpdatedAtWeight { get; init; } = 1;
    public int SourceWeight { get; init; } = 100;
    public int ChannelWeight { get; init; } = 50;
    public int ConfidentialityWeight { get; init; } = 80;
    public bool BlockLowerConfidentiality { get; init; } = false;
    public int MinimumConfidentialityRank { get; init; } = 0;
    public Dictionary<string, int> SourcePriority { get; init; } = new();
    public Dictionary<string, int> ChannelPriority { get; init; } = new();
    public Dictionary<string, int> ConfidentialityPriority { get; init; } = new();
    public Dictionary<string, string> FieldChannelMap { get; init; } = new();
}
