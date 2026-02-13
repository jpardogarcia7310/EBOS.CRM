namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM.Models;

public sealed record CustomerDuplicateCandidate(
    long CustomerId,
    string MatchReason,
    int Score
);
