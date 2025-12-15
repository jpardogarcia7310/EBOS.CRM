using EBOS.CRM.Application.Features.TaxRegimes.Dtos;
using EBOS.CRM.Domain.Entities;
using Mapster;

namespace EBOS.CRM.Application.Mappings;

public sealed class MappingTaxRegime : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TaxRegime, TaxRegimeResponseDto>();

        config.NewConfig<TaxRegimeResponseDto, TaxRegime>();
    }
}