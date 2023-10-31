using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using UserAuthAPI.Models.Concrete;

namespace UserAuthAPI.DataAccess.Configurations
{
    public class OTPConfiguration : IEntityTypeConfiguration<OTP>
    {
        public void Configure(EntityTypeBuilder<OTP> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.OtpCode).IsRequired().HasDefaultValueSql("0");
            builder.Property(x => x.OtpToken).IsRequired(false);
            builder.Property(x => x.UserId).IsRequired().HasDefaultValueSql("0");
            builder.Property(x => x.IsLoggedIn).IsRequired().HasDefaultValueSql("0");
            builder.Property(x => x.NumberOfAttempts).IsRequired().HasDefaultValueSql("0");
            builder.Property(x => x.CreateDate).IsRequired().HasColumnType("datetime").HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.ValidityDate).IsRequired().HasColumnType("datetime").HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.LoginDate).IsRequired(false).HasColumnType("datetime");
            builder.ToTable("OTPs");
        }
    }
}
