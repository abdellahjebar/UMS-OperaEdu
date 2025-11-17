using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UMS.Core.Entities.Tenants;

namespace UMS.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Entity Framework configuration for SuperAdmin entity
    /// </summary>
    public class SuperAdminConfiguration : IEntityTypeConfiguration<SuperAdmin>
    {
        public void Configure(EntityTypeBuilder<SuperAdmin> builder)
        {
            builder.ToTable("SuperAdmins");

            // Primary key
            builder.HasKey(x => x.Id);

            // Email - Required, unique, indexed
            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.Email)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0"); // Only enforce uniqueness on non-deleted records

            // PasswordHash - Required
            builder.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            // FullName - Required
            builder.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(200);

            // PhoneNumber - Optional
            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(20);

            // IsActive - Required, default true
            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // LastLoginAt - Optional
            builder.Property(x => x.LastLoginAt)
                .IsRequired(false);

            // EmailConfirmed - Required, default true
            builder.Property(x => x.EmailConfirmed)
                .IsRequired()
                .HasDefaultValue(true);

            // Notes - Optional
            builder.Property(x => x.Notes)
                .HasMaxLength(1000);

            // IsDeleted - Soft delete (from BaseEntity)
            builder.Property(x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(x => x.IsDeleted);

            // CreatedAt index for sorting
            builder.HasIndex(x => x.CreatedAt);

            // IsActive index for filtering
            builder.HasIndex(x => x.IsActive);
        }
    }
}
