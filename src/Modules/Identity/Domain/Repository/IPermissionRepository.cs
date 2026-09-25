using Identity.Domain.Entities;
using SharedKernel.Commons;

namespace Identity.Domain.Repository;

public interface IPermissionRepository
{
    Task<PagedList<Permission>> GetAllAsync(string? search,int? roleId, int page, int pageSize,  CancellationToken ct = default);
    Task<List<int>> GetIdsByCodesAsync(IEnumerable<string> codes, CancellationToken ct = default);
    // Task<List<Permission>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default); 
    Task<Permission?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<Permission?> GetByIdAsync(int? id, CancellationToken ct = default);
    Task<bool> ExistsAsync(string code, CancellationToken ct = default);
}