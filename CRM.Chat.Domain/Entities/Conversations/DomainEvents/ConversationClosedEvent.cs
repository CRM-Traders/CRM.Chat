﻿using CRM.Chat.Domain.Common.Events;

namespace CRM.Chat.Domain.Entities.Conversations.DomainEvents;

public class ConversationClosedEvent : DomainEvent
{
    public Guid ClosedBy { get; }
    public ICollection<Guid> MemberIds { get; }

    public ConversationClosedEvent(Guid conversationId, Guid closedBy, ICollection<Guid> memberIds)
        : base(conversationId, nameof(Conversation))
    {
        ClosedBy = closedBy;
        MemberIds = memberIds;
        ProcessingStrategy = ProcessingStrategy.Immediate;
    }
}