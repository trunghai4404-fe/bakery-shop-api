using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfigurations : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token).IsRequired().HasMaxLength(500);
        builder.HasIndex(x => x.Token);
        
        builder.Property(x => x.UserId);
        builder.Property(x => x.CreatedOn);
        builder.Property(x => x.ExpiresOn);
        builder.Property(x => x.RevokedOn);
    }
}