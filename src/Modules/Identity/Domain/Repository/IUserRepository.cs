using Identity.Domain.Entities;
using SharedKernel.Commons;
using SharedKernel.Domain;

namespace Identity.Domain.Repository;

public interface IUserRepository
{
    Task<PagedList<User>> GetAllUsersAsync(string? search, int? roleId, string? roleCode, bool? isDeleted, int page, int pageSize, CancellationToken ct);
    Task<User?> GetUserById(Guid id, CancellationToken ct);
    Task<User?> GetUserByEmail(string email, CancellationToken ct);
    Task<User?> FindUserByRefreshToken(string refreshToken, CancellationToken ct);
    Task<User?> GetUserWithRefreshTokenById(Guid id, CancellationToken ct);
    Task<bool> IsEmailExists(string email, CancellationToken ct);
    Task<Result<User>> AddAsync(User user, CancellationToken ct);
    void Update(User user);
    void Delete(User user);
}