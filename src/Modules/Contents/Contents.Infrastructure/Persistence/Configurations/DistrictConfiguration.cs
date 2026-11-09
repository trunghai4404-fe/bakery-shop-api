using Contents.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contents.Infrastructure.Persistence.Configurations;

public class DistrictConfiguration : IEntityTypeConfiguration<District>
{
    public void Configure(EntityTypeBuilder<District> builder)
    {
        builder.ToTable("Districts");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).ValueGeneratedNever();

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(d => d.FullName)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(d => d.DivisionType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Codename)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(d => d.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(d => d.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasMany(d => d.Wards)
            .WithOne(w => w.District)
            .HasForeignKey(w => w.DistrictId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(d => d.ProvinceId);
        builder.HasIndex(d => d.IsActive);
        builder.HasIndex(d => d.SortOrder);
    }
}
