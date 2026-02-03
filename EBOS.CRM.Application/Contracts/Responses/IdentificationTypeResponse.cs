namespace EBOS.CRM.Application.Contracts.Responses;


public record IdentificationTypeResponse(
    long Id,
    string Code,
    string Description
);