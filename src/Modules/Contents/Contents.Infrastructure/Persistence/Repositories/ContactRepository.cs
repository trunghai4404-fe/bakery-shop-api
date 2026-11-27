using Contents.Domain.Entities;
using Contents.Domain.Enums;
using Contents.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Commons;

namespace Contents.Infrastructure.Persistence.Repositories;

public class ContactRepository(ContentsDbContext context) : IContactRepository
{
    public async Task<PagedList<Contact>> GetContactsAsync(
        ContactType? type, bool? isActive, int page, int pageSize, CancellationToken ct)
    {
        var query = context.Contacts.AsNoTracking();
        if (type.HasValue)
            query = query.Where(contact => contact.Type == type.Value);
        if (isActive.HasValue)
            query = query.Where(contact => contact.IsActive == isActive.Value);

        page = Math.Max(page, 1);
        pageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, 100);
        var totalCount = await query.CountAsync(ct);
        var offset = (page - 1L) * pageSize;
        var items = offset >= totalCount
            ? []
            : await OrderContacts(query).Skip((int)offset).Take(pageSize).ToListAsync(ct);

        return new PagedList<Contact>(items, page, pageSize, totalCount);
    }

    public Task<List<Contact>> GetActiveAsync(CancellationToken ct) =>
        OrderContacts(context.Contacts.AsNoTracking().Where(contact => contact.IsActive)).ToListAsync(ct);

    public Task<Contact?> GetByIdAsync(Guid id, CancellationToken ct) =>
        context.Contacts.FirstOrDefaultAsync(contact => contact.Id == id, ct);

    public async Task AddAsync(Contact contact, CancellationToken ct)
    {
        await context.Contacts.AddAsync(contact, ct);
    }

    public void Delete(Contact contact) => context.Contacts.Remove(contact);

    private static IOrderedQueryable<Contact> OrderContacts(IQueryable<Contact> query) =>
        query.OrderBy(contact => contact.Type).ThenBy(contact => contact.CreatedAt).ThenBy(contact => contact.Id);
}
