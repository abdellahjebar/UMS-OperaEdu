using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UMS.Core.Entities.Academic;

namespace UMS.Infrastructure.Persistence.Configurations
{
    public class EnrollmentConfiguration : BaseEntityConfiguration<Enrollment>
    {
        public override void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            base.Configure(builder);

            builder.ToTable("Enrollments");

            builder.HasIndex(e => new { e.StudentId, e.SectionId }).IsUnique();
        }
    }
}