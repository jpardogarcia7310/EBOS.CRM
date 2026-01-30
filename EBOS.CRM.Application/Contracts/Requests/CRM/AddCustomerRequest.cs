namespace EBOS.CRM.Application.Contracts.Requests.CRM;

public sealed record AddCustomerRequest(
    string Code,
    string Email,
    string Phone,
    long StatusId);
