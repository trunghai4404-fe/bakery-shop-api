using Contents.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contents.Infrastructure.Persistence.Configurations;

public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
{
    public void Configure(EntityTypeBuilder<Province> builder)
    {
        builder.ToTable("Provinces");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.FullName)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(p => p.DivisionType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Codename)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasMany(p => p.Districts)
            .WithOne(d => d.Province)
            .HasForeignKey(d => d.ProvinceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.IsActive);
        builder.HasIndex(p => p.SortOrder);
    }
}
