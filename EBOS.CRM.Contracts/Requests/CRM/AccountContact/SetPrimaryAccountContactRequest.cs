namespace EBOS.CRM.Contracts.Requests.CRM.AccountContact;

public record SetPrimaryAccountContactRequest(
    long TenantId,
    bool IsPrimary
);
