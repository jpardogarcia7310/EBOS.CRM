using System;

namespace EBOS.CRM.Application.Contracts.Requests.CRM.CorporateCustomer;

public record UpdateCorporateCustomerRequest(
    long TenantId,
    string Code,
    string Email,
    string Phone,
    long StatusId,
    string LegalName,
    string TaxIdentification
);
