using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using UserAuthAPI.Models.Concrete;

namespace UserAuthAPI.DataAccess.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired().HasDefaultValueSql("0");
            builder.Property(x => x.RefToken).IsRequired(false);
            builder.Property(x => x.CreateDate).IsRequired().HasColumnType("datetime").HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.ValidityDate).IsRequired().HasColumnType("datetime").HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.IsValid).IsRequired().HasDefaultValueSql("0");
        }
    }
}
