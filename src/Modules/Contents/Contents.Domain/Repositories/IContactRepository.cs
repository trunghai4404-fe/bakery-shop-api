using Contents.Domain.Entities;
using Contents.Domain.Enums;
using SharedKernel.Commons;

namespace Contents.Domain.Repositories;

public interface IContactRepository
{
    Task<PagedList<Contact>> GetContactsAsync(
        ContactType? type, bool? isActive, int page, int pageSize, CancellationToken ct);
    Task<List<Contact>> GetActiveAsync(CancellationToken ct);
    Task<Contact?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Contact contact, CancellationToken ct);
    void Delete(Contact contact);
}
