using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Domain.Entities;
using Mapster;

namespace EBOS.CRM.Application.Mappings;

public sealed class MappingStatus : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Status, StatusResponse>()
              .Map(dest => dest.Id, src => src.Id)
              .Map(dest => dest.Description, src => src.Description);

        config.NewConfig<StatusResponse, Status>()
              .Map(dest => dest.Id, src => src.Id)
              .Map(dest => dest.Description, src => src.Description)
              .Ignore(dest => dest.Customers);
    }
}