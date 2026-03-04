using EBOS.CRM.Contracts.Responses.EBOS;
using EBOS.CRM.Domain.Entities.EBOS;
using Mapster;

namespace EBOS.CRM.Application.Mappings.EBOS;

public class MappingValidationRule : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ValidationRule, ValidationRuleResponse>();
    }
}
