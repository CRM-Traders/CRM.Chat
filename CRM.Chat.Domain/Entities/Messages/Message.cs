using CRM.Chat.Domain.Common.Entities;
using CRM.Chat.Domain.Entities.Messages.DomainEvents;
using CRM.Chat.Domain.Entities.Messages.Enums;
using CRM.Chat.Domain.Entities.MessageStatuses;

namespace CRM.Chat.Domain.Entities.Messages;

public class Message : AggregateRoot
{
    public Guid ConversationId { get; private set; }
    public Guid SenderId { get; private set; }
    public string Content { get; private set; }
    public DateTimeOffset SentAt { get; private set; }
    public bool IsEdited { get; private set; }
    public MessageType Type { get; private set; }
    public List<Guid> AttachmentIds { get; private set; } = new();
    private readonly List<MessageStatus> _statuses = new();
    public IReadOnlyCollection<MessageStatus> Statuses => _statuses.AsReadOnly();

    private Message() { }

    public static Message Create(Guid conversationId, Guid senderId, string content,
        MessageType type = MessageType.Text, List<Guid>? attachmentIds = null, string? senderIp = null)
    {
        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Content = content,
            SentAt = DateTimeOffset.UtcNow,
            IsEdited = false,
            Type = type,
            AttachmentIds = attachmentIds ?? new List<Guid>()
        };

        message.SetCreationTracking(senderId.ToString(), senderIp);

        message.AddDomainEvent(new MessageSentEvent(
            message.Id,
            conversationId,
            senderId,
            content,
            type,
            message.AttachmentIds.Any()));

        return message;
    }

    public void Edit(string newContent, string modifiedBy, string? ipAddress)
    {
        if (Content == newContent) return;

        Content = newContent;
        IsEdited = true;
        SetModificationTracking(modifiedBy, ipAddress);

        AddDomainEvent(new MessageEditedEvent(
            Id,
            ConversationId,
            newContent));
    }

    public void MarkAsReadBy(Guid userId)
    {
        AddDomainEvent(new MessageReadEvent(
            Id,
            ConversationId,
            userId));
    }
}