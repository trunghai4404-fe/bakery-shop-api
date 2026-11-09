using Contents.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contents.Infrastructure.Persistence.Configurations;

public class WardConfiguration : IEntityTypeConfiguration<Ward>
{
    public void Configure(EntityTypeBuilder<Ward> builder)
    {
        builder.ToTable("Wards");

        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id).ValueGeneratedNever();

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(w => w.FullName)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(w => w.DivisionType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.Codename)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(w => w.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(w => w.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasIndex(w => w.DistrictId);
        builder.HasIndex(w => w.IsActive);
        builder.HasIndex(w => w.SortOrder);
    }
}
