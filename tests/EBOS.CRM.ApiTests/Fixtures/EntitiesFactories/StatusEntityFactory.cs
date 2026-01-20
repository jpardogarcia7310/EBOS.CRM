using EBOS.CRM.Domain.Entities;

namespace EBOS.CRM.ApiTests.Fixtures.EntitiesFactories;

public static class StatusEntityFactory
{
    public static Status CreateValidCountry(string description = "Activo")
    {
        return new Status
        {
            Description = description
        };
    }
}