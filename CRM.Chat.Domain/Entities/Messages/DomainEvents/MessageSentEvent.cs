using CRM.Chat.Domain.Common.Events;
using CRM.Chat.Domain.Entities.Messages.Enums;

namespace CRM.Chat.Domain.Entities.Messages.DomainEvents;

public class MessageSentEvent : DomainEvent
{
    public Guid SenderId { get; }
    public string Content { get; }
    public MessageType MessageType { get; }
    public bool HasAttachment { get; }

    public MessageSentEvent(Guid messageId, Guid conversationId, Guid senderId,
        string content, MessageType messageType, bool hasAttachment)
        : base(messageId, nameof(Message))
    {
        SenderId = senderId;
        Content = content;
        MessageType = messageType;
        HasAttachment = hasAttachment;
        // Process immediately for real-time notifications
        ProcessingStrategy = ProcessingStrategy.Immediate;
    }
}