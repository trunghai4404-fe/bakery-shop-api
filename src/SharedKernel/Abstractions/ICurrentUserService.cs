namespace SharedKernel.Abstractions;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
    bool HasPermission(string permission);
}