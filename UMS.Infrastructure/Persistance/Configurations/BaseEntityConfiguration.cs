using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UMS.Core.Entities.Base;

namespace UMS.Infrastructure.Persistence.Configurations
{
    public class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.IsDeleted).HasDefaultValue(false);

            builder.HasQueryFilter(e => !e.IsDeleted);

            builder.HasIndex(e => e.CreatedAt);
            builder.HasIndex(e => e.IsDeleted);
        }
    }
}