using Contracts.DTOs.Files;
using MediatR;
using SharedKernel.Abstractions;
using SharedKernel.Domain;

namespace Contents.Application.Features.Files.Commands;

public class UploadFilesCommandHandler(IStorageService storageService) : IRequestHandler<UploadFilesCommand, Result<UploadFilesResponse>>
{
    public async Task<Result<UploadFilesResponse>> Handle(UploadFilesCommand request, CancellationToken ct)
    {
        var uploadTasks = new List<Task<string>>();
        foreach (var file in request.Files)
        {
            var task = storageService.UploadFileAsync(
                file.Stream, 
                file.FileName, 
                file.ContentType, 
                "Uploads",
                ct);
                
            uploadTasks.Add(task);
        }
        var urls = await Task.WhenAll(uploadTasks);
        
        var response = new UploadFilesResponse 
        {
            Urls = urls.ToList()
        };
        
        return Result.Success(response); 
    }
}   