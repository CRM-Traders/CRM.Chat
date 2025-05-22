namespace CRM.Chat.Application.Common.Services.MessageEncryption;

public interface IMessageEncryptionService
{
    Task<string> EncryptMessageAsync(string content);
    Task<string> DecryptMessageAsync(string encryptedContent);
    Task<string> GenerateKeyAsync();
}