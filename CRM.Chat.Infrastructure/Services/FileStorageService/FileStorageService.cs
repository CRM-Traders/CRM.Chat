using CRM.Chat.Application.Common.Services.FileStorage;
using Microsoft.AspNetCore.Http;

namespace CRM.Chat.Infrastructure.Services.FileStorageService;

public class FileStorageService : IFileStorageService
{
    public async Task<string> StoreFileAsync(IFormFile file, string path, CancellationToken cancellationToken = default)
    {
        // TODO: Implement file storage (Azure Blob, AWS S3, local file system, etc.)
        await Task.Delay(100, cancellationToken);
        return $"storage/{path}/{file.FileName}";
    }

    public async Task<Stream> GetFileAsync(string path, CancellationToken cancellationToken = default)
    {
        // TODO: Implement file retrieval
        await Task.Delay(100, cancellationToken);
        return Stream.Null;
    }

    public async Task DeleteFileAsync(string path, CancellationToken cancellationToken = default)
    {
        // TODO: Implement file deletion
        await Task.Delay(100, cancellationToken);
    }
}
