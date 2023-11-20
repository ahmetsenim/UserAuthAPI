using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using UserAuthAPI.Models.Concrete;

namespace UserAuthAPI.DataAccess.Configurations
{
    public class ResetPasswordOTPConfiguration : IEntityTypeConfiguration<ResetPasswordOTP>
    {
        public void Configure(EntityTypeBuilder<ResetPasswordOTP> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired().HasDefaultValueSql("0");
            builder.Property(x => x.OTP).HasMaxLength(10);
            builder.Property(x => x.NumberOfAttempts).IsRequired().HasDefaultValueSql("0");
            builder.Property(x => x.ValidityDate).IsRequired().HasColumnType("datetime").HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.PasswordReset).IsRequired().HasDefaultValueSql("0");
        }
    }
}
