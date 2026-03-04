namespace EBOS.CRM.Contracts.Requests.CRM.AccountContact;

public record AddAccountContactRequest(
    long TenantId,
    long CorporateCustomerId,
    long IndividualCustomerId,
    bool IsPrimary,
    DateTime StartAt,
    DateTime? EndAt
);
