using CRM.Chat.Domain.Common.Events;
using CRM.Chat.Domain.Entities.Conversations;

namespace CRM.Chat.Domain.Entities.Messages.DomainEvents;

public class MessageAddedEvent : DomainEvent
{
    public Guid MessageId { get; }

    public MessageAddedEvent(Guid conversationId, Guid messageId)
        : base(conversationId, nameof(Conversation))
    {
        MessageId = messageId;
        ProcessingStrategy = ProcessingStrategy.Immediate;
    }
}