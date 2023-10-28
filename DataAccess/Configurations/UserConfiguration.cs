using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using UserAuthAPI.Models.Concrete;

namespace UserAuthAPI.DataAccess.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FullName).HasMaxLength(300);
            builder.Property(x => x.PhoneNumber).HasMaxLength(100);
            builder.Property(x => x.PasswordSalt).IsRequired();
            builder.Property(x => x.PasswordSalt).IsRequired();
            builder.Property(x => x.CreateDate).IsRequired().HasColumnType("datetime").HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.IsActive).IsRequired().HasDefaultValueSql("0");
        }
    }
}
