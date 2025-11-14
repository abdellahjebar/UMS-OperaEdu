using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UMS.Core.Entities.Academic;

namespace UMS.Infrastructure.Persistence.Configurations
{
    public class SectionConfiguration : BaseEntityConfiguration<Section>
    {
        public override void Configure(EntityTypeBuilder<Section> builder)
        {
            base.Configure(builder);

            builder.ToTable("Sections");

            builder.Property(e => e.SectionNumber).IsRequired().HasMaxLength(10);

            builder.HasIndex(e => new { e.CourseId, e.SectionNumber, e.Term, e.Year }).IsUnique();
        }
    }
}