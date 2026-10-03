using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).UseIdentityByDefaultColumn();
        
        builder.Property(p => p.Slug)
            .IsRequired()
            .HasMaxLength(128);
        builder.HasIndex(p => p.Slug).IsUnique();

        builder.Property(p => p.Description)
            .HasMaxLength(256);
        builder.Property(p => p.ParentId);
        builder.Property(p => p.ImageUrl)
            .HasMaxLength(1000);
        builder.Property(p => p.IsActive).IsRequired().HasDefaultValue(false);

        builder.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}