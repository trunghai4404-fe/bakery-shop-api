using Contents.Contracts.DTOs.Contacts;
using Contents.Domain.Entities;
using Contents.Domain.Enums;
using Contents.Domain.Errors;
using Contents.Domain.Repositories;
using MediatR;
using SharedKernel.Commons;
using SharedKernel.Domain;

namespace Contents.Application.Features.Contacts;

public record GetContactsQuery(int? Type, bool? IsActive, int Page, int PageSize)
    : IRequest<Result<PagedList<ContactDto>>>;
public record GetContactByIdQuery(Guid Id) : IRequest<Result<ContactDto>>;
public record GetActiveContactsQuery : IRequest<Result<List<ActiveContactDto>>>;

public class GetContactsQueryHandler(IContactRepository contacts)
    : IRequestHandler<GetContactsQuery, Result<PagedList<ContactDto>>>
{
    public async Task<Result<PagedList<ContactDto>>> Handle(GetContactsQuery request, CancellationToken ct)
    {
        var type = (ContactType?)request.Type;
        if (type.HasValue && !Enum.IsDefined(type.Value))
            return Result.Failure<PagedList<ContactDto>>(ContentsErrors.InvalidContactType);

        var results = await contacts.GetContactsAsync(type, request.IsActive, request.Page, request.PageSize, ct);
        var items = results.Items.Select(ContactMapping.ToDto).ToList();
        return Result.Success(new PagedList<ContactDto>(items, results.Page, results.PageSize, results.TotalCount));
    }
}

public class GetContactByIdQueryHandler(IContactRepository contacts)
    : IRequestHandler<GetContactByIdQuery, Result<ContactDto>>
{
    public async Task<Result<ContactDto>> Handle(GetContactByIdQuery query, CancellationToken ct)
    {
        var contact = await contacts.GetByIdAsync(query.Id, ct);
        return contact == null
            ? Result.Failure<ContactDto>(ContentsErrors.ContactNotFound)
            : Result.Success(ContactMapping.ToDto(contact));
    }
}

public class GetActiveContactsQueryHandler(IContactRepository contacts)
    : IRequestHandler<GetActiveContactsQuery, Result<List<ActiveContactDto>>>
{
    public async Task<Result<List<ActiveContactDto>>> Handle(GetActiveContactsQuery query, CancellationToken ct)
    {
        var activeContacts = await contacts.GetActiveAsync(ct);
        return Result.Success(activeContacts.Select(ContactMapping.ToActiveDto).ToList());
    }
}

internal static class ContactMapping
{
    public static ContactDto ToDto(Contact contact) =>
        new(contact.Id, contact.Type.ToString(), (int)contact.Type, contact.Value,
            contact.IsActive, contact.CreatedAt, contact.LastModifiedAt);

    public static ActiveContactDto ToActiveDto(Contact contact) =>
        new(contact.Id, contact.Type.ToString(), (int)contact.Type, contact.Value);
}
