using Contents.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contents.Infrastructure.Persistence.Configurations;

public class BannerConfiguration : IEntityTypeConfiguration<Banner>
{
    public void Configure(EntityTypeBuilder<Banner> builder)
    {
        builder.ToTable("Banners");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(b => b.ImageUrl)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(b => b.MobileImageUrl)
            .HasMaxLength(1000);

        builder.Property(b => b.ActionType)
            .IsRequired();

        builder.Property(b => b.TargetValue)
            .HasMaxLength(1000);

        builder.Property(b => b.OpenInNewTab)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(b => b.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(b => b.StartDate);
        builder.Property(b => b.EndDate);

        builder.Property(b => b.Metadata)
            .HasMaxLength(4000);

        builder.Property(b => b.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(b => b.DeletedAt);

        builder.HasQueryFilter(b => !b.IsDeleted);

        builder.HasIndex(b => b.IsActive);
        builder.HasIndex(b => b.CreatedAt);
    }
}
