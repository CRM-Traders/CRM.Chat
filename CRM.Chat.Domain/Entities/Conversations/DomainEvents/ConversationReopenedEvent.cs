using CRM.Chat.Domain.Common.Events;

namespace CRM.Chat.Domain.Entities.Conversations.DomainEvents;

public class ConversationReopenedEvent : DomainEvent
{
    public Guid ReopenedBy { get; }
    public ICollection<Guid> MemberIds { get; }

    public ConversationReopenedEvent(Guid conversationId, Guid reopenedBy, ICollection<Guid> memberIds)
        : base(conversationId, nameof(Conversation))
    {
        ReopenedBy = reopenedBy;
        MemberIds = memberIds;
        ProcessingStrategy = ProcessingStrategy.Immediate;
    }
}