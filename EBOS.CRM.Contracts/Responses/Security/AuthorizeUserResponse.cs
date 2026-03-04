namespace EBOS.CRM.Contracts.Responses.Security;

public sealed record AuthorizeUserResponse(
    bool IsAuthorized
);
