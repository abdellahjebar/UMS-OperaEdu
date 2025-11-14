using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UMS.Core.Entities.Identity;

namespace UMS.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : BaseEntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.ToTable("Users");

            builder.Property(e => e.Email).IsRequired().HasMaxLength(100);
            builder.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
            builder.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            builder.Property(e => e.PhoneNumber).HasMaxLength(20);
            builder.Property(e => e.ProfileImageUrl).HasMaxLength(500);

            // Convert enum to string for database storage
            builder.Property(e => e.UserType)
                .HasConversion<string>()
                .IsRequired();

            builder.HasIndex(e => e.Email).IsUnique();

            builder.HasDiscriminator<string>("Discriminator")
                .HasValue<User>("User")
                .HasValue<Student>("Student")
                .HasValue<Faculty>("Faculty")
                .HasValue<Staff>("Staff");
        }
    }
}