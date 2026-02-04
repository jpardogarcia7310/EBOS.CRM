namespace EBOS.CRM.Application.Contracts.Responses.Security;

public sealed record AuthenticatedUserResponse(
    long UserId,
    string ExternalId,
    string Username,
    string Email,
    string DisplayName,
    bool IsActive,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions
);
