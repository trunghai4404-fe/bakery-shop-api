using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Identity.Domain.Enums;
using Identity.Application.Interfaces;

namespace Identity.Infrastructure.Persistence;

public static class IdentityDataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<IdentityDbContext>>();

        try
        {
            if (context.Database.IsNpgsql())
            {
                await context.Database.MigrateAsync();
            }

            var defaultPermissions = new List<Permission>
            {
                new(PermissionEnum.User.UserView, "Xem người dùng", null),
                new(PermissionEnum.User.UserCreate, "Thêm người dùng", null),
                new(PermissionEnum.User.UserUpdate, "Chỉnh sửa người dùng", null),
                new(PermissionEnum.User.UserDelete, "Xóa người dùng", null),

                new(PermissionEnum.Product.ProductView, "Xem sản phẩm", null),
                new(PermissionEnum.Product.ProductCreate, "Thêm sản phẩm", null),
                new(PermissionEnum.Product.ProductUpdate, "Chỉnh sửa sản phẩm", null),
                new(PermissionEnum.Product.ProductDelete, "Xóa sản phẩm", null),

                new(PermissionEnum.Category.CategoryView, "Xem danh mục", null),
                new(PermissionEnum.Category.CategoryCreate, "Thêm danh mục", null),
                new(PermissionEnum.Category.CategoryUpdate, "Chỉnh sửa danh mục", null),
                new(PermissionEnum.Category.CategoryDelete, "Xóa danh mục", null),
                
                new(PermissionEnum.Role.RoleView, "Xem role", null),
                new(PermissionEnum.Role.RoleCreate, "Thêm role", null),
                new(PermissionEnum.Role.RoleUpdate, "Chỉnh sửa role", null),
                new(PermissionEnum.Role.RoleDelete, "Xóa role", null),
                
                new(PermissionEnum.Permission.PermissionView, "Xem quyền", null),
                new(PermissionEnum.Permission.PermissionUpdate, "Chỉnh sửa quyền", null),

                new(PermissionEnum.Banner.BannerView, "Xem banner", null),
                new(PermissionEnum.Banner.BannerCreate, "Thêm banner", null),
                new(PermissionEnum.Banner.BannerUpdate, "Chỉnh sửa banner", null),
                new(PermissionEnum.Banner.BannerDelete, "Xóa banner", null),
            };

            foreach (var permission in defaultPermissions)
            {
                var existingPermission = await context.Permissions
                    .FirstOrDefaultAsync(p => p.Code.ToLower() == permission.Code.ToLower());
                if (existingPermission == null)
                {
                    await context.Permissions.AddAsync(permission);
                }

                await context.SaveChangesAsync();
                logger.LogInformation("Seed permission successfully");
            }
            
            //seed role admin
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Code.ToUpper() == RoleEnum.Admin);
            if (adminRole == null)
            {
                adminRole = new Role(RoleEnum.Admin, "Administrator", "Quản trị viên hệ thống", isSystemRole: true);
                await context.Roles.AddAsync(adminRole);
                await context.SaveChangesAsync();
                logger.LogInformation("Seed role admin successfully");
            }
            
            //Seed role user
            var userRole = await context.Roles.FirstOrDefaultAsync(r => r.Code.ToUpper() == RoleEnum.User);
            if (userRole == null)
            {
                userRole = new Role(RoleEnum.User, "User", "Người dùng", isSystemRole: false);
                await context.Roles.AddAsync(userRole);
                await context.SaveChangesAsync();
                logger.LogInformation("Seed role user successfully");
            }
            
            var allPermissions = await context.Permissions.ToListAsync();
            var currentRolePermissionIds = await context.RolePermissions
                .Where(rp => rp.RoleId == adminRole.Id)
                .Select(rp => rp.PermissionId)
                .ToListAsync();
            foreach (var permission in allPermissions)
            {
                if (!currentRolePermissionIds.Contains(permission.Id))
                {
                    await context.RolePermissions.AddAsync(new RolePermission(adminRole.Id, permission.Id));
                }
                await context.SaveChangesAsync();
                logger.LogInformation("Seed role permission successfully");
            }

            const string adminEmail = "admin@gmail.com";
            const string adminFullname = "Admin";
            const string password = "Admin@123";
            
            var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == adminEmail.ToLower());

            if (adminUser == null)
            {
                var passwordHash = passwordHasher.HashPassword(password);
                var userResult = User.Create(adminFullname, adminEmail, passwordHash);
                if (userResult.IsSuccess)
                {
                    adminUser = userResult.Value;
                    await context.Users.AddAsync(adminUser);
                    await context.SaveChangesAsync();

                    var role = new UserRole(adminUser.Id, adminRole.Id);
                    await context.UserRoles.AddAsync(role);
                    await context.SaveChangesAsync();
                    
                    logger.LogInformation("Seed user admin successfully");
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured during seeding");
            throw;
        }
    }
}
