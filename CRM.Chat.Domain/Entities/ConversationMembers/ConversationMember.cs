using CRM.Chat.Domain.Common.Entities;

namespace CRM.Chat.Domain.Entities.ConversationMembers;

public class ConversationMember : AuditableEntity
{
    public Guid ConversationId { get; private set; }
    public Guid UserId { get; private set; }
    public bool IsAdmin { get; private set; }
    public DateTimeOffset JoinedAt { get; private set; }
    public DateTimeOffset? LastSeen { get; private set; }

    private ConversationMember() { }

    public ConversationMember(Guid conversationId, Guid userId, bool isAdmin = false)
    {
        ConversationId = conversationId;
        UserId = userId;
        IsAdmin = isAdmin;
        JoinedAt = DateTimeOffset.UtcNow;
    }

    public void MakeAdmin()
    {
        IsAdmin = true;
    }

    public void RemoveAdmin()
    {
        IsAdmin = false;
    }

    public void UpdateLastSeen()
    {
        LastSeen = DateTimeOffset.UtcNow;
    }
}