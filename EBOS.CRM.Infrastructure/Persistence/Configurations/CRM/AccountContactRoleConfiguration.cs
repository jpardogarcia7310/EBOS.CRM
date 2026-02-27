using EBOS.CRM.Domain.Entities.CRM;

namespace EBOS.CRM.Infrastructure.Persistence.Configurations.CRM;

public class AccountContactRoleConfiguration : IEntityTypeConfiguration<AccountContactRole>
{
    public void Configure(EntityTypeBuilder<AccountContactRole> builder)
    {
        builder.ToTable("AccountContactRoles", "CRM");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.TenantId).IsRequired();
        builder.Property(x => x.AccountContactId).IsRequired();
        builder.Property(x => x.RoleCode)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(x => x.IsPrimary).IsRequired();
        builder.Property(x => x.ValidFrom).IsRequired();
        builder.Property(x => x.ValidTo);
        builder.Property(x => x.Erased).IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.AccountContactId })
            .HasDatabaseName("IX_AccountContactRole_TenantId_AccountContactId");

        builder.HasOne(x => x.AccountContact)
            .WithMany(c => c.Roles)
            .HasForeignKey(x => x.AccountContactId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
