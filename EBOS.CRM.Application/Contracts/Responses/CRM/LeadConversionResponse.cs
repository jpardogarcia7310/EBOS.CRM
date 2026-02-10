namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public sealed record LeadConversionResponse(
    long LeadId,
    long? OpportunityId,
    string Status
);
