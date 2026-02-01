using System.Linq.Expressions;
using System.Reflection;
using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Entities.CRM;
using Microsoft.EntityFrameworkCore;

namespace EBOS.CRM.Infrastructure.Persistence;

public class CrmDbContext(DbContextOptions<CrmDbContext> options) : DbContext(options)
{
    // DbSets
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CorporateCustomer> CorporateCustomers => Set<CorporateCustomer>();
    public DbSet<IndividualCustomer> IndividualCustomers => Set<IndividualCustomer>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<BranchOffice> BranchOffices => Set<BranchOffice>();
    public DbSet<TaxInformation> TaxInformation => Set<TaxInformation>();
    public DbSet<BankInformation> BankInformation => Set<BankInformation>();
    public DbSet<CreditAccount> CreditAccounts => Set<CreditAccount>();
    public DbSet<CreditTransaction> CreditTransactions => Set<CreditTransaction>();
    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<IdentificationType> IdentificationTypes => Set<IdentificationType>();
    public DbSet<AddressType> AddressTypes => Set<AddressType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Load all entity configurations.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CrmDbContext).Assembly);

        ApplySoftDeleteQueryFilter(modelBuilder);

        // SAFETY NET: Force DeleteBehavior.Restrict on any unconfigured FK
        _ = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Select(fk => fk.DeleteBehavior = DeleteBehavior.Restrict)
            .ToList();

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // We do not overwrite options if they are already configured by the host/DI
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
            .Where(e => softErasableInterface.IsAssignableFrom(e.ClrType))
            .Where(e => e.BaseType == null)
            .ToList();

        foreach (var entityType in entityTypes)
        {
            var clrType = entityType.ClrType;
            var parameter = Expression.Parameter(clrType, "e");

            var convertedParam = Expression.Convert(parameter, typeof(object));
            var erasedProperty = Expression.Call(
                erasedPropertyMethod,
                convertedParam,
                Expression.Constant(nameof(ISoftDeletable.Erased)));
            var compare = Expression.Equal(erasedProperty, Expression.Constant(false));
            var lambda = Expression.Lambda(compare, parameter);

            modelBuilder.Entity(clrType).HasQueryFilter(lambda);
        }
    }
}