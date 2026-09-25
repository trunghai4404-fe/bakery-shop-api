using Identity.Domain.Entities;
using SharedKernel.Commons;

namespace Identity.Domain.Repository;

public interface IRoleRepository
{
    Task<PagedList<Role>> GetAllAsync(string? search, int page, int pageSize, CancellationToken ct = default);
    Task<Role?> GetByIdAsync(int roleId, CancellationToken ct = default);
    Task<Role?> GetByIdWithPermissionsAsync(int roleId, CancellationToken ct = default);
    Task<Role?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<List<int>> GetIdsByCodeAsync(IEnumerable<string>? codes, CancellationToken ct = default);
    Task<bool> ExistsAsync(string code, CancellationToken ct = default);
    Task<Role> CreateAsync(Role role, CancellationToken ct = default);
    void UpdateAsync(Role role);
    void DeleteAsync(Role role);
}