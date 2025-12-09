using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace EBOS.CRM.Infrastructure.Persistence;

public class CrmDbContext(DbContextOptions<CrmDbContext> options) : DbContext(options)
{
    public DbSet<Country> Countries { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuración básica para Country
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CrmDbContext).Assembly);

        AplySoftDeleteQueryFilter(modelBuilder);
        //// SAFETY NET: fuerza DeleteBehavior.Restrict en cualquier FK no configurada
        _ = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Select(fk => fk.DeleteBehavior = DeleteBehavior.Restrict)
            .ToList();

        base.OnModelCreating(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        optionsBuilder.EnableSensitiveDataLogging();
    }
    private static void AplySoftDeleteQueryFilter(ModelBuilder modelBuilder)
    {
        var softErasableInterface = typeof(ISoftDeletable);
        var erasedPropertyMethod = typeof(EF)
            .GetMethod(nameof(EF.Property), BindingFlags.Static | BindingFlags.Public)?
            .MakeGenericMethod(typeof(bool));

        if (erasedPropertyMethod == null)
            return;
        foreach (var clrType in modelBuilder.Model.GetEntityTypes().Select(e => e.ClrType).Where(clrType => softErasableInterface.IsAssignableFrom(clrType)))
        {
            var parameter = Expression.Parameter(clrType, "e");
            var erasedProperty = Expression.Call(erasedPropertyMethod, parameter, Expression.Constant(nameof(ISoftDeletable.Erased))
            );
            var compare = Expression.Equal(erasedProperty, Expression.Constant(false));
            var lambda = Expression.Lambda(compare, parameter);

            modelBuilder.Entity(clrType).HasQueryFilter(lambda);
        }
    }
}