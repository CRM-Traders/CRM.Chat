using CRM.Chat.Domain.Common.Events;

namespace CRM.Chat.Domain.Entities.Messages.DomainEvents;

public class MessageReadEvent : DomainEvent
{
    public Guid UserId { get; }
    public Guid ConversationId { get; }

    public MessageReadEvent(Guid messageId, Guid conversationId, Guid userId)
        : base(messageId, nameof(Message))
    {
        ConversationId = conversationId;
        UserId = userId;
        // This can be processed in the background as it's less time-sensitive
        ProcessingStrategy = ProcessingStrategy.Background;
    }
}