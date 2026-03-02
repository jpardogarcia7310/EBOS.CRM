using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class AccountHierarchyConfiguration : IEntityTypeConfiguration<AccountHierarchy>
{
    public void Configure(EntityTypeBuilder<AccountHierarchy> builder)
    {
        builder.ToTable("AccountHierarchies", "CRM", c =>
        {
            c.HasCheckConstraint(
                "CK_AccountHierarchy_Parent_Child_Different",
                "[ParentCorporateCustomerId] <> [ChildCorporateCustomerId]");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.TenantId).IsRequired();
        builder.Property(x => x.ParentCorporateCustomerId).IsRequired();
        builder.Property(x => x.ChildCorporateCustomerId).IsRequired();
        builder.Property(x => x.RelationType)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(x => x.ValidFrom).IsRequired();
        builder.Property(x => x.ValidTo);
        builder.Property(x => x.IsCurrent).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        builder.Property(x => x.Erased).IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.ParentCorporateCustomerId })
            .HasDatabaseName("IX_AccountHierarchy_TenantId_ParentCorporateCustomerId");
        builder.HasIndex(x => new { x.TenantId, x.ChildCorporateCustomerId })
            .HasDatabaseName("IX_AccountHierarchy_TenantId_ChildCorporateCustomerId");
        builder.HasIndex(x => new
            { x.TenantId, x.ParentCorporateCustomerId, x.ChildCorporateCustomerId, x.RelationType })
            .IsUnique()
            .HasFilter("[IsCurrent] = 1 AND [Erased] = 0")
            .HasDatabaseName("UX_AccountHierarchy_Tenant_Parent_Child_Relation_Current");

        builder.HasOne(x => x.ParentCorporateCustomer)
            .WithMany(c => c.ParentRelationships)
            .HasForeignKey(x => x.ParentCorporateCustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ChildCorporateCustomer)
            .WithMany(c => c.ChildRelationships)
            .HasForeignKey(x => x.ChildCorporateCustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}


