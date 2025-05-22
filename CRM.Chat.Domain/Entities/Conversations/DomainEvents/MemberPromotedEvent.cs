using CRM.Chat.Domain.Common.Events;

namespace CRM.Chat.Domain.Entities.Conversations.DomainEvents;

public class MemberPromotedEvent : DomainEvent
{
    public Guid UserId { get; }
    public Guid PromotedBy { get; }

    public MemberPromotedEvent(Guid conversationId, Guid userId, Guid promotedBy)
        : base(conversationId, nameof(Conversation))
    {
        UserId = userId;
        PromotedBy = promotedBy;
        ProcessingStrategy = ProcessingStrategy.Immediate;
    }
}