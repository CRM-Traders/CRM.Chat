using Microsoft.AspNetCore.Http;

namespace CRM.Chat.Application.Common.Services.FileStorage;

public interface IFileStorageService
{
    Task<string> StoreFileAsync(IFormFile file, string path, CancellationToken cancellationToken = default);
    Task<Stream> GetFileAsync(string path, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string path, CancellationToken cancellationToken = default);
}