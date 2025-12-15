using EBOS.CRM.Application.Features.TaxRegimes.Dtos;
using EBOS.CRM.Domain.Entities;
using Mapster;

namespace EBOS.CRM.Application.Mappings;

public sealed class MappingTaxRegime : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TaxRegime, TaxRegimeResponseDto>()
              .Map(dest => dest.Id, src => src.Id)
              .Map(dest => dest.Description, src => src.Description);

        config.NewConfig<TaxRegimeResponseDto, TaxRegime>()
              .Map(dest => dest.Id, src => src.Id)
              .Map(dest => dest.Description, src => src.Description)
              .Ignore(dest => dest.Customers);
    }
}