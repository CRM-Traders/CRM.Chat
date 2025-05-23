﻿using CRM.Chat.Domain.Common.Events;

namespace CRM.Chat.Domain.Entities.Messages.DomainEvents;

public class MessageEditedEvent : DomainEvent
{
    public Guid ConversationId { get; }
    public Guid SenderId { get; }
    public string NewContent { get; }
    public string OldContent { get; }
    public DateTimeOffset EditedAt { get; }
    public ICollection<Guid> RecipientIds { get; }

    public MessageEditedEvent(Guid messageId, Guid conversationId, Guid senderId,
        string oldContent, string newContent, ICollection<Guid> recipientIds)
        : base(messageId, nameof(Message))
    {
        ConversationId = conversationId;
        SenderId = senderId;
        OldContent = oldContent;
        NewContent = newContent;
        RecipientIds = recipientIds;
        EditedAt = DateTimeOffset.UtcNow;
        ProcessingStrategy = ProcessingStrategy.Immediate;
    }
}