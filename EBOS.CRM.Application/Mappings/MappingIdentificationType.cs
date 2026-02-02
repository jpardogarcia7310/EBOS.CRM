using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Domain.Entities;
using Mapster;

namespace EBOS.CRM.Application.Mappings;

public sealed class MappingIdentificationType : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<IdentificationType, IdentificationTypeResponse>()
              .Map(dest => dest.Id, src => src.Id)
              .Map(dest => dest.Code, src => src.Code)
              .Map(dest => dest.Description, src => src.Description);

        config.NewConfig<IdentificationTypeResponse, IdentificationType>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.Description, src => src.Description)
            .Ignore(dest => dest.Erased);
    }
}
