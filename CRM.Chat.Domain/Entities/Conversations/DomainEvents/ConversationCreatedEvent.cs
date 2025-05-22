using CRM.Chat.Domain.Common.Events;
using CRM.Chat.Domain.Entities.Conversations.Enums;

namespace CRM.Chat.Domain.Entities.Conversations.DomainEvents;

public class ConversationCreatedEvent : DomainEvent
{
    public ConversationType Type { get; }
    public ICollection<Guid> MemberIds { get; }
    public string Name { get; }
    public Guid? ExternalUserId { get; }
    public Guid CreatedBy { get; }

    public ConversationCreatedEvent(Guid conversationId, ConversationType type,
        string name, Guid createdBy, ICollection<Guid> memberIds, Guid? externalUserId = null)
        : base(conversationId, nameof(Conversation))
    {
        Type = type;
        Name = name;
        CreatedBy = createdBy;
        MemberIds = memberIds;
        ExternalUserId = externalUserId;
        ProcessingStrategy = ProcessingStrategy.Immediate;
    }
}