using Identity.Domain.Entities;
using Identity.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Commons;

namespace Identity.Infrastructure.Persistence.Repository;

public class PermissionRepository(IdentityDbContext context) : IPermissionRepository
{
    public async Task<PagedList<Permission>> GetAllAsync(
        string? search,
        int? roleId,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        page = page < 0 ? 1 : page;
        pageSize = pageSize  <= 0  ? 10 : pageSize;
        var query = context.Permissions
            .AsNoTracking()
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim().ToLower();
            query = query.Where(r =>
                r.Name.ToLower().Contains(keyword) ||
                r.Code.ToLower().Contains(keyword) 
            );
        }

        if (roleId.HasValue)
        {
            query = query.Where(p => context.RolePermissions
                .Any(r => r.RoleId == roleId.Value && r.PermissionId == p.Id));
        }
        
        var totalCount = await query.CountAsync(ct);
        var permission = await query
            .OrderBy(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return new PagedList<Permission>(permission, page, pageSize, totalCount);
    }

    public async Task<List<int>> GetIdsByCodesAsync(IEnumerable<string>? codes, CancellationToken ct = default)
    {
        if (codes is null)
        {
            return new List<int>();
        }

        var upperCodes = codes
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Select(c => c.Trim().ToUpper())
            .Distinct()
            .ToList();
        if (upperCodes.Count == 0)
        {
            return new List<int>();
        }
        
        return await context.Permissions
            .AsNoTracking()
            .Where(p => upperCodes.Contains(p.Code.ToUpper()))
            .Select(p => p.Id)
            .ToListAsync(ct);
    }
    // Task<List<Permission>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default); 
    public async Task<Permission?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(code)) return null;
        return await  context.Permissions.FirstOrDefaultAsync(p => p.Code.ToUpper() == code.ToUpper().Trim(), ct);
    }

    public async Task<Permission?> GetByIdAsync(int? id, CancellationToken ct = default)
    {
        if (id == null) return null;
        return await  context.Permissions.FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<bool> ExistsAsync(string code, CancellationToken ct = default)
    {
        if(string.IsNullOrWhiteSpace(code)) return false;
        return await context.Permissions.AnyAsync(p => p.Code.ToUpper() == code.ToUpper().Trim(), ct);
    }
}