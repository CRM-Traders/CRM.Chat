using CRM.Chat.Domain.Common.Events;
using CRM.Chat.Domain.Entities.Conversations.Enums;

namespace CRM.Chat.Domain.Entities.Conversations.DomainEvents;

public class ConversationCreatedEvent : DomainEvent
{
    public ConversationType Type { get; }
    public ICollection<Guid> MemberIds { get; }

    public ConversationCreatedEvent(Guid conversationId, ConversationType type, ICollection<Guid> memberIds)
        : base(conversationId, nameof(Conversation))
    {
        Type = type;
        MemberIds = memberIds;
    }
}