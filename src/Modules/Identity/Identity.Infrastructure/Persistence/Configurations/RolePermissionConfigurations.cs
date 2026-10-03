using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public class RolePermissionConfigurations : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");
        builder.HasKey(x => new { x.RoleId, x.PermissionId });
        
        builder.HasOne(r => r.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(r => r.RoleId);
        builder.HasOne(r => r.Permission)
            .WithMany()
            .HasForeignKey(r => r.PermissionId);
    }
}