namespace EBOS.CRM.Domain.Interfaces.Repositories.CRM.Models;

public sealed record CustomerDedupeCriteria(
    long TenantId,
    string? Email,
    string? Phone,
    string? TaxId,
    string? IdentificationNumber
);
