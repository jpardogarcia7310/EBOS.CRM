namespace EBOS.CRM.Application.Contracts.Requests.Security;

public sealed record AuthenticateUserRequest(
    string ExternalId,
    string Username,
    string Email,
    string DisplayName,
    bool IsActive);
