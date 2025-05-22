using CRM.Chat.Domain.Common.Events;

namespace CRM.Chat.Domain.Entities.Messages.DomainEvents;

public class MessageDeliveredEvent : DomainEvent
{
    public Guid ConversationId { get; }
    public Guid SenderId { get; }
    public Guid DeliveredToUserId { get; }
    public DateTimeOffset DeliveredAt { get; }

    public MessageDeliveredEvent(Guid messageId, Guid conversationId, Guid senderId, Guid deliveredToUserId)
        : base(messageId, nameof(Message))
    {
        ConversationId = conversationId;
        SenderId = senderId;
        DeliveredToUserId = deliveredToUserId;
        DeliveredAt = DateTimeOffset.UtcNow;
        ProcessingStrategy = ProcessingStrategy.Background;
    }
}