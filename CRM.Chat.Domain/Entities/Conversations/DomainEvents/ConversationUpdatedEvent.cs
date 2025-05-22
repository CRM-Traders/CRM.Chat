using CRM.Chat.Domain.Common.Events;

namespace CRM.Chat.Domain.Entities.Conversations.DomainEvents;

public class ConversationUpdatedEvent : DomainEvent
{
    public string NewName { get; }
    public string OldName { get; }
    public Guid UpdatedBy { get; }

    public ConversationUpdatedEvent(Guid conversationId, string oldName, string newName, Guid updatedBy)
        : base(conversationId, nameof(Conversation))
    {
        OldName = oldName;
        NewName = newName;
        UpdatedBy = updatedBy;
        ProcessingStrategy = ProcessingStrategy.Immediate;
    }
}