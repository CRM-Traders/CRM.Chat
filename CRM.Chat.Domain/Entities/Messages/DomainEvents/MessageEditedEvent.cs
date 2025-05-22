using CRM.Chat.Domain.Common.Events;

namespace CRM.Chat.Domain.Entities.Messages.DomainEvents;

public class MessageEditedEvent : DomainEvent
{
    public string NewContent { get; }
    public DateTimeOffset EditedAt { get; }
    public Guid ConversationId { get; }

    public MessageEditedEvent(Guid messageId, Guid conversationId, string newContent)
        : base(messageId, nameof(Message))
    {
        ConversationId = conversationId;
        NewContent = newContent;
        EditedAt = DateTimeOffset.UtcNow;
        ProcessingStrategy = ProcessingStrategy.Immediate;
    }
}