using EBOS.CRM.Application.Features.Statuses.Dtos;
using EBOS.CRM.Domain.Entities;
using Mapster;

namespace EBOS.CRM.Application.Mappings;

public sealed class MappingEstado : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Estado, EstadoResponseDto>()
              .Map(dest => dest.Id, src => src.Id)
              .Map(dest => dest.Description, src => src.Description);

        config.NewConfig<EstadoResponseDto, Estado>()
              .Map(dest => dest.Id, src => src.Id)
              .Map(dest => dest.Description, src => src.Description)
              .Ignore(dest => dest.Clientes);
    }
}