namespace EBOS.CRM.Contracts.Responses.CRM;

public record CustomerDuplicateCandidateResponse(
    long CustomerId,
    string MatchReason,
    int Score
);
