﻿using CRM.Chat.Domain.Common.Events;

namespace CRM.Chat.Domain.Entities.Messages.DomainEvents;

public class MessageReadEvent : DomainEvent
{
    public Guid ConversationId { get; }
    public Guid SenderId { get; }
    public Guid ReadByUserId { get; }
    public DateTimeOffset ReadAt { get; }

    public MessageReadEvent(Guid messageId, Guid conversationId, Guid senderId, Guid readByUserId)
        : base(messageId, nameof(Message))
    {
        ConversationId = conversationId;
        SenderId = senderId;
        ReadByUserId = readByUserId;
        ReadAt = DateTimeOffset.UtcNow;
        ProcessingStrategy = ProcessingStrategy.Background;
    }
}