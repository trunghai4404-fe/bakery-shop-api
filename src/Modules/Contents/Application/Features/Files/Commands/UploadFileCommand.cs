using Contracts.DTOs.Files;
using MediatR;
using SharedKernel.Domain;

namespace Contents.Application.Features.Files.Commands;

public record FileData(Stream Stream, string FileName, string ContentType);

public record UploadFilesCommand(List<FileData> Files) : IRequest<Result<UploadFilesResponse>>;
