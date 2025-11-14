using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UMS.Core.Entities.Tenants;

namespace UMS.Infrastructure.Persistence.Configurations
{
    public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.ToTable("Tenants");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Subdomain)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.ConnectionString)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(t => t.AdminEmail)
                .HasMaxLength(100);

            builder.Property(t => t.AdminPhone)
                .HasMaxLength(20);

            builder.Property(t => t.Address)
                .HasMaxLength(500);

            builder.Property(t => t.LogoUrl)
                .HasMaxLength(500);

            builder.Property(t => t.AnnualFee)
                .HasPrecision(18, 2);

            // Indexes
            builder.HasIndex(t => t.Subdomain)
                .IsUnique();

            builder.HasIndex(t => t.IsActive);
            builder.HasIndex(t => t.CreatedAt);
        }
    }
}
