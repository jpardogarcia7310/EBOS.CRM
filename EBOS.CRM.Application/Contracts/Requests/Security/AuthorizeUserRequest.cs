namespace EBOS.CRM.Application.Contracts.Requests.Security;

public sealed record AuthorizeUserRequest(long UserId, string PolicyCode);
