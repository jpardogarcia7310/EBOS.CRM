using System.Linq.Expressions;
using System.Reflection;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Domain.Entities;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Entities.Identity;
using EBOS.CRM.Domain.Interfaces;
using EBOS.CRM.Domain.Services;
using EBOS.CRM.Infrastructure.Options;

namespace EBOS.CRM.Infrastructure.Persistence;

public class CrmDbContext(DbContextOptions<CrmDbContext> options, ITenantContext? tenantContext = null,
        IOptions<MultiTenantOptions>? multiTenantOptions = null)
    : DbContext(options)
{
    private readonly long _tenantId = tenantContext?.TenantId ?? 0;
    private readonly MultiTenantOptions _multiTenantOptions = multiTenantOptions?.Value ?? new MultiTenantOptions();
    public long CurrentTenantId => _tenantId;
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
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Policy> Policies => Set<Policy>();
    public DbSet<AbacAttribute> AbacAttributes => Set<AbacAttribute>();
    public DbSet<PolicyRule> PolicyRules => Set<PolicyRule>();
    public DbSet<PolicyRuleCondition> PolicyRuleConditions => Set<PolicyRuleCondition>();
    public DbSet<PolicyRole> PolicyRoles => Set<PolicyRole>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<PolicyPermission> PolicyPermissions => Set<PolicyPermission>();
    public DbSet<UserPolicy> UserPolicies => Set<UserPolicy>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Load all entity configurations.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CrmDbContext).Assembly);

        ApplySoftDeleteQueryFilter(modelBuilder);
        ApplyTenantQueryFilter(modelBuilder);
        ApplyTenantSchema(modelBuilder);

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

    private void ApplyTenantSchema(ModelBuilder modelBuilder)
    {
        if (_multiTenantOptions.Strategy != MultiTenantStrategy.Schema || _tenantId <= 0)
        {
            return;
        }

        var schemaName = $"{_multiTenantOptions.SchemaPrefix}{_tenantId}";
        var targets = new HashSet<string>(_multiTenantOptions.SchemaTargets, StringComparer.OrdinalIgnoreCase);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var schema = entityType.GetSchema();
            if (schema == null || !targets.Contains(schema))
            {
                continue;
            }

            entityType.SetSchema(schemaName);
        }
    }

    private void ApplyTenantQueryFilter(ModelBuilder modelBuilder)
    {
        var tenantPropertyMethod = typeof(EF)
            .GetMethod(nameof(EF.Property), BindingFlags.Static | BindingFlags.Public)?
            .MakeGenericMethod(typeof(long));

        if (tenantPropertyMethod == null)
        {
            return;
        }

        var entityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(e => e.FindProperty("TenantId") != null)
            .Where(e => e.BaseType == null)
            .ToList();

        foreach (var entityType in entityTypes)
        {
            var clrType = entityType.ClrType;
            var parameter = Expression.Parameter(clrType, "e");

            var convertedParam = Expression.Convert(parameter, typeof(object));
            var tenantProperty = Expression.Call(
                tenantPropertyMethod,
                convertedParam,
                Expression.Constant("TenantId"));
            var currentTenant = Expression.Property(Expression.Constant(this), nameof(CurrentTenantId));
            var compare = Expression.Equal(tenantProperty, currentTenant);
            var allowAllTenants = Expression.Equal(currentTenant, Expression.Constant(0L));
            var body = Expression.OrElse(allowAllTenants, compare);
            var lambda = Expression.Lambda(body, parameter);

            modelBuilder.Entity(clrType).HasQueryFilter(lambda);
        }
    }

    public override int SaveChanges()
    {
        EnforceTenantInvariant();
        ApplyTenantIdToAddedEntities();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        EnforceTenantInvariant();
        ApplyTenantIdToAddedEntities();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void EnforceTenantInvariant()
    {
        if (_tenantId <= 0)
        {
            return;
        }

        foreach (var entry in ChangeTracker.Entries()
                     .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
        {
            if (entry.Entity is not ITenantScopedEntity tenantScoped)
            {
                continue;
            }

            var tenantValue = tenantScoped.TenantId;
            if (tenantValue > 0 && tenantValue != _tenantId)
            {
                throw new InvalidOperationException(
                    $"TenantId mismatch for {entry.Metadata.ClrType.Name}: {tenantValue} != {_tenantId}.");
            }

            if (entry.State != EntityState.Added && tenantValue <= 0)
            {
                TenantInvariants.EnsureTenantAssigned(tenantScoped);
            }
        }
    }

    private void ApplyTenantIdToAddedEntities()
    {
        if (_tenantId <= 0)
        {
            return;
        }

        foreach (var entry in ChangeTracker.Entries()
                     .Where(e => e.State == EntityState.Added))
        {
            if (entry.Entity is not ITenantScopedEntity tenantScoped)
            {
                continue;
            }

            if (tenantScoped.TenantId > 0)
            {
                if (tenantScoped.TenantId != _tenantId)
                {
                    throw new InvalidOperationException(
                        $"TenantId mismatch for {entry.Metadata.ClrType.Name}: {tenantScoped.TenantId} != {_tenantId}.");
                }

                continue;
            }

            tenantScoped.TenantId = _tenantId;
        }
    }
}
