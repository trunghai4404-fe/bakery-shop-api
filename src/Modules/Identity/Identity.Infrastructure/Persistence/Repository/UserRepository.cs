using Identity.Domain.Entities;
using Identity.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Commons;
using SharedKernel.Domain;

namespace Identity.Infrastructure.Persistence.Repository;

public class UserRepository(IdentityDbContext identityDbContext) : IUserRepository
{
    public async Task<PagedList<User>> GetAllUsersAsync(
        string? search,
        int? roleId,
        string? roleCode,
        bool? isDeleted,
        int page,
        int pageSize,
        CancellationToken ct = default
    )
    {
        var query = identityDbContext.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .AsQueryable();
        if (isDeleted.HasValue)
        {
            query = query.Where(u => u.IsDeleted == isDeleted.Value);
        }
        else if (!isDeleted.HasValue)
        {
            query = query.Where(u => !u.IsDeleted);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u =>
                u.FullName.Contains(search) ||
                u.Email.Contains(search) ||
                (u.PhoneNumber != null && u.PhoneNumber.Contains(search)));
        }

        if (roleId.HasValue)
        {
            query = query.Where(u => u.UserRoles.Any(ur => ur.RoleId == roleId.Value));
        }
        else if (!string.IsNullOrWhiteSpace(roleCode))
        {
            var roleUpper = roleCode.ToUpper().Trim();
            query = query.Where(u => u.UserRoles.Any(ur => ur.Role.Code.ToUpper() == roleUpper));
        }

        var totalCount = await query.CountAsync(ct);

        query = query.OrderByDescending(u => u.CreatedAt);

        var skip = (page - 1) * pageSize;
        var users = await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);
        return new PagedList<User>(users, page, pageSize, totalCount);
    }

    public async Task<User?> GetUserById(Guid id, CancellationToken ct)
        => await identityDbContext.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(u => u.RolePermissions)
            .ThenInclude(ur => ur.Permission)
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, ct);

    public async Task<User?> GetUserByEmail(string email, CancellationToken ct)
        => await identityDbContext.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(u => u.RolePermissions)
            .ThenInclude(ur => ur.Permission)
            .Include(u => u.RefreshTokens.Where(rt => rt.RevokedOn == null && rt.ExpiresOn > DateTime.UtcNow))
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, ct);

    public async Task<User?> FindUserByRefreshToken(string refreshToken, CancellationToken ct)
        => await identityDbContext.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(u => u.RolePermissions)
            .ThenInclude(ur => ur.Permission)
            .Include(u => u.RefreshTokens.Where(rt => rt.Token == refreshToken))
            .AsSplitQuery()
            .FirstOrDefaultAsync(u =>
                u.RefreshTokens.Any(rt => rt.Token == refreshToken &&
                                          rt.RevokedOn == null &&
                                          rt.ExpiresOn > DateTime.UtcNow)
                && !u.IsDeleted, ct);

    public async Task<User?> GetUserWithRefreshTokenById(Guid id, CancellationToken ct)
    {
        return await identityDbContext.Users
            .Include(u => u.RefreshTokens
                .Where(rt => rt.RevokedOn == null &&
                             rt.ExpiresOn > DateTime.UtcNow))
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, ct);
    }

    public async Task<Result<User>> AddAsync(User user, CancellationToken ct)
    {
        await identityDbContext.Users.AddAsync(user, ct);
        return Result.Success(user);
    }

    public async Task<bool> IsEmailExists(string email, CancellationToken ct)
    {
        return await identityDbContext.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower() && !u.IsDeleted, ct);
    }

    public void Update(User user)
    {
        identityDbContext.Users.Update(user);
    }

    public void Delete(User user)
    {
        identityDbContext.Users.Remove(user);
    }
}