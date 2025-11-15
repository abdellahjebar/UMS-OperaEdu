using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UMS.Core.Entities.Academic;

namespace UMS.Infrastructure.Persistence.Configurations
{
    public class DepartmentConfiguration : BaseEntityConfiguration<Department>
    {
        public override void Configure(EntityTypeBuilder<Department> builder)
        {
            base.Configure(builder);

            builder.ToTable("Departments");

            builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Code).IsRequired().HasMaxLength(20);
            builder.Property(e => e.Description).HasMaxLength(1000);
            builder.Property(e => e.Building).HasMaxLength(100);
            builder.Property(e => e.Phone).HasMaxLength(20);
            builder.Property(e => e.Email).HasMaxLength(100);

            builder.HasIndex(e => e.Code).IsUnique();
            builder.HasIndex(e => e.Name);

            // Navigation properties
            builder.HasMany(e => e.Programs)
                .WithOne()
                .HasForeignKey(p => p.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Courses)
                .WithOne()
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
