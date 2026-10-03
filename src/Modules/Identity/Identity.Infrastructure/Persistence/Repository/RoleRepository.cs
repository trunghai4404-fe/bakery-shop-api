using Identity.Domain.Entities;
using Identity.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Commons;

namespace Identity.Infrastructure.Persistence.Repository;

public class RoleRepository(IdentityDbContext context) : IRoleRepository
{
    public async Task<PagedList<Role>> GetAllAsync(string? search, int page, int pageSize,
        CancellationToken ct = default)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 10 : pageSize;

        var query = context.Roles
            .AsNoTracking()
            .Include(r => r.RolePermissions)
            .ThenInclude(ur => ur.Permission)
            .AsSplitQuery()
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.ToLower().Trim();
            query = query.Where(r =>
                r.Name.ToLower().Contains(keyword) ||
                r.Code.ToLower().Contains(keyword) ||
                (r.Description != null && r.Description.ToLower().Contains(search)));

        }
        var totalCount = await query.CountAsync(ct);
        var roles = await query
            .OrderBy(r => r.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        
        return new PagedList<Role>(roles, page, pageSize, totalCount);
    }

    public async Task<Role?> GetByIdAsync(int roleId, CancellationToken ct = default)
    {
        return await context.Roles
            .FirstOrDefaultAsync(r => r.Id == roleId, ct);
    }

    public async Task<Role?> GetByIdWithPermissionsAsync(int roleId, CancellationToken ct = default)
    {
        return await context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(ur => ur.Permission)
            .AsSplitQuery()
            .FirstOrDefaultAsync(r => r.Id == roleId, ct);
    }

    public async Task<Role?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        return await context.Roles
            .FirstOrDefaultAsync(r => r.Code.ToLower() == code.ToLower(), ct);
    }

    public async Task<List<int>> GetIdsByCodeAsync(IEnumerable<string>? codes, CancellationToken ct = default)
    {
        if (codes is null)
        {
            return new List<int>();
        }

        var upperCode = codes
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Select(c => c.Trim().ToUpper())
            .Distinct()
            .ToList();

        if (upperCode.Count == 0)
        {
            return new List<int>();
        }

        return await context.Roles
            .AsNoTracking()
            .Where(r => upperCode.Contains(r.Code.ToUpper()))
            .Select(r => r.Id)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsAsync(string code, CancellationToken ct = default)
    {
        return await context.Roles
            .AnyAsync(r => r.Code.ToLower() == code.ToLower(), ct);
    }

    public async Task<Role> CreateAsync(Role role, CancellationToken ct = default)
    {
        await context.Roles.AddAsync(role, ct);
        return role;
    }

    public void UpdateAsync(Role role)
    {
        context.Roles.Update(role);
    }

    public void DeleteAsync(Role role)
    {
        context.Roles.Remove(role);
    }
}