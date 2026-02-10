using System;

namespace EBOS.CRM.Application.Contracts.Requests.CRM.IndividualCustomer;

public record AddIndividualCustomerRequest(
    long TenantId,
    string Code,
    string Email,
    string Phone,
    long StatusId,
    string FirstName,
    string LastName,
    DateTime BirthDate,
    string? IdentificationNumber,
    long IdentificationTypeId
);
