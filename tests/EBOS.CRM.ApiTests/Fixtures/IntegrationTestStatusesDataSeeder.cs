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
                    Description  = "Active"
                },
                new() {
                    Description  = "Defaulter"
                },
                new() {
                    Description  = "Suspended"
                }
            };

        context.Statuses.AddRange(statuses);
        context.SaveChanges();
    }
}
