namespace SharedKernel.Abstractions;

public interface IStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folderPath = "uploads", CancellationToken cancellationToken = default);
    
}