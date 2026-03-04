using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Application.Options;

namespace EBOS.CRM.Application.Features.CRM.CustomerMerge;

public class CustomerMergeFieldResolver(ICurrentUserContext currentUser, CustomerMergeOptions options)
{
    private readonly CustomerMergeOptions _options = options;
    private readonly long _currentUserId = currentUser.UserId;
    private readonly Dictionary<string, int> _sourcePriority = new(options.SourcePriority, StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _channelPriority = new(options.ChannelPriority, StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _confidentialityPriority = new(options.ConfidentialityPriority, StringComparer.OrdinalIgnoreCase);

    public string? ResolveString(string? winnerValue, DateTime? winnerUpdatedAt,
        string? candidateValue, DateTime? candidateUpdatedAt, bool preferWinner)
    {
        return ResolveString(winnerValue, winnerUpdatedAt, candidateValue, candidateUpdatedAt, preferWinner, null, null);
    }

    public string? ResolveString(string? winnerValue, DateTime? winnerUpdatedAt,
        string? candidateValue, DateTime? candidateUpdatedAt, bool preferWinner,
        CustomerMergeFieldContext? winnerContext, CustomerMergeFieldContext? candidateContext)
    {
        if (string.IsNullOrWhiteSpace(candidateValue))
        {
            return winnerValue;
        }

        if (string.IsNullOrWhiteSpace(winnerValue))
        {
            return Trim(candidateValue);
        }

        var winnerTime = winnerUpdatedAt ?? DateTime.MinValue;
        var candidateTime = candidateUpdatedAt ?? DateTime.MinValue;

        if (winnerContext is null && candidateContext is null)
        {
            if (candidateTime > winnerTime)
            {
                return Trim(candidateValue);
            }

            if (candidateTime == winnerTime && !preferWinner)
            {
                return Trim(candidateValue);
            }

            return winnerValue;
        }

        var winnerScore = ComputeScore(winnerTime, winnerContext, isWinner: true);
        var candidateScore = ComputeScore(candidateTime, candidateContext, isWinner: false);

        if (candidateScore > winnerScore)
        {
            return Trim(candidateValue);
        }

        if (candidateScore == winnerScore && !preferWinner)
        {
            return Trim(candidateValue);
        }

        return winnerValue;
    }

    public long ResolveLong(long winnerValue, DateTime? winnerUpdatedAt,
        long candidateValue, DateTime? candidateUpdatedAt, bool preferWinner)
    {
        return ResolveLong(winnerValue, winnerUpdatedAt, candidateValue, candidateUpdatedAt, preferWinner, null, null);
    }

    public long ResolveLong(long winnerValue, DateTime? winnerUpdatedAt,
        long candidateValue, DateTime? candidateUpdatedAt, bool preferWinner,
        CustomerMergeFieldContext? winnerContext, CustomerMergeFieldContext? candidateContext)
    {
        if (candidateValue <= 0)
        {
            return winnerValue;
        }

        if (winnerValue <= 0)
        {
            return candidateValue;
        }

        var winnerTime = winnerUpdatedAt ?? DateTime.MinValue;
        var candidateTime = candidateUpdatedAt ?? DateTime.MinValue;

        if (winnerContext is null && candidateContext is null)
        {
            if (candidateTime > winnerTime)
            {
                return candidateValue;
            }

            if (candidateTime == winnerTime && !preferWinner)
            {
                return candidateValue;
            }

            return winnerValue;
        }

        var winnerScore = ComputeScore(winnerTime, winnerContext, isWinner: true);
        var candidateScore = ComputeScore(candidateTime, candidateContext, isWinner: false);

        if (candidateScore > winnerScore)
        {
            return candidateValue;
        }

        if (candidateScore == winnerScore && !preferWinner)
        {
            return candidateValue;
        }

        return winnerValue;
    }

    public DateTime ResolveDate(DateTime winnerValue, DateTime? winnerUpdatedAt,
        DateTime candidateValue, DateTime? candidateUpdatedAt, bool preferWinner)
    {
        return ResolveDate(winnerValue, winnerUpdatedAt, candidateValue, candidateUpdatedAt, preferWinner, null, null);
    }

    public DateTime ResolveDate(DateTime winnerValue, DateTime? winnerUpdatedAt,
        DateTime candidateValue, DateTime? candidateUpdatedAt, bool preferWinner,
        CustomerMergeFieldContext? winnerContext, CustomerMergeFieldContext? candidateContext)
    {
        if (candidateValue == default)
        {
            return winnerValue;
        }

        if (winnerValue == default)
        {
            return candidateValue;
        }

        var winnerTime = winnerUpdatedAt ?? DateTime.MinValue;
        var candidateTime = candidateUpdatedAt ?? DateTime.MinValue;

        if (winnerContext is null && candidateContext is null)
        {
            if (candidateTime > winnerTime)
            {
                return candidateValue;
            }

            if (candidateTime == winnerTime && !preferWinner)
            {
                return candidateValue;
            }

            return winnerValue;
        }

        var winnerScore = ComputeScore(winnerTime, winnerContext, isWinner: true);
        var candidateScore = ComputeScore(candidateTime, candidateContext, isWinner: false);

        if (candidateScore > winnerScore)
        {
            return candidateValue;
        }

        if (candidateScore == winnerScore && !preferWinner)
        {
            return candidateValue;
        }

        return winnerValue;
    }

    public DateTime ResolveUpdatedAt(DateTime? winnerUpdatedAt, DateTime? candidateUpdatedAt)
    {
        var winnerTime = winnerUpdatedAt ?? DateTime.MinValue;
        var candidateTime = candidateUpdatedAt ?? DateTime.MinValue;
        return candidateTime > winnerTime ? candidateUpdatedAt ?? DateTime.UtcNow : winnerUpdatedAt ?? DateTime.UtcNow;
    }

    public long ResolveUpdatedBy(long? winnerUpdatedBy)
    {
        return winnerUpdatedBy.GetValueOrDefault(_currentUserId);
    }

    private string Trim(string value)
    {
        if (value.Length <= _options.MaxFieldLength)
        {
            return value;
        }

        return value[.._options.MaxFieldLength];
    }

    private long ComputeScore(DateTime updatedAt, CustomerMergeFieldContext? context, bool isWinner)
    {
        var score = 0L;
        var minutes = (long)(updatedAt - DateTime.UnixEpoch).TotalMinutes;
        score += minutes * _options.UpdatedAtWeight;

        if (context is not null)
        {
            var sourceRank = GetPriority(_sourcePriority, context.Source);
            var channelRank = GetPriority(_channelPriority, context.ChannelKey);
            var confidentialityRank = GetPriority(_confidentialityPriority, context.Confidentiality);

            if (_options.BlockLowerConfidentiality && confidentialityRank < _options.MinimumConfidentialityRank)
            {
                return long.MinValue / 2;
            }

            score += sourceRank * _options.SourceWeight;
            score += channelRank * _options.ChannelWeight;
            score += confidentialityRank * _options.ConfidentialityWeight;
        }

        if (isWinner)
        {
            score += _options.WinnerBoostScore;
        }

        return score;
    }

    private static int GetPriority(Dictionary<string, int> map, string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return 0;
        }

        return map.TryGetValue(key, out var value) ? value : 0;
    }
}
