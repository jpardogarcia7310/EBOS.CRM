namespace EBOS.CRM.Contracts.Responses.EBOS;

public record ChannelTypeResponse(
    long Id,
    string Descripcion,
    bool IsActive
);
