using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasIndex(x => x.Email).IsUnique();
        
        builder.Property(x => x.PhoneNumber) .HasMaxLength(50);
        builder.HasIndex(x => x.PhoneNumber);
        
        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(1000);
        builder.Property(x => x.Avatar).HasMaxLength(500);
        builder.Property(x => x.DateOfBirth);
        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);
        
        builder.Property(x => x.Sex);
        builder.Property(x => x.LastLoginAt);
        builder.Property(x => x.IsDeleted);
        builder.Property(x => x.DeletedAt);
        
        builder.HasMany(x => x.RefreshTokens)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.UserRoles)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
    }
}