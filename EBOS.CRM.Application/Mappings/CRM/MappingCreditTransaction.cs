using EBOS.CRM.Application.Contracts.Requests.CRM.CreditTransaction;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using Mapster;

namespace EBOS.CRM.Application.Mappings.CRM;

public class MappingCreditTransaction : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreditTransaction, CreditTransactionResponse>()
            .Map(dest => dest.Active, src => !src.Erased);

        config.NewConfig<AddCreditTransactionRequest, CreditTransaction>()
            .Map(dest => dest.Date, src => src.Date)
            .Map(dest => dest.Amount, src => src.Amount)
            .Map(dest => dest.Type, src => src.Type)
            .Map(dest => dest.ExternalReference, src => src.ExternalReference)
            .Map(dest => dest.Comments, src => src.Comments)
            .Map(dest => dest.CreditAccountId, src => src.CreditAccountId)
            .Map(dest => dest.Erased, _ => false)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreditAccount);

        config.NewConfig<UpdateCreditTransactionRequest, CreditTransaction>()
            .Map(dest => dest.Date, src => src.Date)
            .Map(dest => dest.Amount, src => src.Amount)
            .Map(dest => dest.Type, src => src.Type)
            .Map(dest => dest.ExternalReference, src => src.ExternalReference)
            .Map(dest => dest.Comments, src => src.Comments)
            .Map(dest => dest.CreditAccountId, src => src.CreditAccountId)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Erased)
            .Ignore(dest => dest.CreditAccount);
    }
}
