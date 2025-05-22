using CRM.Chat.Domain.Common.Events;

namespace CRM.Chat.Domain.Entities.Conversations.DomainEvents;

public class MemberDemotedEvent : DomainEvent
{
    public Guid UserId { get; }
    public Guid DemotedBy { get; }

    public MemberDemotedEvent(Guid conversationId, Guid userId, Guid demotedBy)
        : base(conversationId, nameof(Conversation))
    {
        UserId = userId;
        DemotedBy = demotedBy;
        ProcessingStrategy = ProcessingStrategy.Immediate;
    }
}