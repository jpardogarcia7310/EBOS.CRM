using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Infrastructure.Persistence;

namespace EBOS.CRM.ApiTests.Fixtures;

public static class IntegrationTestStatusesDataSeeder
{
    public static void Seed(CrmDbContext context)
    {
        if (context.Statuses.Any())
            return;

        var statuses = new List<Status>
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

        context.Statuses.AddRange(statuses);
        context.SaveChanges();
    }
}