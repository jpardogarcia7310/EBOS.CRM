using EBOS.CRM.Application.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Entities.EBOS;
using Mapster;

namespace EBOS.CRM.Application.Mappings.EBOS;

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
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Erased);
    }
}


