using EBOS.CRM.Domain.Entities;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories;

public static class StatusEntityFactory
{
    public static Estado CreateValidCountry(string description = "Activo")
    {
        return new Estado
        {
            Description = description
        };
    }
}