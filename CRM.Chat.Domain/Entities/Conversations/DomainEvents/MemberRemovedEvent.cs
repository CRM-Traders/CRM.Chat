using CRM.Chat.Domain.Common.Events;

namespace CRM.Chat.Domain.Entities.Conversations.DomainEvents;

public class MemberRemovedEvent : DomainEvent
{
    public Guid UserId { get; }
    public Guid RemovedBy { get; }
    public ICollection<Guid> RemainingMemberIds { get; }

    public MemberRemovedEvent(Guid conversationId, Guid userId, Guid removedBy, ICollection<Guid> remainingMemberIds)
        : base(conversationId, nameof(Conversation))
    {
        UserId = userId;
        RemovedBy = removedBy;
        RemainingMemberIds = remainingMemberIds;
        ProcessingStrategy = ProcessingStrategy.Immediate;
    }
}