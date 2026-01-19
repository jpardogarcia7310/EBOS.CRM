using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Infrastructure.Persistence;

namespace EBOS.CRM.ApiTests.Fixtures;

public static class IntegrationTestStatusesDataSeeder
{
    public static void Seed(CrmDbContext context)
    {
        if (context.Estados.Any())
            return;

        var statuses = new List<Estado>
            {
                new() {
                    Description  = "Activo"
                },
                new() {
                    Description  = "Moroso"
                },
                new() {
                    Description  = "Suspendido"
                }
            };

        context.Estados.AddRange(statuses);
        context.SaveChanges();
    }
}