using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace EBOS.CRM.Infrastructure.Persistence;

public class CrmDbContext(DbContextOptions<CrmDbContext> options) : DbContext(options)
{
    // DbSets
    public DbSet<Customer> Clients { get; set; } = default!;
    public DbSet<TaxRegime> TaxRegimes { get; set; } = default!;
    public DbSet<Country> Countries { get; set; } = default!;
    public DbSet<TaxAddress> FiscalAddresses { get; set; } = default!;
    public DbSet<ShippingAddress> ShippingAddresses { get; set; } = default!;
    public DbSet<SalesData> SalesConfigurations { get; set; } = default!;
    public DbSet<CustomerHistory> ClientHistories { get; set; } = default!;
    public DbSet<DocumentConfiguration> DocumentConfigurations { get; set; } = default!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Carga de todas las Configuraciones de las entidades.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CrmDbContext).Assembly);

        ApplySoftDeleteQueryFilter(modelBuilder);

        // SAFETY NET: fuerza DeleteBehavior.Restrict en cualquier FK no configurada
        _ = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Select(fk => fk.DeleteBehavior = DeleteBehavior.Restrict)
            .ToList();

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // No sobrescribimos opciones si ya están configuradas por el host/DI
        if (!optionsBuilder.IsConfigured)
        {
#if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
#endif
        }
    }

    private static void ApplySoftDeleteQueryFilter(ModelBuilder modelBuilder)
    {
        var softErasableInterface = typeof(ISoftDeletable);
        var erasedPropertyMethod = typeof(EF)
            .GetMethod(nameof(EF.Property), BindingFlags.Static | BindingFlags.Public)?
            .MakeGenericMethod(typeof(bool));

        if (erasedPropertyMethod == null)
            return;

        var entityTypes = modelBuilder.Model.GetEntityTypes()
            .Select(e => e.ClrType)
            .Where(clrType => clrType != null && softErasableInterface.IsAssignableFrom(clrType!))
            .ToList();

        foreach (var clrType in entityTypes!)
        {
            var parameter = Expression.Parameter(clrType!, "e");

            // EF.Property<bool>((object)e, "Erased")
            var convertedParam = Expression.Convert(parameter, typeof(object));
            var erasedProperty = Expression.Call(erasedPropertyMethod, convertedParam, Expression.Constant(nameof(ISoftDeletable.Erased)));
            var compare = Expression.Equal(erasedProperty, Expression.Constant(false));
            var lambda = Expression.Lambda(compare, parameter);

            modelBuilder.Entity(clrType!).HasQueryFilter(lambda);
        }
    }
}