using CRM.Chat.Domain.Common.Events;
using CRM.Chat.Domain.Entities.Conversations;

namespace CRM.Chat.Domain.Entities.Conversations.DomainEvents;

public class MemberRemovedEvent : DomainEvent
{
    public Guid UserId { get; }

    public MemberRemovedEvent(Guid conversationId, Guid userId)
        : base(conversationId, nameof(Conversation))
    {
        UserId = userId;
        ProcessingStrategy = ProcessingStrategy.Immediate;
    }
}