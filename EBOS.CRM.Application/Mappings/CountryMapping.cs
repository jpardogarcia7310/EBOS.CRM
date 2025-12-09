using AutoMapper;
using EBOS.CRM.Application.Features.Countries.Dtos;
using EBOS.CRM.Domain.Entities;

namespace EBOS.CRM.Application.Mappings;

public class CountryMapping : Profile
{
    public CountryMapping()
    {
        CreateMap<Country, CountryDto>().ReverseMap();
    }
}