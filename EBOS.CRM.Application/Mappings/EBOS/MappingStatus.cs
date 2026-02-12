using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Entities.EBOS;
using Mapster;

namespace EBOS.CRM.Application.Mappings.EBOS;

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
              .Ignore(dest => dest.CreatedAt)
              .Ignore(dest => dest.CreatedBy)
              .Ignore(dest => dest.UpdatedAt!)
              .Ignore(dest => dest.UpdatedBy!)
              .Ignore(dest => dest.Customers);
    }
}

