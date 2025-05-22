using CRM.Chat.Domain.Common.Events;

namespace CRM.Chat.Domain.Entities.Messages.DomainEvents;

public class MessageDeletedEvent : DomainEvent
{
    public Guid ConversationId { get; }
    public Guid SenderId { get; }
    public Guid DeletedBy { get; }
    public string Content { get; }
    public ICollection<Guid> RecipientIds { get; }

    public MessageDeletedEvent(Guid messageId, Guid conversationId, Guid senderId,
        Guid deletedBy, string content, ICollection<Guid> recipientIds)
        : base(messageId, nameof(Message))
    {
        ConversationId = conversationId;
        SenderId = senderId;
        DeletedBy = deletedBy;
        Content = content;
        RecipientIds = recipientIds;
        ProcessingStrategy = ProcessingStrategy.Immediate;
    }
}