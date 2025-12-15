using EBOS.CRM.Application.Features.Statuses.Dtos;
using EBOS.CRM.Domain.Entities;
using Mapster;

namespace EBOS.CRM.Application.Mappings;

public sealed class MappingStatus : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Status, StatusResponseDto>();

        config.NewConfig<StatusResponseDto, Status>();
    }
}