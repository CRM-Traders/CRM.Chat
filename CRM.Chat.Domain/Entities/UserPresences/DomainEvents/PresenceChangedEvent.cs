using CRM.Chat.Domain.Common.Events;
using CRM.Chat.Domain.Entities.UserPresences.Enums;

namespace CRM.Chat.Domain.Entities.UserPresences.DomainEvents;

public class PresenceChangedEvent : DomainEvent
{
    public Guid UserId { get; }
    public PresenceStatus OldStatus { get; }
    public PresenceStatus NewStatus { get; }

    public PresenceChangedEvent(Guid presenceId, Guid userId,
        PresenceStatus oldStatus, PresenceStatus newStatus)
        : base(presenceId, nameof(UserPresence))
    {
        UserId = userId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        ProcessingStrategy = ProcessingStrategy.Immediate;
    }
}