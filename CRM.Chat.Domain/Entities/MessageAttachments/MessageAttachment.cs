using CRM.Chat.Domain.Common.Entities;

namespace CRM.Chat.Domain.Entities.MessageAttachments;

public class MessageAttachment : AuditableEntity
{
    public Guid MessageId { get; private set; }
    public string FileName { get; private set; }
    public string ContentType { get; private set; }
    public long FileSize { get; private set; }
    public string StoragePath { get; private set; }

    private MessageAttachment() { }

    public static MessageAttachment Create(Guid messageId, string fileName, string contentType, long fileSize, string storagePath)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be empty", nameof(fileName));

        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("Content type cannot be empty", nameof(contentType));

        if (fileSize <= 0)
            throw new ArgumentException("File size must be greater than 0", nameof(fileSize));

        if (string.IsNullOrWhiteSpace(storagePath))
            throw new ArgumentException("Storage path cannot be empty", nameof(storagePath));

        var attachment = new MessageAttachment
        {
            MessageId = messageId,
            FileName = fileName,
            ContentType = contentType,
            FileSize = fileSize,
            StoragePath = storagePath
        };

        return attachment;
    }
}