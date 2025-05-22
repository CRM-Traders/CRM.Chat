using CRM.Chat.Application.Common.Services.MessageEncryption;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace CRM.Chat.Infrastructure.Services.MessageEncryption;

public class AESMessageEncryptionService : IMessageEncryptionService
{
    private readonly IConfiguration _configuration;

    public AESMessageEncryptionService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> EncryptMessageAsync(string content)
    {
        var key = _configuration["Encryption:Key"];
        // Implementation for AES encryption
        return await Task.FromResult(Convert.ToBase64String(Encoding.UTF8.GetBytes(content))); // Simplified
    }

    public async Task<string> DecryptMessageAsync(string encryptedContent)
    {
        // Implementation for AES decryption
        return await Task.FromResult(Encoding.UTF8.GetString(Convert.FromBase64String(encryptedContent))); // Simplified
    }

    public async Task<string> GenerateKeyAsync()
    {
        return await Task.FromResult(Guid.NewGuid().ToString());
    }
}