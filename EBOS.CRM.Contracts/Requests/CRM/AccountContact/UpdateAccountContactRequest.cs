namespace EBOS.CRM.Contracts.Requests.CRM.AccountContact;

public record UpdateAccountContactRequest(
    long TenantId,
    long CorporateCustomerId,
    long IndividualCustomerId,
    bool IsPrimary,
    DateTime StartAt,
    DateTime? EndAt
);
