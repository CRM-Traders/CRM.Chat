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
    public static Conversation CreateExternalConversation(Guid userId)
    {
        var convesationType = ConversationType.External;

        var conversation = new Conversation
        {
            Name = $"Support Conversation",
            Type = convesationType,
            ExternalUserId = userId,
            IsActive = true
        };

        conversation.AddDomainEvent(new ConversationCreatedEvent(conversation.Id, convesationType, [userId]));
        return conversation;
    }

    // Direct message between internal users
    public static Conversation CreateDirectConversation(Guid initiatorId, Guid receiverId)
    {
        var convesationType = ConversationType.Direct;

        var conversation = new Conversation
        {
            Name = $"Direct Message",
            Type = convesationType,
            IsActive = true
        };

        conversation._members.Add(new ConversationMember(conversation.Id, initiatorId));
        conversation._members.Add(new ConversationMember(conversation.Id, receiverId));

        conversation.AddDomainEvent(new ConversationCreatedEvent(conversation.Id, convesationType, [initiatorId, receiverId]));
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

        conversation.AddDomainEvent(new ConversationCreatedEvent(
            conversation.Id,
            conversation.Type,
            conversation._members.Select(m => m.UserId).ToList()));

        return conversation;
    }

    public void AddMessage(Message message)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot add message to inactive conversation");

        _messages.Add(message);
        AddDomainEvent(new MessageAddedEvent(Id, message.Id));
    }

    public void AddMember(Guid userId, bool isAdmin = false)
    {
        if (Type != ConversationType.Group)
            throw new InvalidOperationException("Members can only be added to group conversations");

        if (_members.Any(m => m.UserId == userId))
            return; // Already a member

        var member = new ConversationMember(Id, userId, isAdmin);
        _members.Add(member);

        AddDomainEvent(new MemberAddedEvent(Id, userId, isAdmin));
    }

    public void RemoveMember(Guid userId)
    {
        if (Type != ConversationType.Group)
            throw new InvalidOperationException("Members can only be removed from group conversations");

        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member == null)
            return; // Not a member

        // Cannot remove the last admin
        if (member.IsAdmin && _members.Count(m => m.IsAdmin) <= 1)
            throw new InvalidOperationException("Cannot remove the last admin from a group");

        _members.Remove(member);

        AddDomainEvent(new MemberRemovedEvent(Id, userId));
    }

    public void Close()
    {
        IsActive = false;
        AddDomainEvent(new ConversationClosedEvent(Id));
    }
}