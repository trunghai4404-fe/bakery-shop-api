using Contents.Contracts.DTOs.Contacts;
using Contents.Domain.Entities;
using Contents.Domain.Enums;
using Contents.Domain.Errors;
using Contents.Domain.Repositories;
using MediatR;
using SharedKernel.Domain;

namespace Contents.Application.Features.Contacts;

public record CreateContactCommand(ContactRequest Request) : IRequest<Result<ContactDto>>;
public record UpdateContactCommand(Guid Id, ContactRequest Request) : IRequest<Result<ContactDto>>;
public record ToggleContactCommand(Guid Id, bool? IsActive) : IRequest<Result<ToggleContactResponse>>;
public record DeleteContactCommand(Guid Id) : IRequest<Result>;

public class CreateContactCommandHandler(IContactRepository contacts, IContentsUnitOfWork unitOfWork)
    : IRequestHandler<CreateContactCommand, Result<ContactDto>>
{
    public async Task<Result<ContactDto>> Handle(CreateContactCommand command, CancellationToken ct)
    {
        var request = command.Request;
        var result = Contact.Create((ContactType)request.Type, request.Value, request.IsActive);
        if (result.IsFailure)
            return Result.Failure<ContactDto>(result.Error);

        await contacts.AddAsync(result.Value, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(ContactMapping.ToDto(result.Value));
    }
}

public class UpdateContactCommandHandler(IContactRepository contacts, IContentsUnitOfWork unitOfWork)
    : IRequestHandler<UpdateContactCommand, Result<ContactDto>>
{
    public async Task<Result<ContactDto>> Handle(UpdateContactCommand command, CancellationToken ct)
    {
        var contact = await contacts.GetByIdAsync(command.Id, ct);
        if (contact == null)
            return Result.Failure<ContactDto>(ContentsErrors.ContactNotFound);

        var request = command.Request;
        var result = contact.Update((ContactType)request.Type, request.Value, request.IsActive);
        if (result.IsFailure)
            return Result.Failure<ContactDto>(result.Error);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(ContactMapping.ToDto(contact));
    }
}

public class ToggleContactCommandHandler(IContactRepository contacts, IContentsUnitOfWork unitOfWork)
    : IRequestHandler<ToggleContactCommand, Result<ToggleContactResponse>>
{
    public async Task<Result<ToggleContactResponse>> Handle(ToggleContactCommand command, CancellationToken ct)
    {
        var contact = await contacts.GetByIdAsync(command.Id, ct);
        if (contact == null)
            return Result.Failure<ToggleContactResponse>(ContentsErrors.ContactNotFound);

        contact.ToggleStatus(command.IsActive);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(new ToggleContactResponse(contact.Id, contact.IsActive));
    }
}

public class DeleteContactCommandHandler(IContactRepository contacts, IContentsUnitOfWork unitOfWork)
    : IRequestHandler<DeleteContactCommand, Result>
{
    public async Task<Result> Handle(DeleteContactCommand command, CancellationToken ct)
    {
        var contact = await contacts.GetByIdAsync(command.Id, ct);
        if (contact == null)
            return Result.Failure(ContentsErrors.ContactNotFound);

        contacts.Delete(contact);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
