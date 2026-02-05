using System;

namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public record IndividualCustomerResponse(
    long Id,
    string Code,
    string Email,
    string Phone,
    DateTime CreatedAt,
    long StatusId,
    string FirstName,
    string LastName,
    DateTime BirthDate,
    string? IdentificationNumber,
    long IdentificationTypeId,
    bool Active
);
