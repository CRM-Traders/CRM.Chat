using CRM.Chat.Domain.Common.Events;
using CRM.Chat.Domain.Entities.Conversations;

namespace CRM.Chat.Domain.Entities.Conversations.DomainEvents;

public class MemberAddedEvent : DomainEvent
{
    public Guid UserId { get; }
    public bool IsAdmin { get; }

    public MemberAddedEvent(Guid conversationId, Guid userId, bool isAdmin)
        : base(conversationId, nameof(Conversation))
    {
        UserId = userId;
        IsAdmin = isAdmin;
        ProcessingStrategy = ProcessingStrategy.Immediate;
    }
}
