using EBOS.CRM.Application.Contracts.Requests.CRM;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingBranchOffice : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<BranchOffice, BranchOfficeResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
            .Map(dest => dest.CorporateCustomerId, src => src.CorporateCustomerId)
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<AddBranchOfficeRequest, BranchOffice>()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
            .Map(dest => dest.CorporateCustomerId, src => src.CorporateCustomerId)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CorporateCustomer)
            .Ignore(dest => dest.BranchOfficeAddresses);

        config.NewConfig<UpdateBranchOfficeRequest, BranchOffice>()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
            .Map(dest => dest.CorporateCustomerId, src => src.CorporateCustomerId)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.CorporateCustomer)
            .Ignore(dest => dest.BranchOfficeAddresses);
    }
}
