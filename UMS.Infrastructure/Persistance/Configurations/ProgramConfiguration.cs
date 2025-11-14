using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UMS.Core.Entities.Academic;

namespace UMS.Infrastructure.Persistence.Configurations
{
    public class ProgramConfiguration : BaseEntityConfiguration<Program>
    {
        public override void Configure(EntityTypeBuilder<Program> builder)
        {
            base.Configure(builder);

            builder.ToTable("Programs");

            builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Code).IsRequired().HasMaxLength(20);
            builder.Property(e => e.Description).HasMaxLength(1000);

            builder.HasIndex(e => e.Code).IsUnique();
        }
    }
}