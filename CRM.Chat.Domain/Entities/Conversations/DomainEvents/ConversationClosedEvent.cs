using CRM.Chat.Domain.Common.Events;

namespace CRM.Chat.Domain.Entities.Conversations.DomainEvents;

public class ConversationClosedEvent : DomainEvent
{

    public ConversationClosedEvent(Guid conversationId)
        : base(conversationId, nameof(Conversation))
    {
    }
}