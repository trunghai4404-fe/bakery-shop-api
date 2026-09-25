using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using SharedKernel.Abstractions;

namespace Shared.Infrastructure.Services;

public class R2StorageService : IStorageService
{
    private readonly string _bucketName;
    private readonly string _publicUrl;
    private readonly AmazonS3Client _s3Client;

    public R2StorageService(IConfiguration configuration)
    {
        var accountId = configuration["CloudflareR2:AccountId"] ?? throw new ArgumentNullException("CloudflareR2:AccountId is missing");
        var accessKey = configuration["CloudflareR2:AccessKeyId"] ?? throw new ArgumentNullException("CloudflareR2:AccessKeyId is missing");
        var secretKey = configuration["CloudflareR2:SecretAccessKey"] ?? throw new ArgumentNullException("CloudflareR2:SecretAccessKey is missing");
        _bucketName = configuration["CloudflareR2:BucketName"] ?? throw new ArgumentNullException("CloudflareR2:BucketName is missing");
        _publicUrl = configuration["CloudflareR2:PublicDomain"] ?? "";

        var s3Config = new AmazonS3Config
        {
            ServiceURL = $"https://{accountId}.r2.cloudflarestorage.com",
            AuthenticationRegion = "auto",
            ForcePathStyle = true 
        };
        _s3Client = new AmazonS3Client(accessKey, secretKey, s3Config);
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folderPath = "uploads", CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(fileName);
        var newFileName = $"{Guid.NewGuid():N}{extension}";
        var s3Key = string.IsNullOrWhiteSpace(folderPath) ? newFileName : $"{folderPath}/{newFileName}";


        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = s3Key,
            InputStream = fileStream,
            ContentType = contentType,
            DisablePayloadSigning = true
        };

        await _s3Client.PutObjectAsync(putRequest, cancellationToken);
        return $"{_publicUrl}/{s3Key}";
    }
}