using EBOS.CRM.Application.Contracts.Responses;
using EBOS.CRM.Domain.Entities;
using Mapster;

namespace EBOS.CRM.Application.Mappings;

public sealed class MappingAddressType : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AddressType, AddressTypeResponse>()
              .Map(dest => dest.Id, src => src.Id)
              .Map(dest => dest.Code, src => src.Code)
              .Map(dest => dest.Description, src => src.Description)
              .Map(dest => dest.Category, src => src.Category)
              .Map(dest => dest.AllowsMultiple, src => src.AllowsMultiple)
              .Map(dest => dest.RequiresPrimary, src => src.RequiresPrimary);

        config.NewConfig<AddressTypeResponse, AddressType>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Category, src => src.Category)
            .Map(dest => dest.AllowsMultiple, src => src.AllowsMultiple)
            .Map(dest => dest.RequiresPrimary, src => src.RequiresPrimary)
            .Ignore(dest => dest.Addresses);
    }
}

