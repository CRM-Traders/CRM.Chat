using CRM.Chat.Domain.Common.Entities;
using CRM.Chat.Domain.Entities.ConversationMembers;
using CRM.Chat.Domain.Entities.Conversations.DomainEvents;
using CRM.Chat.Domain.Entities.Conversations.Enums;
using CRM.Chat.Domain.Entities.Messages;
using CRM.Chat.Domain.Entities.Messages.DomainEvents;

namespace CRM.Chat.Domain.Entities.Conversations;

public class Conversation : AggregateRoot
{
    public string Name { get; private set; }
    public ConversationType Type { get; private set; }
    public Guid? ExternalUserId { get; private set; }
    private readonly List<Message> _messages = new();
    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();
    private readonly List<ConversationMember> _members = new();
    public IReadOnlyCollection<ConversationMember> Members => _members.AsReadOnly();
    public bool IsActive { get; private set; } = true;

    private Conversation() { }

    // User-operator conversation
    public static Conversation CreateExternalConversation(Guid userId, Guid createdBy)
    {
        var conversation = new Conversation
        {
            Name = $"Support Conversation",
            Type = ConversationType.External,
            ExternalUserId = userId,
            IsActive = true
        };

        conversation.AddDomainEvent(new ConversationCreatedEvent(
            conversation.Id,
            conversation.Type,
            conversation.Name,
            createdBy,
            [userId],
            userId));

        return conversation;
    }

    // Direct message between internal users
    public static Conversation CreateDirectConversation(Guid initiatorId, Guid receiverId)
    {
        var conversation = new Conversation
        {
            Name = $"Direct Message",
            Type = ConversationType.Direct,
            IsActive = true
        };

        conversation._members.Add(new ConversationMember(conversation.Id, initiatorId));
        conversation._members.Add(new ConversationMember(conversation.Id, receiverId));

        conversation.AddDomainEvent(new ConversationCreatedEvent(
            conversation.Id,
            conversation.Type,
            conversation.Name,
            initiatorId,
            new[] { initiatorId, receiverId }));

        return conversation;
    }

    // Group conversation
    public static Conversation CreateGroupConversation(string name, Guid creatorId, List<Guid> memberIds)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Group name cannot be empty", nameof(name));

        var conversation = new Conversation
        {
            Name = name,
            Type = ConversationType.Group,
            IsActive = true
        };

        // Add creator as member and admin
        var creatorMember = new ConversationMember(conversation.Id, creatorId, isAdmin: true);
        conversation._members.Add(creatorMember);

        // Add all other members
        foreach (var memberId in memberIds.Where(id => id != creatorId))
        {
            conversation._members.Add(new ConversationMember(conversation.Id, memberId));
        }

        var allMemberIds = conversation._members.Select(m => m.UserId).ToList();
        conversation.AddDomainEvent(new ConversationCreatedEvent(
            conversation.Id,
            conversation.Type,
            conversation.Name,
            creatorId,
            allMemberIds));

        return conversation;
    }

    public void UpdateName(string newName, Guid updatedBy)
    {
        if (Type != ConversationType.Group)
            throw new InvalidOperationException("Only group conversations can have their name updated");

        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Group name cannot be empty", nameof(newName));

        var oldName = Name;
        Name = newName;

        AddDomainEvent(new ConversationUpdatedEvent(Id, oldName, newName, updatedBy));
    }

    public void AddMessage(Message message)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot add message to inactive conversation");

        _messages.Add(message);
        // Note: MessageSentEvent is raised by the Message entity itself
    }

    public void AddMember(Guid userId, Guid addedBy, bool isAdmin = false)
    {
        if (Type != ConversationType.Group)
            throw new InvalidOperationException("Members can only be added to group conversations");

        if (_members.Any(m => m.UserId == userId && !m.IsDeleted))
            return; // Already a member

        var member = new ConversationMember(Id, userId, isAdmin);
        _members.Add(member);

        var allMemberIds = _members.Where(m => !m.IsDeleted).Select(m => m.UserId).ToList();
        AddDomainEvent(new MemberAddedEvent(Id, userId, addedBy, isAdmin, allMemberIds));
    }

    public void RemoveMember(Guid userId, Guid removedBy)
    {
        if (Type != ConversationType.Group)
            throw new InvalidOperationException("Members can only be removed from group conversations");

        var member = _members.FirstOrDefault(m => m.UserId == userId && !m.IsDeleted);
        if (member == null)
            return; // Not a member

        // Cannot remove the last admin
        if (member.IsAdmin && _members.Count(m => m.IsAdmin && !m.IsDeleted) <= 1)
            throw new InvalidOperationException("Cannot remove the last admin from a group");

        member.SetDeletionTracking(removedBy.ToString(), null);

        var remainingMemberIds = _members.Where(m => !m.IsDeleted).Select(m => m.UserId).ToList();
        AddDomainEvent(new MemberRemovedEvent(Id, userId, removedBy, remainingMemberIds));
    }

    public void PromoteMember(Guid userId, Guid promotedBy)
    {
        if (Type != ConversationType.Group)
            throw new InvalidOperationException("Members can only be promoted in group conversations");

        var member = _members.FirstOrDefault(m => m.UserId == userId && !m.IsDeleted);
        if (member == null)
            throw new InvalidOperationException("User is not a member of this conversation");

        if (member.IsAdmin)
            return; // Already an admin

        member.MakeAdmin();
        AddDomainEvent(new MemberPromotedEvent(Id, userId, promotedBy));
    }

    public void DemoteMember(Guid userId, Guid demotedBy)
    {
        if (Type != ConversationType.Group)
            throw new InvalidOperationException("Members can only be demoted in group conversations");

        var member = _members.FirstOrDefault(m => m.UserId == userId && !m.IsDeleted);
        if (member == null)
            throw new InvalidOperationException("User is not a member of this conversation");

        if (!member.IsAdmin)
            return; // Not an admin

        // Cannot demote the last admin
        if (_members.Count(m => m.IsAdmin && !m.IsDeleted) <= 1)
            throw new InvalidOperationException("Cannot demote the last admin from a group");

        member.RemoveAdmin();
        AddDomainEvent(new MemberDemotedEvent(Id, userId, demotedBy));
    }

    public void Close(Guid closedBy)
    {
        if (!IsActive)
            return; // Already closed

        IsActive = false;
        var memberIds = _members.Where(m => !m.IsDeleted).Select(m => m.UserId).ToList();
        AddDomainEvent(new ConversationClosedEvent(Id, closedBy, memberIds));
    }

    public void Reopen(Guid reopenedBy)
    {
        if (IsActive)
            return; // Already active

        IsActive = true;
        var memberIds = _members.Where(m => !m.IsDeleted).Select(m => m.UserId).ToList();
        AddDomainEvent(new ConversationReopenedEvent(Id, reopenedBy, memberIds));
    }

    public void EmitTypingIndicator(Guid userId, bool isTyping)
    {
        if (!_members.Any(m => m.UserId == userId && !m.IsDeleted))
            throw new InvalidOperationException("User is not a member of this conversation");

        var memberIds = _members.Where(m => !m.IsDeleted && m.UserId != userId).Select(m => m.UserId).ToList();
        AddDomainEvent(new TypingIndicatorEvent(Id, userId, isTyping, memberIds));
    }

    public ICollection<Guid> GetMemberIds()
    {
        return _members.Where(m => !m.IsDeleted).Select(m => m.UserId).ToList();
    }
}