using CRM.Chat.Domain.Common.Events;

namespace CRM.Chat.Domain.Entities.Conversations.DomainEvents;

public class MemberAddedEvent : DomainEvent
{
    public Guid UserId { get; }
    public Guid AddedBy { get; }
    public bool IsAdmin { get; }
    public ICollection<Guid> AllMemberIds { get; }

    public MemberAddedEvent(Guid conversationId, Guid userId, Guid addedBy, bool isAdmin, ICollection<Guid> allMemberIds)
        : base(conversationId, nameof(Conversation))
    {
        UserId = userId;
        AddedBy = addedBy;
        IsAdmin = isAdmin;
        AllMemberIds = allMemberIds;
        ProcessingStrategy = ProcessingStrategy.Immediate;
    }
}