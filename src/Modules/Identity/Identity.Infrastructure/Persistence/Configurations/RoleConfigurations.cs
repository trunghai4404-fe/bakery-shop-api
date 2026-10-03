using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public class RoleConfigurations : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);
        builder.HasIndex(x => x.Code).IsUnique();
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(x => x.Description)
            .HasMaxLength(500);
        
        builder.HasMany(x => x.RolePermissions)
            .WithOne(x => x.Role)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}