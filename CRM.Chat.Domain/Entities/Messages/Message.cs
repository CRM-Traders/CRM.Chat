using CRM.Chat.Domain.Common.Entities;
using CRM.Chat.Domain.Entities.MessageAttachments;
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
    private readonly List<MessageAttachment> _attachments = new();
    public IReadOnlyCollection<MessageAttachment> Attachments => _attachments.AsReadOnly();
    private readonly List<MessageStatus> _statuses = new();
    public IReadOnlyCollection<MessageStatus> Statuses => _statuses.AsReadOnly();

    private Message() { }

    public static Message Create(Guid conversationId, Guid senderId, string content,
        MessageType type = MessageType.Text, string? senderIp = null)
    {
        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Content = content,
            SentAt = DateTimeOffset.UtcNow,
            IsEdited = false,
            Type = type
        };

        message.SetCreationTracking(senderId.ToString(), senderIp);

        // Use the base entity method to create a domain event
        message.AddDomainEvent(new MessageSentEvent(
            message.Id,
            conversationId,
            senderId,
            content,
            type,
            false)); // No attachments by default

        return message;
    }

    public void Edit(string newContent, string modifiedBy, string? ipAddress)
    {
        if (Content == newContent) return;

        Content = newContent;
        IsEdited = true;
        SetModificationTracking(modifiedBy, ipAddress);

        // Use the base entity method to create a domain event
        AddDomainEvent(new MessageEditedEvent(
            Id,
            ConversationId,
            newContent));
    }

    public void MarkAsReadBy(Guid userId)
    {
        // Use the base entity method to create a domain event
        AddDomainEvent(new MessageReadEvent(
            Id,
            ConversationId,
            userId));
    }
}