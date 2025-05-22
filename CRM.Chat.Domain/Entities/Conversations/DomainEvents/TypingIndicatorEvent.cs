using CRM.Chat.Domain.Common.Events;

namespace CRM.Chat.Domain.Entities.Conversations.DomainEvents;

public class TypingIndicatorEvent : DomainEvent
{
    public Guid UserId { get; }
    public bool IsTyping { get; }
    public ICollection<Guid> MemberIds { get; }

    public TypingIndicatorEvent(Guid conversationId, Guid userId, bool isTyping, ICollection<Guid> memberIds)
        : base(conversationId, nameof(Conversation))
    {
        UserId = userId;
        IsTyping = isTyping;
        MemberIds = memberIds;
        ProcessingStrategy = ProcessingStrategy.Immediate;
    }
}