using EBOS.CRM.Application.Contracts.Requests.CRM.CreditAccount;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using global::EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingCreditAccount : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreditAccount, CreditAccountResponse>()
            .Map(dest => dest.AvailableAmount, src => src.AvailableAmount)
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<AddCreditAccountRequest, CreditAccount>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.MaxAmount, src => src.MaxAmount)
            .Map(dest => dest.UsedAmount, src => src.UsedAmount)
            .Map(dest => dest.CustomerId, src => src.CustomerId)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Customer)
            .Ignore(dest => dest.CreditTransactions);

        config.NewConfig<UpdateCreditAccountRequest, CreditAccount>()
            .Map(dest => dest.TenantId, src => src.TenantId)
            .Map(dest => dest.MaxAmount, src => src.MaxAmount)
            .Map(dest => dest.UsedAmount, src => src.UsedAmount)
            .Map(dest => dest.CustomerId, src => src.CustomerId)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.UpdatedBy!)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.Customer)
            .Ignore(dest => dest.CreditTransactions);
    }
}


