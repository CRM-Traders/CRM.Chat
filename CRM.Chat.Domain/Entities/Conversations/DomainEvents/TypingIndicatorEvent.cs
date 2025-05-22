using CRM.Chat.Domain.Common.Events;

namespace CRM.Chat.Domain.Entities.Conversations.DomainEvents;

public class TypingIndicatorEvent : DomainEvent
{
    public Guid UserId { get; }

    public TypingIndicatorEvent(Guid conversationId, Guid userId)
        : base(conversationId, nameof(Conversation))
    {
        UserId = userId;
        ProcessingStrategy = ProcessingStrategy.Immediate;
    }
}