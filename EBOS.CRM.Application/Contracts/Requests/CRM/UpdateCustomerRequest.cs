namespace EBOS.CRM.Application.Contracts.Requests.CRM;

public sealed record UpdateCustomerRequest(
    long Id,
    string Code,
    string Email,
    string Phone,
    long StatusId);
