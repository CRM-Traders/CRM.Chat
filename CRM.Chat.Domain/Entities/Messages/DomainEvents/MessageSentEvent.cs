using CRM.Chat.Domain.Common.Events;
using CRM.Chat.Domain.Entities.Messages.Enums;

namespace CRM.Chat.Domain.Entities.Messages.DomainEvents;

public class MessageSentEvent : DomainEvent
{
    public Guid ConversationId { get; }
    public Guid SenderId { get; }
    public string Content { get; }
    public MessageType MessageType { get; }
    public bool HasAttachment { get; }
    public List<Guid> AttachmentIds { get; }
    public ICollection<Guid> RecipientIds { get; }
    public DateTimeOffset SentAt { get; }

    public MessageSentEvent(Guid messageId, Guid conversationId, Guid senderId,
        string content, MessageType messageType, List<Guid> attachmentIds,
        ICollection<Guid> recipientIds)
        : base(messageId, nameof(Message))
    {
        ConversationId = conversationId;
        SenderId = senderId;
        Content = content;
        MessageType = messageType;
        AttachmentIds = attachmentIds ?? new List<Guid>();
        HasAttachment = AttachmentIds.Any();
        RecipientIds = recipientIds;
        SentAt = DateTimeOffset.UtcNow;
        ProcessingStrategy = ProcessingStrategy.Immediate;
    }
}